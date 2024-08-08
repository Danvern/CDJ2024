using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

public class AgentSkirmish : IAgent
{
	private EntityMediator entity;
	private BehaviourTree tree;
	BlackboardKey isRetreatingKey;
	BlackboardKey targetKey;
	BlackboardKey targetPosition;

	public float MinimumRange { get; set; } = 0;
	public float MaximumRange { get; set; } = 0;
	public float SensingRange { get; set; } = 0;

	public class Builder
	{
		private EntityMediator entity;
		private float maxRange = 8;
		private float minRange = 4;
		private float senseRange = 16;
		public Builder(EntityMediator entity)
		{
			this.entity = entity;
		}
		public Builder WithMaxRange(float max)
		{
			maxRange = max;
			return this;
		}
		public Builder WithMinRange(float min)
		{
			minRange = min;
			return this;
		}
		public Builder WithSenseRange(float sense)
		{
			senseRange = sense;
			return this;
		}
		public AgentSkirmish Build()
		{
			var agent = new AgentSkirmish(entity);
			agent.MaximumRange = maxRange;
			agent.MinimumRange = minRange;
			agent.SensingRange = senseRange;
			return agent;
		}
	}


	private AgentSkirmish(EntityMediator entity) => this.entity = entity;
	public void BootstrapBehaviorTree()
	{
		Blackboard blackboard = entity.GetServiceLocator().Get<BlackboardController>().GetBlackboard();
		entity.GetServiceLocator().Get<BlackboardController>().RegisterExpert(this);

		isRetreatingKey = blackboard.GetOrRegisterKey("IsRetreating");
		targetKey = blackboard.GetOrRegisterKey("Target");
		targetPosition = blackboard.GetOrRegisterKey("TargetPosition");

		tree = new BehaviourTree("Skirmisher");
		PrioritySelector actions = new PrioritySelector("Agent Logic");

		Sequence runToSafetySeq = new Sequence("RunToSafety", 100);
		bool IsRetreating()
		{
			if (blackboard.TryGetValue(isRetreatingKey, out bool isSafe))
			{
				if (!isSafe)
				{
					runToSafetySeq.Reset();
					return true;
				}
			}
			return false;
		}
		EntityMediator GetTarget()
		{
			if (blackboard.TryGetValue(targetKey, out EntityMediator target))
				return target;
			return null;
		}
		Vector3 GetTargetPosition()
		{
			if (blackboard.TryGetValue(targetPosition, out Vector3 target))
				return target;
			return Vector3.zero;
		}
		bool IsInSight(EntityMediator targetEntity)
		{
			var ray = GetTargetPosition() - entity.GetPosition();
			return !Physics2D.Raycast(entity.GetPosition(), direction: ray.normalized, distance: ray.magnitude, layerMask: LayerMask.GetMask("EnviromentObstacles") );
		}

		// runToSafetySeq.AddChild(new Leaf("IsRetreating?", new Condition(IsRetreating)));
		// runToSafetySeq.AddChild(new Leaf("Go To Safety", new MoveToTarget(entity, GetTarget())));
		// actions.AddChild(runToSafetySeq);
		Sequence attackTarget = new Sequence("AttackTarget", 100);
		attackTarget.AddChild(new Leaf("isTargetNear?", new Condition(() => GetTarget() != null && IsInSight(GetTarget()) && Vector2.Distance(GetTargetPosition(), entity.GetPosition()) < MaximumRange)));
		attackTarget.AddChild(new Leaf("Stop", new StopMoving(entity)));
		attackTarget.AddChild(new Leaf("AttackPlayer", new AttackTowardsDirection(entity, () => GetTargetPosition())));
		// attackTarget.AddChild(new Leaf("AttackPlayer", new ActionStrategy(()=>{
		// 	entity.PrimaryFire(true);
		// })));
		// attackTarget.AddChild(new Leaf("AttackPlayer", new ActionStrategy(()=>{
		// 	entity.PrimaryFire(false);
		// })));
		actions.AddChild(attackTarget);

		Selector goToPlayer = new Selector("GoToPlayer", 50);
		Sequence goDirectly = new Sequence("ApproachPlayerDirectly");
		goDirectly.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null && IsInSight(GetTarget()))));
		goDirectly.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTargetPosition(), entity.GetPosition()) < SensingRange)));
		goDirectly.AddChild(new Leaf("GoToPlayer", new MoveToTarget(entity, () => GetTargetPosition())));
		goToPlayer.AddChild(goDirectly);

		Sequence goPathing = new Sequence("ApproachPlayerPathing");
		goPathing.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null)));
		goPathing.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTargetPosition(), entity.GetPosition()) < SensingRange)));
		goPathing.AddChild(new Leaf("GoToPlayer", new NavigateToTargetDynamic(entity, () => (GetTarget()?.GetTransform()))));
		goToPlayer.AddChild(goPathing);
		//goDirectly.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
		actions.AddChild(goToPlayer);

		Leaf patrol = new Leaf("Patrol", new RandomPatrolStrategy(entity));
		actions.AddChild(patrol);

		tree.AddChild(actions);
	}

	public int GetInsistence(Blackboard blackboard) => blackboard.TryGetValue(targetKey, out EntityMediator target) ? 25 : 10;
	public void Execute(Blackboard blackboard)
	{
		blackboard.AddAction(() =>
		{
			if (!blackboard.TryGetValue(targetKey, out EntityMediator target) || target == null || target.GetTransform().OrNull() == null)
			{
				blackboard.SetValue(targetKey, ServiceLocator.Global.Get<AgentDirector>().GetPrimaryPlayer());
			}
			else
			{
				blackboard.SetValue(targetPosition, target.GetPosition());
			}
		});
	}
	public void Update() => tree.Process(); //TODO: should be unified running of the behavior tree + a bootstrapping function
}