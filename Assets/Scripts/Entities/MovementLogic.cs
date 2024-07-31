using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementLogic
{
	private Rigidbody rb;
	private Vector3 targetDirection = Vector3.zero;
	private float speed = 10;

	public MovementLogic(Rigidbody rb, float speed)
	{
		this.rb = rb;
		this.speed = speed;
	}

	// Change movement direction.
	public void MoveToDirection(Vector3 direction)
	{
		targetDirection = direction;
	}

    public void Update(float deltaTime)
	{
		if (rb == null)
			return;
		
		rb.velocity = Vector3.zero;
		rb.AddForce(targetDirection * speed * deltaTime, ForceMode.VelocityChange);
	}
}
