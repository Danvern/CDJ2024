using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	private ParticleSystem particles;

    public void Activate()
	{
		if (particles != null)
			particles.Play();
	}

    public void Deactivate()
	{
		if (particles != null)
			particles.Stop();
	}

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}
}
