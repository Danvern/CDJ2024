using UnityEngine;

public abstract class IAgentFactory : ScriptableObject
{
	public abstract IAgent CreateAgent(EntityMediator entity);
}