using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	private Attack[] attacks = new Attack[0];
	private Entity owner;

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
		foreach (Attack attack in attacks)
		{
			attack.Activate();
		}
	}

	public void Deactivate()
	{
		foreach (Attack attack in attacks)
		{
			attack.Deactivate();
		}
	}

	void Awake()
	{
		attacks = GetComponentsInChildren<Attack>();
	}
}
