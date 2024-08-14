using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackDefinition : IComboDefinition
{
	[SerializeField] private int id = 1;
	[SerializeField] private float cooldown = 1;
	[SerializeField] private float idealTiming = 1;
	[SerializeField] private float idealTimingWindow = 1;
	[SerializeField] private bool chargeAttack = false;

	public float GetCooldown() { return cooldown; }
	public float GetIdealTiming() { return idealTiming; }
	public float GetIdealTimingWindow() { return idealTimingWindow; }
	public int GetIndex() { return id; }
	public bool IsCharge() { return chargeAttack; }
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameplayDefinitions/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
	public AttackDefinition[] AttackDefinitions = new AttackDefinition[0];
	public int MaxCombo = 0;

	public AttackDefinition[] GetComboDefinitions()
	{
		List<AttackDefinition> attacks = new();
		foreach (AttackDefinition attack in AttackDefinitions)
		{
			if (!attack.IsCharge())
				attacks.Add(attack);
		}
		return attacks.ToArray();
	}
	public AttackDefinition[] GetChargeDefinitions()
	{
		List<AttackDefinition> attacks = new();
		foreach (AttackDefinition attack in AttackDefinitions)
		{
			if (attack.IsCharge())
				attacks.Add(attack);
		}
		return attacks.ToArray();
	}
}
