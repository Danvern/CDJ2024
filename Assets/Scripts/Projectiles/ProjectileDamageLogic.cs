using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;

public class ProjectileDamageLogic : IProjectileDamageLogic
{
	private float damageMax = 0;
	private float damageMin = 1;
	private int piercing = 0;

	private float collisionRadius = 3;
	private float collisionArc = 0;
	private Vector3 forward = Vector3.zero;

	private Dictionary<Transform, float> entityCollisions = new Dictionary<Transform, float>();

	public ProjectileDamageLogic(ProjectileDamageData data)
	{
		damageMax = data.DamageMax;
		damageMin = data.DamageMin;
		piercing = data.Piercing;
		collisionRadius = data.CollisionRadius;
	}

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }

	public float GetCollisionRadius() { return collisionRadius; }

	public float GetCollisionArc() { return collisionArc; }

	public Vector3 GetForwardVector() { return forward; }

	public void CheckCollisons(Vector3 position)
	{
		Collider[] potentialCollisions = Physics.OverlapSphere(position, GetCollisionRadius());

		if (potentialCollisions.Length == 0) return;

		foreach (Collider collider in potentialCollisions)
		{
			if (GetCollisionArc() != 0 && !IsColliderInsideArc(collider.transform.position, position, GetForwardVector(), GetCollisionArc())) 
				continue;

			if (entityCollisions.ContainsKey(collider.transform))
			{

			}
			else
			{
				entityCollisions.Add(collider.transform, Time.time);
				DoEntityEffect(collider.transform);
			}
		}
	}

	public void DoEntityEffect(Transform transform)
	{
		Entity entity = transform.GetComponent<Entity>();
		if (entity == null) return;

		entity.Accept(this);
	}

	public void Visit(MovementLogic visitable) {}

	public void Visit(EntityHealthLogic visitable) 
	{
		visitable.DoDamage(GetDamageRandom());
		Debug.Log(visitable.ToString());
		
	}

	private bool IsColliderInsideArc(Vector3 colliderPosition, Vector3 position, Vector3 forward, float arc)
	{
		return Vector3.Angle(forward, colliderPosition - position) <= arc;
	}

	public void Visit(Entity visitable) {}
}
