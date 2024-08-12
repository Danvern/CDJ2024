
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BeatMeter : MonoBehaviour
{
	Animator animator;
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
		animator.Play("Base Layer.BeatFlower", 0, timing);
    }
}
