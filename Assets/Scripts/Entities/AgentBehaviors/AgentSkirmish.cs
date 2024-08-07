using UnityEngine;

[CreateAssetMenu(fileName = "AgentSkirmish", menuName = "GameplayDefinitions/AIAgent/Skirmish", order = 1)]
public class AgentSkirmishFactory : IAgentFactory
{
	[SerializeField] float minimumRange;
	[SerializeField] float maximumRange;

}

public class AgentSkirmish : IAgent
{

	public void Update() {} //TODO: should be unified running of the behavior tree + a bootstrapping function
}