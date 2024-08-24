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
		while(countdown > 0);
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
	[SerializeField] VoiceLineGroup hurtLines;
	[SerializeField] VoiceLineGroup spawnLines;



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
				hurtLines.PlayLine();
				break;
			case VoicePrompt.HealingPickup:
				hurtLines.PlayLine();
				break;
			case VoicePrompt.MagicPickup:
				hurtLines.PlayLine();
				break;
			case VoicePrompt.Lose:
				hurtLines.PlayLine();
				break;
		}
	}


}
