using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDamageData", menuName = "GameplayDefinitions/DamageData", order = 1)]
public class ProjectileDamageData : ScriptableObject
{
	public float Lifetime = 3;
    public float DamageMax = 0;
    public float DamageMin = 1;
	public int Piercing = 0;
	public float Knockback = 1;
	public float KnockbackStun = 0.25f;
	public float HitLag = 0;
	public bool IsIndescriminate = false;
	public bool IsExplosion = false;
	public bool IsProjectileDestroyer = false;
	public bool IsHitEffectOnlyOnPierce = false;
	public bool IsBlockedByWorld = false;
	public bool IsInvisibleOnDeath = true;
	public bool DetachOnDeath = false;
	public float CollisionRadius = 1;
	public float CollisionArc = 360;
	public float StartingVelocity = 10f;

	public GameObject HitEffect;
	public GameObject DeathEffect;
	public EventReference SmallKillSFX;
	public EventReference DamageSFX;
	public EventReference HeavyKillSFX;
	public EventReference HeavyDamageSFX;
}
