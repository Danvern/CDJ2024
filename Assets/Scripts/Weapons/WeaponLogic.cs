using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : IWeaponLogic
{
	WeaponData data;
	float lastAttack;

	public WeaponLogic(WeaponData data)
	{
		this.data = data;
		lastAttack = -data.Cooldown;
	}

	public bool IsAttackReady() { return GetCooldown() == 0;}

	public float GetCooldown() { return Mathf.Max(0, data.Cooldown - (Time.time - lastAttack));}

	public void ResetCooldown() { lastAttack = Time.time;}
}
