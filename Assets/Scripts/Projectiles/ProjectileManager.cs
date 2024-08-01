using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public static ProjectileManager Instance;

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		Instantiate(prefab, position, rotation, transform);
	}

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Entity owner)
	{
		GenerateProjectile(prefab, position, rotation, transform, owner);
	}

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, Entity owner)
	{
		GameObject instance = Instantiate(prefab, position, rotation, parent);
		ProjectileBase projectile = instance.GetComponent<ProjectileBase>();

		if (projectile != null)
		{
			projectile.TakeOwnership(owner);
		}
	}

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Multiple projectile managers detected!");
		}
	}
}
