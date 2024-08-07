using UnityEngine;

public class IAgentFactory : ScriptableObject
{
	IAgent CreateAgent() { return new AgentSkirmish(); }
}