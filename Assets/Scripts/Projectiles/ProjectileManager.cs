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

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
	{
		Instantiate(prefab, position, rotation, parent);
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
