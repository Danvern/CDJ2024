using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	private MovementLogic movement;
	private ControlLogic control;

    // Start is called before the first frame update
    void Start()
    {
        movement = new MovementLogic(GetComponent<Rigidbody>(), 10);
		control = new ControlLogic();

		movement.MoveToDirection(Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (movement != null)
			movement.Update(Time.deltaTime);
    }
}
