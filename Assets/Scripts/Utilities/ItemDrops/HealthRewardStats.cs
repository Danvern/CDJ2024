using UnityEngine;

[CreateAssetMenu(fileName = "HealthReward", menuName = "Pickups/Health")]
public class HealthRewardStats : GenericPickup
{
	[field: SerializeField] float health;
	[field: SerializeField] float shield;

	public override void Visit(EntityMediator visitable)
	{
		if (!visitable.IsUsingPickups())
			return;

		float multiplier = 1f;
		if (visitable.GetUpgradeStats() != null && visitable.GetAffinity() != null)
			multiplier += visitable.GetUpgradeStats().GetHealthDropBonus() * visitable.GetAffinity().GetHealthDropValue();
		visitable.AddHealth(health * multiplier);
		visitable.AddShield(shield * multiplier);
		Consume();
	}
}
