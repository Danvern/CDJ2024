using UnityEngine;

public delegate void KillNotification(ProjectileBase killer);
public delegate void DamageNotification(float damage, ProjectileBase source);
public class EntityHealthLogic : IEntityHealthLogic
{
	public event KillNotification entityKilled;
	public event DamageNotification entityDamaged;
	private float healthCurrent = 0;
	private float healthMax = 0;

	public EntityHealthLogic(EntityHealthData data)
	{
		healthCurrent = data.HealthCurrent;
		healthMax = data.HealthMax;
	}

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	public float GetHealthMax() { return healthMax; }
	
	public float GetHealthCurrent() { return healthCurrent; }

	public void DoDamage(float damage) 
	{ 
		DoDamage(damage, null);
	}

	public void DoDamage(float damage, ProjectileBase source) 
	{ 
		healthCurrent = Mathf.Max(0, healthCurrent - damage);
		entityDamaged?.Invoke(damage, source);
		if (healthCurrent <= 0)
			entityKilled?.Invoke(source);
	}

	public override string ToString() { return "" + GetHealthCurrent() + "/" + GetHealthMax(); }
}
