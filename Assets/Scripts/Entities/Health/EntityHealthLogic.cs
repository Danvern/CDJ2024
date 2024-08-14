using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void KillNotification(ProjectileBase killer);
public delegate void DamageNotification(float damage, ProjectileBase source);
public enum InvincibilitySource {Dashing, DamageFrames}
[Serializable]
public class EntityHealthLogic : IEntityHealthLogic
{
	public event KillNotification EntityKilled;
	public event DamageNotification EntityDamaged;
	private float healthCurrent = 0;
	private float healthMax = 0;
	private float damageInterval = 0;
	private float lastDamageTime = 0;
	private Dictionary<InvincibilitySource, bool> invulnerable = new();

	public EntityHealthLogic(EntityHealthData data)
	{
		healthCurrent = data.HealthCurrent;
		healthMax = data.HealthMax;
		damageInterval = data.DamageInterval;
	}

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	public float GetHealthMax() { return healthMax; }

	public float GetHealthCurrent() { return healthCurrent; }

	public void SetInvulnerable(bool invulnerable, InvincibilitySource source) { this.invulnerable[source] = invulnerable; }
	public bool IsInvulnerable() 
	{ 
		return invulnerable.ContainsValue(true);
	}

	public void Heal(float value)
	{
		healthCurrent = Mathf.Max(healthMax, healthCurrent + value);
		EntityDamaged?.Invoke(-value, null);
	}

	public void DoDamage(float damage)
	{
		DoDamage(damage, null);
	}

	public void DoDamage(float damage, ProjectileBase source)
	{
		if (IsInvulnerable()) return;

		healthCurrent = Mathf.Max(0, healthCurrent - damage);
		EntityDamaged?.Invoke(damage, source);
		if (damage > 0 && damageInterval > 0)
		{
			SetInvulnerable(true, InvincibilitySource.DamageFrames);
			lastDamageTime = Time.time;
		}
		if (healthCurrent <= 0)
			EntityKilled?.Invoke(source);
	}

	public void Update()
	{
		if (IsInvulnerable() && Time.time - lastDamageTime > damageInterval)
		{
			SetInvulnerable(false, InvincibilitySource.DamageFrames);
		}

	}

	public override string ToString() { return "" + GetHealthCurrent() + "/" + GetHealthMax(); }
}
