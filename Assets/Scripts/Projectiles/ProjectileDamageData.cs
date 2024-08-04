using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDamageData", menuName = "GameplayDefinitions/DamageData", order = 1)]
public class ProjectileDamageData : ScriptableObject
{
	public float Lifetime = 3;
    public float DamageMax = 0;
    public float DamageMin = 1;
	public int Piercing = 0;
	public float CollisionRadius = 1;
	public float CollisionArc = 360;
	public EventReference SmallKillSFX;
	public EventReference DamageSFX;
}
