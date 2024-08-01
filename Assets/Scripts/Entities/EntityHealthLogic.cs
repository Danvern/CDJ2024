using UnityEngine;

public class EntityHealthLogic : IEntityHealthLogic
{
	private float healthCurrent = 0;
	private float healthMax = 0;

	public EntityHealthLogic(EntityHealthData data)
	{
		healthCurrent = data.HealthCurrent;
		healthMax = data.HealthMax;
	}

	public float GetHealthMax() { return healthMax; }
	
	public float GetHealthCurrent() { return healthCurrent; }

	public void DoDamage(float damage) { healthCurrent = Mathf.Max(0, healthCurrent - damage); }
}
