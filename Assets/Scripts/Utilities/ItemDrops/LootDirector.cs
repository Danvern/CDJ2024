using System;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityServiceLocator;

public class LootDirector : MonoBehaviour
{
	public static LootDirector Instance;
	[field: SerializeField] LootDirectorLogic logic = new LootDirectorLogic();

	void Start()
	{
		if (Instance == null)
			Instance = this;
		else
			Debug.LogWarning("More than one loot manager detected!!");

		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{

			EntityMediator playerMediator = ServiceLocator.For(player.GetComponent<Entity>()).Get<EntityMediator>();
			if (playerMediator != null)
			{
				logic.TrackHealth(playerMediator);
				logic.TrackAmmunition(playerMediator);
			}
		}
	}

	public float ModifiedDropRateHealth(float itemDropRate) => logic.ModifiedDropRateHealth(itemDropRate);
	public float ModifiedDropRateAmmunition(float itemDropRate, AmmoType type) => logic.ModifiedDropRateAmmunition(itemDropRate, type);
}

[Serializable]
public class LootDirectorLogic
{
	IHealth playerHealth;
	IAmmunitionSource playerAmmo;
	[field: SerializeField] float lifeMinimumChance = .05f;
	[field: SerializeField] float lifeMaximumChance = .02f;
	[field: SerializeField] AnimationCurve lifeDropCurve = AnimationCurve.Linear(0, 0, 1, 1);
	[field: SerializeField] float ammoMinimumChance = .05f;
	[field: SerializeField] float ammoMaximumChance = .02f;
	[field: SerializeField] AnimationCurve ammoDropCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public void SetLifeDropCurve(AnimationCurve value) => lifeDropCurve = value;

	public AnimationCurve GetLifeDropCurve() => lifeDropCurve;

	public void SetLifeMinimumChance(float value)
	{
		if (value < 0)
			lifeMinimumChance = 0f;
		else if (value > 1)
			lifeMinimumChance = 1f;
		else
			lifeMinimumChance = value;
	}

	public float GetLifeMinimumChance() => lifeMinimumChance;

	public void SetLifeMaximumChance(float value)
	{
		if (value < 0)
			lifeMaximumChance = 0f;
		else if (value > 1)
			lifeMaximumChance = 1f;
		else
			lifeMaximumChance = value;
	}

	public float GetLifeMaximumChance() => lifeMaximumChance;

	public void SetAmmoDropCurve(AnimationCurve value) => ammoDropCurve = value;

	public AnimationCurve GetAmmoDropCurve() => ammoDropCurve;

	public void SetAmmoMinimumChance(float value)
	{
		if (value < 0)
			ammoMinimumChance = 0f;
		else if (value > 1)
			ammoMinimumChance = 1f;
		else
			ammoMinimumChance = value;
	}

	public float GetAmmoMinimumChance() => ammoMinimumChance;

	public void SetAmmoMaximumChance(float value)
	{
		if (value < 0)
			ammoMaximumChance = 0f;
		else if (value > 1)
			ammoMaximumChance = 1f;
		else
			ammoMaximumChance = value;
	}

	public float GetAmmoMaximumChance() => ammoMaximumChance;

	public void TrackHealth(IHealth health)
	{
		if (health != null)
			playerHealth = health;
	}

	public void TrackAmmunition(IAmmunitionSource ammunition)
	{
		if (ammunition != null)
			playerAmmo = ammunition;
	}

	public float PlayerTeamHealthPercentage() => playerHealth.GetHealth() / playerHealth.GetHealthMax();

	public float PlayerTeamAmmunitionPercentage(AmmoType type) => playerAmmo.GetAmmo(type) > 0 ? (float)playerAmmo.GetAmmo(type) / playerAmmo.GetAmmoMax(type) : 0;

	public float ModifiedDropRateHealth(float itemDropRate)
	{
		if (playerHealth == null)
			return 0f;
		if (lifeDropCurve == null)
			return 0f;

		float modifier = Mathf.Lerp(GetLifeMinimumChance(), GetLifeMaximumChance(), lifeDropCurve.Evaluate(PlayerTeamHealthPercentage()));
		return Mathf.Clamp(modifier * itemDropRate, 0, 1);
	}

	public float ModifiedDropRateAmmunition(float itemDropRate, AmmoType type)
	{
		if (playerAmmo == null)
			return 0f;
		if (ammoDropCurve == null)
			return 0f;

		float modifier = Mathf.Lerp(GetAmmoMinimumChance(), GetAmmoMaximumChance(), ammoDropCurve.Evaluate(PlayerTeamAmmunitionPercentage(type)));
		return Mathf.Clamp(modifier * itemDropRate, 0, 1);
	}
}
