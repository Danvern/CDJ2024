using UnityEngine;

[CreateAssetMenu(fileName = "AgentStalker", menuName = "GameplayDefinitions/AIAgent/Stalker", order = 1)]
public class AgentStalkerFactory : IAgentFactory
{
	public override IAgent CreateAgent(EntityMediator entity)
	{
		var agent = new AgentStalker.Builder(entity)
			.WithMinRange(minimumRange)
			.WithMaxRange(maximumRange)
			.WithMaxRangeAlternate(maximumRange)
			.WithMinRangeAlternate(minimumRange)
			.WithDashPower(dashPower)
			.WithDashDuration(dashDuration)
			.WithHoldAfterFire(holdAfterFire)
			.Build();
		agent.BootstrapBehaviorTree();
		return agent;
	}
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;
	[SerializeField] float minimumRangeAlternate;
	[SerializeField] float maximumRangeAlternate;
	[SerializeField] float dashPower;
	[SerializeField] float dashDuration;
	[SerializeField] float holdAfterFire;

}