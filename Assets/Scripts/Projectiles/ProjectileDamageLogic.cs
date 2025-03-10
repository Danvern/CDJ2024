using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class ProjectileDamageLogic : IProjectileDamageLogic
{
	private float lifetime = 3;
	private float damageMax = 0;
	private float damageMin = 1;
	private int piercing = 0;
	private float collisionRadius = 3;
	private float collisionArc = 0;
	private float speed = 1;
	private float knockback = 10;
	private float KnockbackStun = .25f;
	private float hitLag = 0f;
	private Vector2 impactPosition = Vector2.zero;
	private bool isIndescriminate = false;
	private bool isExplosion = false;
	private bool isProjectileDestroyer = false;
	private ProjectileDamageData data;
	private ProjectileBase mediator;

	private Dictionary<Transform, float> entityCollisions = new Dictionary<Transform, float>();

	public ProjectileDamageLogic(ProjectileDamageData data, ProjectileBase mediator)
	{
		this.data = data;
		lifetime = data.Lifetime;
		damageMax = data.DamageMax;
		damageMin = data.DamageMin;
		piercing = data.Piercing;
		collisionRadius = data.CollisionRadius;
		collisionArc = data.CollisionArc;
		speed = data.StartingVelocity;
		knockback = data.Knockback;
		KnockbackStun = data.KnockbackStun;
		isIndescriminate = data.IsIndescriminate;
		isExplosion = data.IsExplosion;
		isProjectileDestroyer = data.IsProjectileDestroyer;
		hitLag = data.HitLag;
		this.mediator = mediator;
	}

	public float GetLifetime() { return lifetime; }

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }

	public float GetCollisionRadius() { return collisionRadius; }

	public float GetCollisionArc() { return collisionArc; }

	public float GetKnockback() { return knockback; }

	public float GetKnockbackDelay() { return KnockbackStun; }
	public float GetSpeed() { return speed; }
	public bool IsIndescriminate() { return isIndescriminate; }
	public bool IsExplosion() { return isExplosion; }
	public void DoDeathEffect(Transform transform, EntityMediator owner)
	{
		if (GetDeathEffect())
			ProjectileManager.Instance.GenerateProjectile(GetDeathEffect(), transform.position, GetDeathEffect().transform.rotation, owner);

	}

	public GameObject GetHitEffect() { return data.HitEffect; }
	public GameObject GetDeathEffect() { return data.DeathEffect; }

	public bool CheckCollisons(Transform transform, EntityMediator owner)
	{
		bool kill = false;
		if (GetCollisionRadius() == 0) return false;

		LayerMask entityMask = LayerMask.GetMask("Entities");
		LayerMask environmentMask = LayerMask.GetMask("EnvironmentObstacles");
		LayerMask projectileMask = LayerMask.GetMask("ProjectileVulnerable");
		Collider2D[] potentialCollisions = Physics2D.OverlapCircleAll(transform.position, GetCollisionRadius(), layerMask: entityMask | projectileMask);

		if (potentialCollisions.Length == 0) return false;

		impactPosition = transform.position;
		foreach (Collider2D collider in potentialCollisions)
		{
			if (GetCollisionArc() != 0 && !IsColliderInsideArc(collider.transform.position, transform.position, transform.up, GetCollisionArc()))
				continue;
			Entity entity = collider.transform.GetComponent<Entity>();
			ProjectileBase projectile = collider.transform.GetComponent<ProjectileBase>();
			if (projectile != null)
			{
				if (projectile.CanCollideWith(owner) && isProjectileDestroyer)
				{
					projectile.KillNext();

				}
			}
			else if (entity != null)
			{
				kill = CheckEntityCollision(transform, owner, collider, entity) || kill;
			}
			potentialCollisions = Physics2D.OverlapCircleAll(transform.position, GetCollisionRadius(), layerMask: environmentMask);
			if (data.IsBlockedByWorld && potentialCollisions.Length > 0)
				kill = true;
		}
		return kill;
	}

	private bool CheckEntityCollision(Transform transform, EntityMediator owner, Collider2D collider, Entity entity)
	{
		EntityMediator entityMediator = ServiceLocator.For(entity).Get<EntityMediator>();

		if (entityCollisions.ContainsKey(collider.transform))
		{

		}
		else if (entityMediator.IsDead()) { }
		else if (owner == null || owner.IsHostile(entityMediator) || (IsIndescriminate() && (owner != entityMediator || IsExplosion())))
		{
			if (hitLag > 0)
			{

				ProjectileManager.Instance.DoHitLag(hitLag);
				AudioManager.Instance.PlayOneShot(data.DamageSFX, transform.position);
			} //TODO: clean up here

			entityCollisions.Add(collider.transform, Time.time);
			DecreasePierce(1);
			DoEntityEffect(entityMediator);
			if (GetHitEffect() && (piercing >= 0 || !data.IsHitEffectOnlyOnPierce))
				ProjectileManager.Instance.GenerateProjectile(GetHitEffect(), impactPosition, GetHitEffect().transform.rotation, owner);
			//ProjectileManager.Instance.GenerateProjectile(GetHitEffect(), collider.ClosestPoint(impactPosition), transform.rotation, owner);
			if (hitLag > 0)

				if (entityMediator.IsDead())
				{
					if (!data.HeavyKillSFX.IsNull && entityMediator.IsHeavy())
						AudioManager.Instance.PlayOneShot(data.HeavyKillSFX, transform.position);
					else if (!data.SmallKillSFX.IsNull)
						AudioManager.Instance.PlayOneShot(data.SmallKillSFX, transform.position);
				}
				else
				{
					if (!data.HeavyDamageSFX.IsNull && entityMediator.IsHeavy())
						AudioManager.Instance.PlayOneShot(data.HeavyDamageSFX, transform.position);
					else if (!data.DamageSFX.IsNull)
						AudioManager.Instance.PlayOneShot(data.DamageSFX, transform.position);
				}
			if (piercing < 0 && !IsExplosion())
			{
				return true;
			}
		}
		return false;
	}

	public void DoEntityEffect(EntityMediator entity)
	{
		entity.Accept(this);
	}

	public void Visit(IMovementLogic visitable)
	{
		visitable.KnockbackStun(GetKnockback(), GetKnockbackDelay(), (visitable.GetRigidbody().position - impactPosition).normalized);
	}

	public void Visit(EntityHealthLogic visitable)
	{
		visitable.DoDamage(GetDamageRandom(), mediator);
		//Debug.Log(visitable.ToString());

	}
	public void Visit(EntityMediator visitable)
	{
		//Debug.Log(visitable.ToString());

	}

	private bool IsColliderInsideArc(Vector2 colliderPosition, Vector2 position, Vector2 forward, float arc)
	{
		return Vector2.Angle(forward, colliderPosition - position) <= arc;
	}

	public void Visit(Entity visitable) { }
}
