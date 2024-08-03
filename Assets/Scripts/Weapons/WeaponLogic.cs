using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : IWeaponLogic
{
	WeaponData data;
	float lastAttack;
	int activeAttack = 0;

	public WeaponLogic(WeaponData data)
	{
		this.data = data;
		lastAttack = -data.AttackDefinitions[activeAttack].GetCooldown();
	}

	public void SetTrackedAttack(int index)
	{
		if (data.AttackDefinitions.Length <= index)
			return;
		activeAttack = index;
	}

	public bool IsAttackReady() { return GetCooldown() == 0; }

	public float GetCooldown() { return Mathf.Max(0, data.AttackDefinitions[activeAttack].GetCooldown() - (Time.time - lastAttack)); }

	public void ResetCooldown() { lastAttack = Time.time; }
}
