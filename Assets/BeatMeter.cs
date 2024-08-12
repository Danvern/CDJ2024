
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BeatMeter : MonoBehaviour
{
	Animator animator;
	float lastPing;
	// Start is called before the first frame update
	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
	}

	// Update is called once per frame
	void LateUpdate()
	{
		float timing = AudioManager.Instance.GetMusicPing();
		if (lastPing < timing)
			animator.Play("Base Layer.BeatFlower", 0, 0.125f + timing / 0.625f);
		else
			animator.Play("Base Layer.BeatFlower", 0, (1.25f - timing / 0.625f) % 1f);
		lastPing = timing;
	}
}
