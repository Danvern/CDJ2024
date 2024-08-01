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
	private Vector3 forward = Vector3.zero;

	private Dictionary<Transform, float> entityCollisions = new Dictionary<Transform, float>();

	public ProjectileDamageLogic(ProjectileDamageData data)
	{
		lifetime = data.Lifetime;
		damageMax = data.DamageMax;
		damageMin = data.DamageMin;
		piercing = data.Piercing;
		collisionRadius = data.CollisionRadius;
	}

	public float GetLifetime() { return lifetime; }

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }

	public float GetCollisionRadius() { return collisionRadius; }

	public float GetCollisionArc() { return collisionArc; }

	public Vector3 GetForwardVector() { return forward; }

	public bool CheckCollisons(Vector3 position, Entity owner)
	{
		bool kill = false;
		Collider[] potentialCollisions = Physics.OverlapSphere(position, GetCollisionRadius());

		if (potentialCollisions.Length == 0) return false;

		foreach (Collider collider in potentialCollisions)
		{
			if (GetCollisionArc() != 0 && !IsColliderInsideArc(collider.transform.position, position, GetForwardVector(), GetCollisionArc()))
				continue;
			Entity entity = collider.transform.GetComponent<Entity>();
			if (entity == null) 
				continue;

			if (entityCollisions.ContainsKey(collider.transform))
			{

			}
			else if (owner.IsHostile(entity))
			{
				entityCollisions.Add(collider.transform, Time.time);
				DecreasePierce(1);
				DoEntityEffect(entity);
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

	public void Visit(MovementLogic visitable) { }

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
