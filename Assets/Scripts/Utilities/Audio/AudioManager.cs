using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }
	private LinkedList<EventInstance> activeInstances = new();
	public float MusicVolume { get; set; }


	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Found more than one AudioManager in the scene.");
		}
		Instance = this;
	}

	public void PlayOneShot(EventReference sound, Vector3 origin)
	{
		RuntimeManager.PlayOneShot(sound, origin);
	}

	public EventInstance CreateInstance(EventReference sound)
	{
		EventInstance instance = RuntimeManager.CreateInstance(sound);
		activeInstances.AddLast(instance);
		return instance;
	}

	private void Cleanup()
	{
		foreach (EventInstance fInstance in activeInstances)
		{
			fInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			fInstance.release();
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		Cleanup();
	}
}
