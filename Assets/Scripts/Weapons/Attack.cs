using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackEffect
{
	void Activate();

	void Deactivate();
}

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	[SerializeField] bool melee = false;
	IAttackEffect[] effects = new IAttackEffect[0];
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

		foreach (IAttackEffect effect in effects)
		{
			effect.Activate();
		}
	}

	public void Deactivate()
	{
		if (particles != null)
			particles.Stop();

		foreach (IAttackEffect effect in effects)
		{
			effect.Activate();
		}
	}

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}
}
