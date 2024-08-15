using UnityEngine;

[CreateAssetMenu(fileName = "AgentBasic", menuName = "GameplayDefinitions/AIAgent/Basic", order = 1)]
public class AgentBasicFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentBasic.Builder(entity)
			.WithMinRange(minimumRange)
			.WithMaxRange(maximumRange)
			.Build();
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}