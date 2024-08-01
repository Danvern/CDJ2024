using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
	private Vector3 moveDirection = Vector2.zero;
	private Entity entity;

	private void Awake()
	{
		entity = GetComponent<Entity>();
	}

	public void OnMove(InputValue value)
	{
		moveDirection = value.Get<Vector2>();
	}

	public void OnFire(InputValue value)
	{
		entity.PrimaryFire(value.isPressed);
	}

	private void Update()
	{
		if (entity == null)
			return;

		entity.MoveToDirection(new Vector3(moveDirection.x, 0, moveDirection.y));
	}

}
