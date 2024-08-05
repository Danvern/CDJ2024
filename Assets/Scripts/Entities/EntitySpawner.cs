using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] GameObject enemy;
	[SerializeField] float cooldown = 10f;
	[SerializeField] float cooldownRandomization = 10f;
	float lastSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time + Random.value * cooldownRandomization - cooldownRandomization / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime > cooldown)
		{
			Instantiate(enemy, transform.position, transform.rotation);
			lastSpawnTime = Time.time + Random.value * cooldownRandomization - cooldownRandomization / 2;
		}
    }
}
