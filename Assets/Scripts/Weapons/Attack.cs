using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	[SerializeField] bool melee = false;
	private ParticleSystem particles;
	private Entity owner;

	public void TakeOwnership(Entity owner)
	{
		this.owner = owner;

	}

	public void Activate()
	{
		if (particles != null)
			particles.Play();

		if (melee)
			ProjectileManager.Instance.GenerateProjectile(projectile, transform.position, transform.rotation, transform, owner);
		else
			ProjectileManager.Instance.GenerateProjectile(projectile, transform.position, transform.rotation, owner);
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
