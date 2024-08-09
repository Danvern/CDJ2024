using BlackboardSystem;
using Pathfinding.BehaviourTrees;
using UnityEngine;
using UnityServiceLocator;

[CreateAssetMenu(fileName = "AgentCharger", menuName = "GameplayDefinitions/AIAgent/Charger", order = 1)]
public class AgentChargerFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentCharger.Builder(entity)
			.WithMinRange(minimumRange)
			.WithMaxRange(maximumRange)
			.Build();
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}