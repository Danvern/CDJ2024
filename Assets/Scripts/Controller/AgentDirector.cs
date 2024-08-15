using System;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityServiceLocator;
using Random = UnityEngine.Random;

[Serializable]
public class SpawningEntry
{
	public int MaxAmount = 0;
	public int MinAmount = 3;
	public SpawnPointType Category;
	public GameObject Entity;
	public int MaxWave = 0;
	public int MinWave = 0;

}
public class AgentDirector : MonoBehaviour
{
	[SerializeField] int currentWave = 0;
	[SerializeField] SpawningEntry[] SpawningEntries = new SpawningEntry[0];

	[SerializeField] int[] scoreThresholds = new int[0];
	[SerializeField] float timeBetweenSpawns;
	float lastSpawn = 0;
	private EntityMediator targetPlayer;
	private EntityMediator targetBoss;
	private int playerScore;

	public void SetPrimaryBoss(EntityMediator targetBoss) => this.targetBoss = targetBoss;
	public EntityMediator GetPrimaryBoss() => targetBoss;
	public void SetPrimaryPlayer(EntityMediator targetPlayer) => this.targetPlayer = targetPlayer;
	public EntityMediator GetPrimaryPlayer() => targetPlayer;

	// Start is called before the first frame update
	void Awake()
	{
		ServiceLocator.ForSceneOf(this).Register(this);
		lastSpawn = Time.time - timeBetweenSpawns;
	}


	// Update is called once per frame
	void FixedUpdate()
	{
		playerScore = targetPlayer.GetScore();

		if (scoreThresholds.Length > currentWave && playerScore > scoreThresholds[currentWave])
			currentWave++;

		if (Time.time - lastSpawn > timeBetweenSpawns)
		{
			var spawner = ServiceLocator.ForSceneOf(this).Get<SpawnDirector>();

			lastSpawn = Time.time;
			foreach (var spawningEntry in SpawningEntries)
			{
				spawner.SpawnEntities(spawningEntry.Entity, Random.Range(spawningEntry.MinAmount, spawningEntry.MaxAmount), spawningEntry.Category, GetPrimaryPlayer().GetTransform().position);
				if (spawningEntry.Category == SpawnPointType.Boss)
				SetPrimaryBoss(ServiceLocator.For(spawner.LastBoss.GetComponent<Entity>()).Get<EntityMediator>());
			}

		}
	}
}
