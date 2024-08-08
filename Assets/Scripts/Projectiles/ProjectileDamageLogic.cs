using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
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
	private Vector2 impactPosition = Vector2.zero;
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
	}

	public float GetLifetime() { return lifetime; }

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }

	public float GetCollisionRadius() { return collisionRadius; }

	public float GetCollisionArc() { return collisionArc; }

	public float GetKnockback() { return 50; }

	public float GetKnockbackDelay() { return 0.1f; }

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
			if (entity == null)
				continue;
			EntityMediator entityMediator = ServiceLocator.For(entity).Get<EntityMediator>();

			if (entityCollisions.ContainsKey(collider.transform))
			{

			}
			else if (entityMediator.IsDead()) {}
			else if (owner == null || owner.IsHostile(entityMediator))
			{
				entityCollisions.Add(collider.transform, Time.time);
				DecreasePierce(1);
				DoEntityEffect(entityMediator);
				if (!data.SmallKillSFX.IsNull && entityMediator.IsDead())
					AudioManager.Instance.PlayOneShot(data.SmallKillSFX, transform.position);
				else
					AudioManager.Instance.PlayOneShot(data.DamageSFX, transform.position);
				if (piercing < 0)
				{
					kill = true;
					break;
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
		Debug.Log(visitable.ToString());

	}

	private bool IsColliderInsideArc(Vector2 colliderPosition, Vector2 position, Vector2 forward, float arc)
	{
		return Vector2.Angle(forward, colliderPosition - position) <= arc;
	}

	public void Visit(Entity visitable) { }
}
