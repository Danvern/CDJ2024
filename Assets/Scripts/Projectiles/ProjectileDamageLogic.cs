using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;

public class ProjectileDamageLogic : IProjectileDamageLogic
{
	private float lifetime = 3;
	private float damageMax = 0;
	private float damageMin = 1;
	private int piercing = 0;
	private float collisionRadius = 3;
	private float collisionArc = 0;
	private Vector3 impactPosition = Vector3.zero;
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

	public bool CheckCollisons(Transform transform, Entity owner)
	{
		bool kill = false;
		Collider[] potentialCollisions = Physics.OverlapSphere(transform.position, GetCollisionRadius());

		if (potentialCollisions.Length == 0) return false;

		impactPosition = transform.position;
		foreach (Collider collider in potentialCollisions)
		{
			if (GetCollisionArc() != 0 && !IsColliderInsideArc(collider.transform.position, transform.position, transform.forward, GetCollisionArc()))
				continue;
			Entity entity = collider.transform.GetComponent<Entity>();
			if (entity == null || entity.IsDead) 
				continue;

			if (entityCollisions.ContainsKey(collider.transform))
			{

			}
			else if (owner == null || owner.IsHostile(entity))
			{
				entityCollisions.Add(collider.transform, Time.time);
				DecreasePierce(1);
				DoEntityEffect(entity);
				if (!data.SmallKillSFX.IsNull && entity.IsDead)
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

	public void DoEntityEffect(Entity entity)
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

	private bool IsColliderInsideArc(Vector3 colliderPosition, Vector3 position, Vector3 forward, float arc)
	{
		return Vector3.Angle(forward, colliderPosition - position) <= arc;
	}

	public void Visit(Entity visitable) { }
}
