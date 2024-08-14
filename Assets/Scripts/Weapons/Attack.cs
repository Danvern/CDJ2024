using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public interface IAttackEffect
{
	void Activate(EntityMediator owner);

	void Deactivate(EntityMediator owner);
}

public class Attack : MonoBehaviour
{
	[SerializeField] GameObject projectile;
	[SerializeField] bool melee = false;
	[SerializeField] Object[] effectData = new Object[0];
	[SerializeField] private EventReference attackSFX;
	IAttackEffect[] effects = new IAttackEffect[0];
	private ParticleSystem particles;
	private EntityMediator owner;

	public void TakeOwnership(EntityMediator owner)
	{
		this.owner = owner;

	}

	public void Activate()
	{
		if (particles != null)
			particles.Play();
		if (!attackSFX.IsNull)
			AudioManager.Instance.PlayOneShot(attackSFX, transform.position);

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
