using System;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityServiceLocator;
using Random = UnityEngine.Random;

[Serializable]
public class VoiceLineGroup
{
	[SerializeField] EventReference[] eventReferences;
	[SerializeField] int[] weights;

	public void PlayLine()
	{

		var countdown = Random.Range(0, GetTotal(weights));
		int i = 0;
		do
		{
			countdown -= weights[i++];

		}
		while (countdown > 0);
		AudioManager.Instance.PlayOneShot(eventReferences[i - 1], Vector3.zero);


	}

	private int GetTotal(int[] array)
	{
		int total = 0;
		for (int i = 0; i < array.Length; i++)
		{
			total += array[i];

		}
		return total;
	}
}
public class VoiceController : MonoBehaviour, IEntityObserver
{
	public void SetPrimaryPlayer(EntityMediator targetPlayer) => targetPlayer.AddObserver(this);
	[SerializeField] VoiceLineGroup idleLines;
	[SerializeField] VoiceLineGroup hurtLines;
	[SerializeField] VoiceLineGroup lowHealthEnterLines;
	[SerializeField] VoiceLineGroup lowHealthExitLines;
	[SerializeField] VoiceLineGroup healFullLines;
	[SerializeField] VoiceLineGroup healCollectLines;
	[SerializeField] VoiceLineGroup magicCollectLines;
	[SerializeField] VoiceLineGroup magicFullLines;
	[SerializeField] VoiceLineGroup magicEmptyLines;
	[SerializeField] VoiceLineGroup magicCastGoodLines;
	[SerializeField] VoiceLineGroup magicCastBadLines;
	[SerializeField] VoiceLineGroup comboStageOneLines;
	[SerializeField] VoiceLineGroup comboStageTwoLines;
	[SerializeField] VoiceLineGroup comboStageThreeLines;
	[SerializeField] VoiceLineGroup spinAttackLines;
	[SerializeField] VoiceLineGroup chargingSwingLines;
	[SerializeField] VoiceLineGroup dashLines;
	[SerializeField] VoiceLineGroup dashBadLines;
	[SerializeField] VoiceLineGroup upgradeCollectLines;
	[SerializeField] VoiceLineGroup winLines;
	[SerializeField] VoiceLineGroup deathLines;
	[SerializeField] VoiceLineGroup retryLines;
	[SerializeField] VoiceLineGroup nextWave;
	[SerializeField] VoiceLineGroup spawnLines;
	[SerializeField] VoiceLineGroup bossBullLines;
	[SerializeField] VoiceLineGroup bossGhostLines;
	[SerializeField] VoiceLineGroup bossFinalLines;
	[SerializeField] VoiceLineGroup tutorialSwordLines;
	[SerializeField] VoiceLineGroup tutorialMagicLines;
	[SerializeField] VoiceLineGroup tutorialDashLines;
	[SerializeField] VoiceLineGroup tutorialPickupLines;

	[SerializeField] float lowHealthPercent = 0.33f;



	// Start is called before the first frame update
	void Awake()
	{
		ServiceLocator.ForSceneOf(this).Register(this);
	}

	public void OnNotify(EntityData data)
	{
		switch (data.Prompt)
		{
			case VoicePrompt.SpawningIn:
				spawnLines.PlayLine();
				break;
			case VoicePrompt.Hurt:
				if (IsLowHealth(data.CurrentHealth / data.MaxHealth))
					lowHealthEnterLines.PlayLine();
				else
					hurtLines.PlayLine();
				break;
			case VoicePrompt.HealingPickup:
				if (IsHealthFull(data.CurrentHealth / data.MaxHealth))
					healFullLines.PlayLine();
				else if (!IsLowHealth(data.CurrentHealth / data.MaxHealth))
					lowHealthExitLines.PlayLine();
				else
					healCollectLines.PlayLine();
				break;
			case VoicePrompt.MagicPickup:
				if (IsMagicFull(data.CurrentMana / data.MaxMana))
					magicFullLines.PlayLine();
				else
					magicCollectLines.PlayLine();
				break;
			case VoicePrompt.MagicCast:
				if (false) // failed due to lacking magic
					magicEmptyLines.PlayLine();
				else if (false) // off beat
					magicCastBadLines.PlayLine();
				else
					magicCastGoodLines.PlayLine();
				break;
			case VoicePrompt.Lose:
				hurtLines.PlayLine();
				break;
		}
	}

	private bool IsLowHealth(float healthPercentage)
	{
		return healthPercentage <= lowHealthPercent;
	}

	private bool IsHealthFull(float healthPercentage)
	{
		return healthPercentage >= 1f;
	}

	private bool IsMagicFull(float magicPercentage)
	{
		return magicPercentage >= 1f;
	}


}
