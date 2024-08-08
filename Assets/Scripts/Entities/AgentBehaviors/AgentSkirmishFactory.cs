using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

[CreateAssetMenu(fileName = "AgentSkirmish", menuName = "GameplayDefinitions/AIAgent/Skirmish", order = 1)]
public class AgentSkirmishFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentSkirmish.Builder(entity)
			.WithMinRange(minimumRange)
			.WithMaxRange(maximumRange)
			.Build();
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}