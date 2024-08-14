using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
	[field: SerializeField] RandomDropTable[] drops;
	public bool IsInstant = false;
	public int MaxPickups = 1;
	public float SpreadDistance = 2.5f;
	[field: SerializeField] float generationDelay = 0.5f;
	private OwlCountdown countdown;
	List<GameObject> dropInstances;


	// Start is called before the first frame update
	void Start()
	{
		if (IsInstant)
		{
			countdown = new OwlCountdown(generationDelay);
			countdown.Expired += () => DropReward(null);
		}
	}

	void Update()
	{
		if (countdown != null)
			countdown.Update(Time.deltaTime);
	}

	void OnRewardPicked()
	{
		if (!gameObject)
			return;

		MaxPickups--;
		if (MaxPickups <= 0)
		{
			foreach (var drop in dropInstances)
				Destroy(drop);
			Destroy(gameObject);
		}
	}

	public void DropReward(IOwnedEntity projectile)
	{
		if (LootDirector.Instance == null)
		{
			Debug.LogWarning("No valid loot director found!");
			return;
		}

		dropInstances = new();
		foreach (RandomDropTable drop in drops)
		{
			if (drop == null)
			{
				Debug.LogWarning("Empty slot in drop controller!");
				continue;
			}
			if (projectile == null)
				drop.DropReward(transform, generatedRewards: dropInstances);
			else
				drop.DropReward(transform, projectile.GetOwner(), generatedRewards: dropInstances);
		}

		if (dropInstances.Count > 0)
		{
			float spreadAngle = 360 / dropInstances.Count;
			int index = 0;
			foreach (GameObject drop in dropInstances)
			{
				PickupReward reward = drop.GetComponent<PickupReward>();

				//var weapon = drop.GetComponent<WeaponChest>();
				if (GetComponent<Entity>() == null)
				{
					if (reward)
						reward.OnPickupUsed += OnRewardPicked;
					// else if (weapon)
					// 	weapon.OnPickupUsed += OnRewardPicked;
				}

				drop.transform.position = OwlVector.PositionAroundPivot(transform.position, drop.transform.rotation, SpreadDistance, spreadAngle * index);
				drop.transform.rotation = Quaternion.identity;
				//drop.transform.rotation = drop.transform.rotation * Quaternion.Euler(0, spreadAngle * index, 0);
				index++;
			}
		}
	}
}
