using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;

public class ProjectileDamageLogic : IProjectileDamageLogic
{
	private float damageMax = 0;
	private float damageMin = 1;
	private int piercing = 0;

	private int collisionRadius = 3;
	private int collisionArc = 90;

	private Dictionary<Transform, float> entityCollisions = new Dictionary<Transform, float>();

	public ProjectileDamageLogic(ProjectileDamageData data)
	{
		damageMax = data.DamageMax;
		damageMin = data.DamageMin;
		piercing = data.Piercing;
	}

	public float GetDamageMax() { return damageMax; }

	public float GetDamageMin() { return damageMin; }

	public float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public bool DoesPierce(int resistance) { return piercing > resistance; }

	public void DecreasePierce(int resistance) { piercing = Mathf.FloorToInt(piercing - resistance); }

	public void CheckCollisons(Vector3 position)
	{
		Collider[] potentialCollisions = Physics.OverlapSphere(position, collisionRadius);

		if (potentialCollisions.Length == 0) return;

		foreach (Collider collider in potentialCollisions)
		{
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

	public void Visit(Entity visitable) {}
}
