using UnityEngine;

public class EntitySpawnerLogic
{
	[SerializeField] GameObject enemy;
	[SerializeField] float cooldown = 10f;
	[SerializeField] float cooldownRandomization = 10f;
	[SerializeField] float cooldownInitial = 10f;
	[SerializeField] float spawnChance = 1f;
	float lastSpawnTime;
	public class Builder
	{
		GameObject enemy;
		float cooldown;
		float cooldownRandomization;
		float cooldownInitial;
		float spawnChance;
		public Builder WithEnemy(GameObject enemy)
		{
			this.enemy = enemy;
			return this;
		}
		public Builder WithCooldown(float cooldown)
		{
			this.cooldown = cooldown;
			return this;
		}
		public Builder WithCooldownRandomization(float cooldown)
		{
			this.cooldownRandomization = cooldown;
			return this;
		}
		public Builder WithCooldownInitial(float cooldown)
		{
			this.cooldownInitial = cooldown;
			return this;
		}
		public Builder WithSpawnChance(float chance)
		{
			this.spawnChance = chance;
			return this;
		}
		public EntitySpawnerLogic Build()
		{
			var logic = new EntitySpawnerLogic()
			{
				enemy = enemy,
				cooldown = cooldown,
				cooldownRandomization = cooldownRandomization,
				cooldownInitial = cooldownInitial,
				spawnChance = spawnChance,
			};
			logic.Initialize();
			return logic;
		}
	}
	private EntitySpawnerLogic() { }

	// Start is called before the first frame update
	void Initialize()
	{
		lastSpawnTime = Time.time + cooldownInitial + Random.value * cooldownRandomization - cooldownRandomization / 2;
	}

	// Update is called once per frame
	public void Update(Transform transform)
	{
		if (Time.time - lastSpawnTime > cooldown)
		{
			if (spawnChance > 0 && Random.value >= spawnChance)
				GameObject.Instantiate(enemy, transform.position, transform.rotation);
			lastSpawnTime = Time.time + Random.value * cooldownRandomization - cooldownRandomization / 2;
		}
	}
}
