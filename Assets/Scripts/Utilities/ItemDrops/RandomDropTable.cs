using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RandomDropTable", menuName = "Pickups/RandomDropTable")]
public class RandomDropTable : ScriptableObject
{
	[Serializable]
	private class Drop
	{
		[field: SerializeField] public GameObject Reward { get; set; }
		[field: SerializeField] public DropScaling Scaling { get; set; } = DropScaling.Health;
		[field: SerializeField] public AmmoType Category { get; set; } = AmmoType.Magic;
		[field: SerializeField][field: Range(0f, 20f)] public float Chance { get; set; } = 1f;

		public GameObject Generate(Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject reward = Instantiate(Reward, position, rotation);
			reward.transform.parent = parent;
			return reward;
		}

		public float GetModifiedChance()
		{
			if (Scaling == DropScaling.Health)
				return LootDirector.Instance.ModifiedDropRateHealth(Chance);
			else if (Scaling == DropScaling.Ammunition)
				return LootDirector.Instance.ModifiedDropRateAmmunition(Chance, Category);
			else
				return Chance;
		}

		public float GetEntityLootModifier(ILootMediator killer)
		{
			if (killer == null || killer.GetUpgradeStats() == null || killer.GetAffinity() == null)
				return 1f;
			if (Scaling == DropScaling.Health)
				return 1f + killer.GetUpgradeStats().GetHealthDropBonus() * killer.GetAffinity().GetHealthDropRate();
			else if (Scaling == DropScaling.Ammunition)
				return 1f + killer.GetUpgradeStats().GetAmmoDropBonus() * killer.GetAffinity().GetAmmoDropRate();
			return 1f;
		}
	}

	public enum DropScaling
	{
		None,
		Health,
		Ammunition,
	}


	[field: SerializeField] Drop[] drops;
	public bool IsWeightedPick = false;
	public int MaxPicks = 1;

	public void DropReward(Transform transform, ILootMediator killer = null, List<GameObject> generatedRewards = null)
	{
		if (LootDirector.Instance == null)
		{
			Debug.LogWarning("No valid loot director found!");
			return;
		}
		for (int i = 0; i < MaxPicks; i++)
		{

			float totalWeight = 0f;
			if (IsWeightedPick)
			{
				foreach (Drop drop in drops)
				{
					if (drop != null && drop.Reward != null && drop.Chance > 0)
						totalWeight += drop.GetModifiedChance() * drop.GetEntityLootModifier(killer);
				}
				totalWeight *= Random.value;
				float epsilon = totalWeight / 100f;
				foreach (Drop drop in drops)
				{
					if (drop == null || drop.Reward == null || drop.Chance <= 0)
						continue;
					if ((totalWeight -= drop.GetModifiedChance() * drop.GetEntityLootModifier(killer)) < 0 + epsilon)
					{
						GameObject generated = drop.Generate(transform.position, transform.rotation, transform.parent);
						if (generatedRewards != null)
							generatedRewards.Add(generated);
						break;
					}
				}
			}
			else
			{
				foreach (Drop drop in drops)
				{
					float totalChance = drop.GetModifiedChance() * drop.GetEntityLootModifier(killer);
					// Debug.Log($"Chance - {totalChance}");
					if (drop.Reward && drop.Chance > 0f && Random.value <= totalChance)
					{
						GameObject generated = drop.Generate(transform.position, transform.rotation, transform.parent);
						if (generatedRewards != null)
							generatedRewards.Add(generated);
					}
				}
			}
		}
	}
}