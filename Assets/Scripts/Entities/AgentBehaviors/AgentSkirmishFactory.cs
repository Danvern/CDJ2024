using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

[CreateAssetMenu(fileName = "AgentSkirmish", menuName = "GameplayDefinitions/AIAgent/Skirmish", order = 1)]
public class AgentSkirmishFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentSkirmish(entity);
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}