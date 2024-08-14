using System;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
	Bullet,
	Explosive,
	Energy,
	Fuel,
}

[Serializable]
public class AmmoBundle
{
	public int bulletAmount = 0;
	public int explosiveAmount = 0;
	public int energyAmount = 0;
	public int fuelAmount = 0;

	public void AddToInventory(IAmmunitionSource inventory, float multiplier = 1f, float maxMultiplier = 1f)
	{
		inventory.AddAmmo(AmmoType.Bullet, (int)(bulletAmount * multiplier), maxMultiplier);
		inventory.AddAmmo(AmmoType.Explosive, (int)(explosiveAmount * multiplier), maxMultiplier);
		inventory.AddAmmo(AmmoType.Energy, (int)(energyAmount * multiplier), maxMultiplier);
		inventory.AddAmmo(AmmoType.Fuel, (int)(fuelAmount * multiplier), maxMultiplier);
	}
}

[Serializable]
public class AmmoParameters
{
	public Vector2Int bulletAmount = new(0, 0);
	public Vector2Int explosiveAmount = new(0, 0);
	public Vector2Int energyAmount = new(0, 0);
	public Vector2Int fuelAmount = new(0, 0);

	public void AddToInventory(AmmoInventory inventory)
	{
		inventory.SetAmmoMax(AmmoType.Bullet, bulletAmount.y);
		inventory.SetAmmoMax(AmmoType.Explosive, explosiveAmount.y);
		inventory.SetAmmoMax(AmmoType.Energy, energyAmount.y);
		inventory.SetAmmoMax(AmmoType.Fuel, fuelAmount.y);
		inventory.AddAmmo(AmmoType.Bullet, bulletAmount.x);
		inventory.AddAmmo(AmmoType.Explosive, explosiveAmount.x);
		inventory.AddAmmo(AmmoType.Energy, energyAmount.x);
		inventory.AddAmmo(AmmoType.Fuel, fuelAmount.x);
	}
}

public class AmmoInventory : IAmmunitionSource
{
	private class AmmoPair
	{
		public int Current;
		public int Max = 64;

		public AmmoPair(int current)
		{
			Current = current;
		}
		
		public AmmoPair(int current, int max)
		{
			Current = current;
			Max = max;
		}
	}

	Dictionary<AmmoType, AmmoPair> ammoReserve = new();

	public void AddAmmo(AmmoType type, int amount, float maxMultiplier = 1f)
	{
		if (ammoReserve.ContainsKey(type))
			ammoReserve[type].Current = Mathf.Max(0, amount + ammoReserve[type].Current);
		else
			ammoReserve.Add(type, new(Mathf.Max(0, amount)));

		ClampAmmo(type, maxMultiplier); // TODO: somewhat liable to breaking method of automatically adjusting reserves. Fix later.
	}

	public void SetAmmo(AmmoType type, int amount)
	{
		if (ammoReserve.ContainsKey(type))
			ammoReserve[type].Current = Mathf.Max(0, amount);
		else
			ammoReserve.Add(type, new(Mathf.Max(0, amount)));

		ClampAmmo(type);
	}

	public int GetAmmo(AmmoType type)
	{
		if (ammoReserve.ContainsKey(type))
			return ammoReserve[type].Current;
		return 0;
	}

	public void SetAmmoMax(AmmoType type, int amount)
	{
		if (ammoReserve.ContainsKey(type))
			ammoReserve[type].Max = Mathf.Max(0, amount);
		else
			ammoReserve.Add(type, new(0, Mathf.Max(0, amount)));

		ClampAmmo(type);
	}

	public int GetAmmoMax(AmmoType type)
	{
		if (ammoReserve.ContainsKey(type))
			return ammoReserve[type].Max;
		return 0;
	}

	private void ClampAmmo(AmmoType type, float maxMultiplier = 1f)
	{
	if (ammoReserve[type].Current > ammoReserve[type].Max * maxMultiplier)
			ammoReserve[type].Current = (int)(ammoReserve[type].Max * maxMultiplier);

	}
}