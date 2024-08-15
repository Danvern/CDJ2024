using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

public class AgentStalker : IAgent
{
	private EntityMediator entity;
	private BehaviourTree tree;
	BlackboardKey isRetreatingKey;
	BlackboardKey targetKey;
	BlackboardKey targetPosition;

	public float MinimumRange { get; set; } = 0;
	public float MaximumRange { get; set; } = 0;
	public float MinimumRangeAlternate { get; set; } = 0;
	public float MaximumRangeAlternate { get; set; } = 0;
	public float SensingRange { get; set; } = 0;

	public float DashPower { get; set; } = 8;
	public float DashDuration { get; set; } = 0.1f;
	public float HoldAfterFire { get; set; }

	public class Builder
	{
		private EntityMediator entity;
		private float maxRange = 2;
		private float minRange = 0;
		private float maxRangeAlternate = 8;
		private float minRangeAlternate = 4;
		private float senseRange = 16;
		float power;
		float duration;
		float holdAfterFire;
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
		public Builder WithMaxRangeAlternate(float max)
		{
			maxRangeAlternate = max;
			return this;
		}
		public Builder WithMinRangeAlternate(float min)
		{
			minRangeAlternate = min;
			return this;
		}
		public Builder WithSenseRange(float sense)
		{
			senseRange = sense;
			return this;
		}
		public Builder WithDashPower(float power)
		{
			this.power = power;
			return this;
		}
		public Builder WithDashDuration(float duration)
		{
			this.duration = duration;
			return this;
		}
		public Builder WithHoldAfterFire(float duration)
		{
			this.holdAfterFire = duration;
			return this;
		}
		public AgentStalker Build()
		{
			var agent = new AgentStalker(entity)
			{
				MaximumRange = maxRange,
				MinimumRange = minRange,
				MaximumRangeAlternate = maxRangeAlternate,
				MinimumRangeAlternate = minRangeAlternate,
				SensingRange = senseRange,
				DashPower = power,
				DashDuration = duration,
				HoldAfterFire = holdAfterFire,
			};
			return agent;
		}
	}


	private AgentStalker(EntityMediator entity) => this.entity = entity;
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
		EntityMediator GetTarget()
		{
			if (blackboard.TryGetValue(targetKey, out EntityMediator target))
				return target;
			return null;
		}
		Vector2 GetTargetPosition()
		{
			if (blackboard.TryGetValue(targetPosition, out Vector2 target))
				return target;
			return Vector2.zero;
		}
		bool IsInSight(EntityMediator targetEntity)
		{
			Vector2 ray = GetTargetPosition() - entity.GetPosition();
			return !Physics2D.Raycast(entity.GetPosition(), direction: ray.normalized, distance: ray.magnitude, layerMask: LayerMask.GetMask("EnviromentObstacles"));
		}
		bool IsInRange(Vector2 targetPosition)
		{
			float distance = Vector3.Distance(targetPosition, entity.GetPosition());
			return distance < MaximumRange;
		}
		bool IsInAlternateRange(Vector2 targetPosition)
		{
			float distance = Vector3.Distance(targetPosition, entity.GetPosition());
			return distance < MaximumRangeAlternate && distance > MinimumRangeAlternate;
		}
		Condition isAlive = new Condition(() => !entity.IsDead());
		Condition isDead = new Condition(() => entity.IsDead());

		Sequence attackTarget = new Sequence("AttackTarget", 100);
		attackTarget.AddChild(new Leaf("IsAlive?", isAlive));
		attackTarget.AddChild(new Leaf("isTargetNear?", new Condition(() => GetTarget() != null && IsInSight(GetTarget()) && IsInRange(GetTargetPosition()))));
		attackTarget.AddChild(new Leaf("AttackPlayer", new AttackTowardsDirection(entity, () => GetTargetPosition())));
		attackTarget.AddChild(new Leaf("DashForward", new DashFromTarget(entity, () => GetTargetPosition(), DashPower, DashDuration, new MovementStrategyForwards())));
		attackTarget.AddChild(new Leaf("Wait", new WaitStrategy(HoldAfterFire)));
		attackTarget.AddChild(new Leaf("Stop", new StopMoving(entity)));
		actions.AddChild(attackTarget);


		Sequence snipeTarget = new Sequence("AttackTarget", 150);
		snipeTarget.AddChild(new Leaf("IsAlive?", isAlive));
		snipeTarget.AddChild(new Leaf("isTargetNear?", new Condition(() => GetTarget() != null && IsInAlternateRange(GetTargetPosition()))));
		snipeTarget.AddChild(new Leaf("Stop", new StopMoving(entity)));
		snipeTarget.AddChild(new Leaf("AttackPlayer", new AttackTowardsDirection(entity, () => GetTargetPosition(), primary: false)));
		//snipeTarget.AddChild(new Leaf("DashForward", new DashFromTarget(entity, () => GetTargetPosition(), DashPower, DashDuration, new MovementStrategyBackwardsRandom())));
		//snipeTarget.AddChild(new Leaf("Wait", new WaitStrategy(HoldAfterFire)));
		actions.AddChild(snipeTarget);

		Selector goToPlayer = new Selector("GoToPlayer", 50);
		Sequence goDirectly = new Sequence("ApproachPlayerDirectly");
		goDirectly.AddChild(new Leaf("IsAlive?", isAlive));
		goDirectly.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null && IsInSight(GetTarget()))));
		goDirectly.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTargetPosition(), entity.GetPosition()) < SensingRange)));
		goDirectly.AddChild(new Leaf("GoToPlayer", new MoveToTarget(entity, () => GetTargetPosition(), interruptable: true)));
		goToPlayer.AddChild(goDirectly);

		Sequence goPathing = new Sequence("ApproachPlayerPathing");
		goPathing.AddChild(new Leaf("IsAlive?", isAlive));
		goPathing.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget() != null)));
		goPathing.AddChild(new Leaf("isNear?", new Condition(() => Vector2.Distance(GetTargetPosition(), entity.GetPosition()) < SensingRange)));
		goPathing.AddChild(new Leaf("GoToPlayer", new NavigateToTargetDynamic(entity, () => (GetTarget()?.GetTransform()))));
		goToPlayer.AddChild(goPathing);
		//goDirectly.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
		actions.AddChild(goToPlayer);

		Sequence goPatrol = new Sequence("GoPatrol");
		goPatrol.AddChild(new Leaf("IsAlive?", isAlive));
		goPatrol.AddChild(new Leaf("Patrol", new RandomPatrolStrategy(entity)));
		actions.AddChild(goPatrol);

		Sequence perish = new Sequence("Perish");
		perish.AddChild(new Leaf("Dying", isDead));

		tree.AddChild(actions);
	}

	public int GetInsistence(Blackboard blackboard) => blackboard.TryGetValue(targetKey, out EntityMediator target) ? 25 : 10;
	public void Execute(Blackboard blackboard)
	{
		blackboard.AddAction(() =>
		{
			if (!blackboard.TryGetValue(targetKey, out EntityMediator target) || target == null || target.GetTransform().OrNull() == null)
			{
				blackboard.SetValue(targetKey, ServiceLocator.ForSceneOf(entity.GetEntity()).Get<AgentDirector>().GetPrimaryPlayer());
			}
			else
			{
				blackboard.SetValue(targetPosition, target.GetPosition());
			}
		});
	}
	public void Update() => tree.Process(); //TODO: should be unified running of the behavior tree + a bootstrapping function
}