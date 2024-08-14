using System;
using UnityEngine;

public delegate void KillNotification(ProjectileBase killer);
public delegate void DamageNotification(float damage, ProjectileBase source);
[Serializable]
public class EntityHealthLogic : IEntityHealthLogic
{
	public event KillNotification EntityKilled;
	public event DamageNotification EntityDamaged;
	private float healthCurrent = 0;
	private float healthMax = 0;
	private bool invulnerable = false;

	public EntityHealthLogic(EntityHealthData data)
	{
		healthCurrent = data.HealthCurrent;
		healthMax = data.HealthMax;
	}

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	public float GetHealthMax() { return healthMax; }

	public float GetHealthCurrent() { return healthCurrent; }

	public void SetInvulnerable(bool invulnerable) { this.invulnerable = invulnerable; }

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
		if (invulnerable) return;

		healthCurrent = Mathf.Max(0, healthCurrent - damage);
		EntityDamaged?.Invoke(damage, source);
		if (healthCurrent <= 0)
			EntityKilled?.Invoke(source);
	}

	public override string ToString() { return "" + GetHealthCurrent() + "/" + GetHealthMax(); }
}
