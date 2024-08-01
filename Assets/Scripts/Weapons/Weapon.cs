using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	private Attack[] attacks;

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

	void Start()
	{
		attacks = GetComponentsInChildren<Attack>();
	}
}
