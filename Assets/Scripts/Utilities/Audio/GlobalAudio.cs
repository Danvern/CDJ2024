using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class GlobalAudio : MonoBehaviour
{
	[field: Header("Music")]
	[field: SerializeField] public EventReference Music { get; private set; }

	public static GlobalAudio Instance { get; private set; }
	private EventInstance musicLoop;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Found more than one FModEvents in the scene.");
		}
		Instance = this;
	}

	private void Start()
	{
		if (!Music.IsNull)
		{
			musicLoop = AudioManager.Instance.CreateInstance(Instance.Music);
			musicLoop.start();
		}
	}

	public void PlayOneShot(EventReference sound, Vector3 origin)
	{
		RuntimeManager.PlayOneShot(sound, origin);
	}
}
