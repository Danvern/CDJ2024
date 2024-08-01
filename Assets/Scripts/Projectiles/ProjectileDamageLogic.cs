using UnityEngine;

public class ProjectileDamageLogic : IProjectileDamageLogic
{
	private float damageMax = 0;
	private float damageMin = 1;
	private int piercing = 0;

	public ProjectileDamageLogic(ProjectileDamageData data)
	{
		damageMax = data.DamageMax;
		damageMin = data.DamageMin;
		piercing = data.Piercing;
	}

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }
}
