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

	public class Builder
	{
		private EntityMediator entity;
		private float maxRange;
		private float minRange;
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
		public AgentSkirmish Build()
		{
			var agent = new AgentSkirmish(entity);
			agent.MaximumRange = maxRange;
			agent.MinimumRange = minRange;
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
		Transform GetTarget()
		{
			if (blackboard.TryGetValue(targetKey, out Transform target))
				return target;
			return null;
		}
		
		runToSafetySeq.AddChild(new Leaf("IsRetreating?", new Condition(IsRetreating)));
		runToSafetySeq.AddChild(new Leaf("Go To Safety", new MoveToTarget(entity, GetTarget())));
		actions.AddChild(runToSafetySeq);

		Selector goToPlayer = new RandomSelector("GoToTreasure", 50);
		Sequence goDirectly = new Sequence("GetTreasure1");
		goDirectly.AddChild(new Leaf("isTarget?", new Condition(() => GetTarget().OrNull() != null)));
		goDirectly.AddChild(new Leaf("GoToTarget", new MoveToTarget(entity, GetTarget())));
		//goDirectly.AddChild(new Leaf("PickUpTreasure1", new ActionStrategy(() => treasure.SetActive(false))));
		goToPlayer.AddChild(goDirectly);

		actions.AddChild(goToPlayer);

		Leaf patrol = new Leaf("Patrol", new RandomPatrolStrategy(entity));
		actions.AddChild(patrol);

		tree.AddChild(actions);
	}

	public int GetInsistence(Blackboard blackboard) => 0;
	public void Execute(Blackboard blackboard)
	{

	}
	public void Update() => tree.Process(); //TODO: should be unified running of the behavior tree + a bootstrapping function
}