using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EntitySpawner", menuName = "GameplayDefinitions/Spawner/Basic", order = 1)]
public class EntitySpawnerFactory : ScriptableObject
{
	[SerializeField] GameObject enemy;
	[SerializeField] float cooldown = 10f;
	[SerializeField] float cooldownRandomization = 10f;
	[SerializeField] float cooldownInitial = 10f;
	[SerializeField] float spawnChance = 1f;
	public EntitySpawnerLogic CreateSpawnerLogic()
	{
		return new EntitySpawnerLogic.Builder()
		.WithEnemy(enemy)
		.WithCooldown(cooldown)
		.WithCooldownRandomization(cooldownRandomization)
		.WithCooldownInitial(cooldownInitial)
		.WithSpawnChance(spawnChance)
		.Build();
	}

}
