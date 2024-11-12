using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityServiceLocator;

public class SpawnDirector : MonoBehaviour
{
	public float MaxSpawnDistance = 20;
	public float MinSpawnDistance = 12;
	List<EntitySpawner> spawnPoints;
	public GameObject LastBoss;

	public bool SpawnEntities(GameObject entity, float amount, SpawnPointType type, Vector3? exclusionPoint = null)
	{
		int attempts = 3;
		float remaining = amount;
		while (remaining > 0 && attempts > 0)
		{
			foreach (EntitySpawner spawner in spawnPoints)
			{
				if (remaining <= 0)
					return true;
				if (exclusionPoint != null && (Vector3.Distance(spawner.transform.position, (Vector3)exclusionPoint) > MaxSpawnDistance))
					continue;
				if (exclusionPoint != null && (Vector3.Distance(spawner.transform.position, (Vector3)exclusionPoint) < MinSpawnDistance))
					continue;
				var created = Instantiate(entity, spawner.transform.position, entity.transform.rotation);
				if (type == SpawnPointType.Boss)
					LastBoss = created;
				remaining--;
			}
			spawnPoints.Shuffle();
			attempts--;
		}
		return false;

	}

	void Awake()
	{
		ServiceLocator.ForSceneOf(this).Register(this);
	}

	// Start is called before the first frame update
	void Start()
	{
		spawnPoints = GetComponentsInChildren<EntitySpawner>().ToList();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
