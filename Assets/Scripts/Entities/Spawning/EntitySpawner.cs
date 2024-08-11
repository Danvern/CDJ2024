using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] EntitySpawnerFactory factory;
	EntitySpawnerLogic logic;

	// Start is called before the first frame update
	void Awake()
	{
		logic = factory.CreateSpawnerLogic();
	}

	// Update is called once per frame
	void Update()
	{
		logic.Update(transform);
	}
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 1f);

	}
#endif
}
