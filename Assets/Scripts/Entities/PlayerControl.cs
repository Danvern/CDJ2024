using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityServiceLocator;

public class PlayerControl : MonoBehaviour
{
	private Vector2 moveDirection = Vector2.zero;
	private Entity entity;
	private float trackingRadius = 5;
	private Vector3 aimPosition = Vector3.zero;

	private void Awake()
	{
		entity = GetComponent<Entity>();
	}

	private void Start()
	{
		ServiceLocator.Global.Get<AgentDirector>().SetPrimaryPlayer(ServiceLocator.For(this).Get<EntityMediator>());
	}

	public void OnMove(InputValue value)
	{
		moveDirection = value.Get<Vector2>();
	}

	public void OnFire(InputValue value)
	{
		entity.PrimaryFire(value.isPressed);
	}

	/// <summary>
	/// Update the position of the targeting cursor as well as the hovered target.
	/// </summary>
	/// <returns>
	/// Returns the target GameObject which was hovered.
	/// </returns>
	private GameObject TargetCursor()
	{
		RaycastHit hitData;
		GameObject targetEnemy = null;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Ray entityRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask entityMask = LayerMask.GetMask("Entities");
		LayerMask groundMask = LayerMask.GetMask("Ground");

		if (Physics.Raycast(ray, out hitData, 1000f, groundMask))
		{
			aimPosition = new(hitData.point.x, hitData.point.y, 0);
		}

		if (Physics.Raycast(entityRay, out hitData, 1000f, entityMask))
		{
			if (hitData.collider.gameObject.CompareTag("Enemy"))
				targetEnemy = hitData.collider.gameObject;
		}
		else
		{
			float lastDistance = trackingRadius;
			foreach (Collider enemy in Physics.OverlapSphere(aimPosition, trackingRadius, entityMask))
			{
				if (enemy == null || enemy.GetComponent<Entity>() == null || enemy.GetComponent<Entity>().IsDead)
					continue;
				if (targetEnemy == null || Vector3.Distance(enemy.transform.position, hitData.point) < lastDistance)
				{
					lastDistance = Vector3.Distance(enemy.transform.position, hitData.point);
					targetEnemy = enemy.gameObject;
				}
			}
		}

		return targetEnemy;
	}

	private void Update()
	{
		if (entity == null)
			return;

		entity.MoveToDirection(new Vector3(moveDirection.x, moveDirection.y, 0));
		// Debug.Log(moveDirection);
		TargetCursor();
		entity.FacePosition(aimPosition);
	}

}
