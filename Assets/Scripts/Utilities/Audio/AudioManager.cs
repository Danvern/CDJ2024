using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[field: Header("Music")]
	[field: SerializeField] public EventReference Music { get; private set; }
	public static AudioManager Instance { get; private set; }
	private LinkedList<EventInstance> activeInstances = new();
	public float MusicVolume { get; set; }
	private EventInstance musicLoop;
	private PARAMETER_ID musicBeatID;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("!Found more than one AudioManager in the scene.");
		}
		Instance = this;
	}

	private void Start()
	{
		if (!Music.IsNull) PlayMusic(Music);
		musicLoop.getDescription(out EventDescription eventDescription);
		eventDescription.getParameterDescriptionByName("ping", out PARAMETER_DESCRIPTION parameterDescription);
		musicBeatID = parameterDescription.id;

		// SetCombatActive(true);
		// SetLowHealth(1);
		// SetPing(15);
	}

	public void PlayMusic(EventReference music)
	{
		musicLoop = CreateInstance(music);
		musicLoop.start();
	}

	public void SetBossActive(bool active) => RuntimeManager.StudioSystem.setParameterByName("boss", active ? 1 : 0);
	public void SetCombatActive(bool active) => RuntimeManager.StudioSystem.setParameterByName("combat", active ? 1 : 0);
	public void SetLowHealth(float percent) => RuntimeManager.StudioSystem.setParameterByName("lowhealth", percent);
	public void SetPing(float percent) => RuntimeManager.StudioSystem.setParameterByName("ping", percent);

	public float GetMusicPing()
	{
		// RuntimeManager.StudioSystem.getParameterByName("ping", out ping);
		

		Debug.Log(musicLoop.getParameterByID(musicBeatID, out float pingInitial, out float ping) == FMOD.RESULT.OK ? "success" : "failure");
		//RuntimeManager.CoreSystem.getMasterChannelGroup(out FMOD.ChannelGroup masterCG);
		//masterCG.getGroup()
		return ping;
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
		// RuntimeManager.StudioSystem.getParameterByName("ping", out ping);
		//RuntimeManager.StudioSystem.getParameterByName("testparam", out float ping);

		// Debug.Log("Test: " + ping);
		Debug.Log("Ping: " + GetMusicPing());
		// musicLoop.getTimelinePosition(out int timelinePosition);
   		// Debug.Log("Timeline Position: " + timelinePosition);
	}

	private void OnDestroy()
	{
		Cleanup();
	}
}
