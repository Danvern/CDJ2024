using System.Collections;
using System.Collections.Generic;
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

	public void DoHitLag(float time)
	{
		StartCoroutine(HitLag(time, 0f));
	}

	private IEnumerator HitLag(float time, float timescaleFreeze)
	{
		Time.timeScale = timescaleFreeze;
		yield return new WaitForSecondsRealtime(time);
		Time.timeScale = 1f;

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
