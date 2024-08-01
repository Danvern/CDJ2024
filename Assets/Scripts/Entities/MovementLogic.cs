using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MovementLogic : IVisitable
{
	private Rigidbody rb;
	private Vector3 targetDirection = Vector3.zero;
	[SerializeField] private float speed = 100;
	public float Speed { get { return speed; } set { speed = Math.Max(0, value); } }

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	public MovementLogic(Rigidbody rb)
	{
		this.rb = rb;
	}

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
