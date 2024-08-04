using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public interface IAttackEffect
{
	void Activate(Entity owner);

	void Deactivate(Entity owner);
}

public class DashEffect : IAttackEffect
{
	float power;
	float slideTime;
	bool controlled = true;

	public DashEffect(float power, float slideTime, bool controlled)
	{
		this.power = power;
		this.slideTime = slideTime;
		this.controlled = controlled;
	}

	public void Activate(Entity owner)
	{
		owner.DashToAim(power, slideTime, fullStun: !controlled);
	}

	public void Deactivate(Entity owner)
	{
	}
}

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	[SerializeField] bool melee = false;
	[SerializeField] Object[] effectData = new Object[0];
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
			effect.Activate(owner);
		}
	}

	public void Deactivate()
	{
		if (particles != null)
			particles.Stop();

		foreach (IAttackEffect effect in effects)
		{
			effect.Activate(owner);
		}
	}

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
		List<IAttackEffect> newEffects = new();
		foreach (var effect in effectData)
		{
			if (effect != null && effect is IAttackEffectData)
			{
				newEffects.Add(((IAttackEffectData)effect).CreateEffect());
			}
		}
		effects = newEffects.ToArray();
	}
}
