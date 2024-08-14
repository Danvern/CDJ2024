using UnityEngine;

[CreateAssetMenu(fileName = "EntityHealthData", menuName = "GameplayDefinitions/Entity/HealthData", order = 1)]
public class EntityHealthData : ScriptableObject
{
	public float HealthCurrent = 0;
	public float HealthMax = 0;
	public float DamageInterval = 0;
}
