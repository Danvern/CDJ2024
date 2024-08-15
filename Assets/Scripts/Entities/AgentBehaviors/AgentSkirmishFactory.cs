using UnityEngine;

[CreateAssetMenu(fileName = "AgentSkirmish", menuName = "GameplayDefinitions/AIAgent/Skirmish", order = 1)]
public class AgentSkirmishFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentSkirmish.Builder(entity)
			.WithMinRange(minimumRange)
			.WithMaxRange(maximumRange)
			.WithDashPower(dashPower)
			.WithDashDuration(dashDuration)
			.WithHoldAfterFire(holdAfterFire)
			.Build();
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;
	[SerializeField] float dashPower;
	[SerializeField] float dashDuration;
	[SerializeField] float holdAfterFire;

}