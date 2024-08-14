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
	private Vector2 impactPosition = Vector2.zero;
	private bool isIndescriminate = false;
	private bool isExplosion = false;
	private bool isProjectileDestroyer = false;
	private ProjectileDamageData data;

	private Dictionary<Transform, float> entityCollisions = new Dictionary<Transform, float>();

	public ProjectileDamageLogic(ProjectileDamageData data)
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

	public GameObject GetHitEffect() { return data.HitEffect; }

	public bool CheckCollisons(Transform transform, EntityMediator owner)
	{
		bool kill = false;
		Collider2D[] potentialCollisions = Physics2D.OverlapCircleAll(transform.position, GetCollisionRadius());

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
				EntityMediator entityMediator = ServiceLocator.For(entity).Get<EntityMediator>();

				if (entityCollisions.ContainsKey(collider.transform))
				{

				}
				else if (entityMediator.IsDead()) { }
				else if (owner == null || owner.IsHostile(entityMediator) || (IsIndescriminate() && (owner != entityMediator || IsExplosion())))
				{
					entityCollisions.Add(collider.transform, Time.time);
					DecreasePierce(1);
					DoEntityEffect(entityMediator);
					if (GetHitEffect())
						ProjectileManager.Instance.GenerateProjectile(GetHitEffect(), collider.ClosestPoint(impactPosition), transform.rotation, owner);
					
					if (entityMediator.IsDead())
					{
						if (!data.SmallKillSFX.IsNull)
							AudioManager.Instance.PlayOneShot(data.SmallKillSFX, transform.position);
					}
					else
					{
						if (!data.DamageSFX.IsNull)
							AudioManager.Instance.PlayOneShot(data.DamageSFX, transform.position);
					}
					if (piercing < 0 && !IsExplosion())
					{
						kill = true;
						break;
					}
				}
			}
		}
		return kill;
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
		visitable.DoDamage(GetDamageRandom());
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
