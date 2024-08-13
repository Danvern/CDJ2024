using System;
using System.Collections.Concurrent;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoReward", menuName = "Pickups/Ammo")]
public class AmmoRewardStats : GenericPickup
{
	[field: SerializeField] AmmoBundle ammunition;

	public override void Visit(EntityMediator visitable)
	{
		if (!visitable.IsUsingPickups())
			return;

		float multiplier = 1f;
		if (visitable.GetUpgradeStats() != null && visitable.GetAffinity() != null)
			ammunition.AddToInventory(visitable, 1f + visitable.GetUpgradeStats().GetAmmoDropBonus() * visitable.GetAffinity().GetAmmoDropValue(), 
					1f + visitable.GetUpgradeStats().GetAmmoReserveBonus() * visitable.GetAffinity().GetAmmoReserve());
		else
			ammunition.AddToInventory(visitable);
		Consume();
	}
}
