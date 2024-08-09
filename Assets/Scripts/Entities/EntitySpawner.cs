using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] EntitySpawnerFactory factory;
	EntitySpawnerLogic logic;

    // Start is called before the first frame update
    void Start()
    {
		logic = factory.CreateSpawnerLogic();
    }

    // Update is called once per frame
    void Update()
    {
        logic.Update();
    }
}
