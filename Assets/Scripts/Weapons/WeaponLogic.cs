using UnityEngine;

public class WeaponLogic : IWeaponLogic
{
	WeaponData data;
	float lastAttackTime;
	int activeAttack = 0;

	public WeaponLogic(WeaponData data)
	{
		this.data = data;
		if (data.AttackDefinitions != null && data.AttackDefinitions.Length > 0)
			lastAttackTime = -data.AttackDefinitions[activeAttack].GetCooldown();
		else
			Debug.LogWarning("Missing weapon attack definitions for weapon " + data.ToString() + ". Weapon will likely not behave as intended.");
	}

	public void SetTrackedAttack(int index)
	{
		if (data.AttackDefinitions.Length <= index)
			return;
		activeAttack = index;
	}

	public bool IsAttackReady() { return GetCooldown() == 0; }

	public float GetCooldown()
	{
		if (data.AttackDefinitions != null && data.AttackDefinitions.Length > 0)
			return Mathf.Max(0, data.AttackDefinitions[activeAttack].GetCooldown() - (Time.time - lastAttackTime));
		else
			return 0;
	}

	public void ResetCooldown() { lastAttackTime = Time.time; }
	public float GetLastAttackTime() { return lastAttackTime; }
}
