using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

[CreateAssetMenu(fileName = "AgentSkirmish", menuName = "GameplayDefinitions/AIAgent/Skirmish", order = 1)]
public class AgentSkirmishFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity) { return new AgentSkirmish(entity); }
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}

public class AgentSkirmish : IAgent
{
	private EntityMediator entity;
	private BehaviourTree tree;
	BlackboardKey isRetreatingKey;
	BlackboardKey targetKey;


	public AgentSkirmish(EntityMediator entity) => this.entity = entity;
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