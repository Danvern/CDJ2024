using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	[SerializeField] bool melee = false;
	private ParticleSystem particles;

	public void Activate()
	{
		if (particles != null)
			particles.Play();

		if (melee)
			ProjectileManager.Instance.GenerateProjectile(projectile, transform.position, transform.rotation, transform);
		else
			ProjectileManager.Instance.GenerateProjectile(projectile, transform.position, transform.rotation);
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
