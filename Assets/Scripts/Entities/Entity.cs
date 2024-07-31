using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour
{
	[SerializeField] private MovementLogic movement;

	public void MoveToDirection(Vector3 direction)
	{
		movement.MoveToDirection(direction);
	}

    // Start is called before the first frame update
    private void Start()
    {
        movement = new MovementLogic(GetComponent<Rigidbody>());

		movement.MoveToDirection(Vector3.forward);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (movement != null)
			movement.Update(Time.deltaTime);
    }
}
