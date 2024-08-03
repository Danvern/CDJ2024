using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField] private WeaponData data;
	private Attack[] attacks = new Attack[0];
	private Entity owner;
	private IWeaponLogic logic;
	private StateMachine stateMachine;

	public void TakeOwnership(Entity owner)
	{
		this.owner = owner;
		foreach (Attack attack in attacks)
		{
			attack.TakeOwnership(owner);
		}
	}

	public void Activate()
	{
		if (logic == null) return;
		if (!logic.IsAttackReady()) return;

		foreach (Attack attack in attacks)
		{
			attack.Activate();
		}
		logic.ResetCooldown();
	}

	public void Deactivate()
	{
		if (logic == null) return;

		foreach (Attack attack in attacks)
		{
			attack.Deactivate();
		}
	}

	public void TriggerAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Activate();
	}

	public void DeactivateAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Deactivate();
	}

	void Awake()
	{
		attacks = GetComponentsInChildren<Attack>();
		logic = new WeaponLogic(data);

		stateMachine = new StateMachine();


	}
}
