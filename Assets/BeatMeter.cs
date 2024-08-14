
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BeatMeter : MonoBehaviour
{
	Animator animator;
	float lastPing;
	bool reverse;
	int beat = 0;
	// Start is called before the first frame update
	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		animator.Play("Base Layer.BeatFlower");
	}

	// Update is called once per frame
	void LateUpdate()
	{
		float timing = AudioManager.Instance.GetMusicPing();
		float frameTime;
		if (!reverse && lastPing > timing)
			beat = ++beat % 4;
		if (lastPing < timing)
			reverse = false;
		else if (lastPing > timing)
			reverse = true;

		frameTime = beat / 4f;
		animator.SetFloat("PlaybackTime", frameTime);
		lastPing = timing;
	}

	void UpdatePerPing()
	{
		float timing = AudioManager.Instance.GetMusicPing();
		float frameTime;
		if (lastPing < timing)
			reverse = false;
		else if (lastPing > timing)
			reverse = true;
		if (!reverse)
			frameTime = 0.125f + timing * 0.625f;
		else
			frameTime = (1.25f - timing * 0.625f) % 1f;
		animator.SetFloat("PlaybackTime", frameTime);
		//Debug.Log(frameTime + " vs " + timing);
		//animator.StopPlayback();
		lastPing = timing;

	}
}
