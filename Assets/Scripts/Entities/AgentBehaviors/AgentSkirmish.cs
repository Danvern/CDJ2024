using UnityEngine;

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
	public AgentSkirmish(EntityMediator entity) => this.entity = entity;
	public void Update() {} //TODO: should be unified running of the behavior tree + a bootstrapping function
}