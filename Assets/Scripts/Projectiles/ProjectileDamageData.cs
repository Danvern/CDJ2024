using UnityEngine;

public class ProjectileDamageData : IProjectileDamageLogic
{
    float damageMax = 0;
    float damageMin = 1;
	int piercing = 0;

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public bool DoesPierce(int resistance) { return piercing > resistance;}

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance);} // May not want to single instance this.
}
