using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	private ParticleSystem particles;

    public void Activate()
	{
		particles.Play();
	}

    public void Deactivate()
	{
		particles.Stop();
		Debug.Log("AttackStopped");
	}

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}
}
