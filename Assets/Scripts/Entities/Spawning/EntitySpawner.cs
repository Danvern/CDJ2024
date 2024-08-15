using UnityEngine;

public enum SpawnPointType {Standard, Elite, Boss}
public class EntitySpawner : MonoBehaviour
{
	[SerializeField] SpawnPointType type = SpawnPointType.Standard;

	// Start is called before the first frame update
	void Awake()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, .5f);

	}
#endif
}
