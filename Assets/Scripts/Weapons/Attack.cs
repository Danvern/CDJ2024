using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	private ParticleSystem particles;

    public void Activate()
	{
		if (particles != null)
			particles.Play();

		Instantiate(projectile);
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
