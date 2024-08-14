using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public static ProjectileManager Instance; //TODO: Use a Builder Here

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		Instantiate(prefab, position, rotation, transform);
	}

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation, EntityMediator owner)
	{
		GameObject instance = Instantiate(prefab, position, rotation, transform);
		ProjectileBase projectile = instance.GetComponent<ProjectileBase>();
		if (projectile != null)
		{
			projectile.TakeOwnership(owner);
		}
	}

	public void GenerateProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, EntityMediator owner)
	{
		GameObject instance = Instantiate(prefab, position, rotation, transform);
		ProjectileBase projectile = instance.GetComponent<ProjectileBase>();
		if (projectile != null)
		{
			projectile.TakeOwnership(owner);
			projectile.TrackTransform(parent);
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
