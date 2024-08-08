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


		// runToSafetySeq.AddChild(new Leaf("IsRetreating?", new Condition(IsRetreating)));
		// runToSafetySeq.AddChild(new Leaf("Go To Safety", new MoveToTarget(entity, GetTarget())));
		// actions.AddChild(runToSafetySeq);
		Sequence attackTarget = new Sequence("AttackTarget", 100);
		attackTarget.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null)));
		attackTarget.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTarget().GetPosition(), entity.GetPosition()) < MaximumRange)));
		attackTarget.AddChild(new Leaf("AttackPlayer", new ActionStrategy(()=>{
			entity.PrimaryFire(true);
		})));
		attackTarget.AddChild(new Leaf("AttackPlayer", new ActionStrategy(()=>{
			entity.PrimaryFire(false);
		})));
		actions.AddChild(attackTarget);

		Selector goToPlayer = new RandomSelector("GoToPlayer", 50);
		Sequence goDirectly = new Sequence("ApproachPlayer");
		goDirectly.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null)));
		goDirectly.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTarget().GetPosition(), entity.GetPosition()) < SensingRange)));
		goDirectly.AddChild(new Leaf("GoToPlayer", new MoveToTargetFunction(entity, ()=>(GetTarget()?.GetTransform()))));
		//goDirectly.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
		goToPlayer.AddChild(goDirectly);
		actions.AddChild(goToPlayer);

		Leaf patrol = new Leaf("Patrol", new RandomPatrolStrategy(entity));
		actions.AddChild(patrol);

		tree.AddChild(actions);
	}

	public int GetInsistence(Blackboard blackboard) => !blackboard.TryGetValue(targetKey, out EntityMediator target) ? 10 : 0;
	public void Execute(Blackboard blackboard)
	{
		blackboard.AddAction(() =>
		{
			if (!blackboard.TryGetValue(targetKey, out EntityMediator target) || target == null)
			{
				blackboard.SetValue(targetKey, ServiceLocator.Global.Get<AgentDirector>().GetPrimaryPlayer());
			}
		});
	}
	public void Update() => tree.Process(); //TODO: should be unified running of the behavior tree + a bootstrapping function
}