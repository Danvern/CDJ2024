using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour, IVisitable
{
	public bool IsDead { get; private set; } = false;
	[SerializeField] private EntityHealthData healthData;
	[SerializeField] private MovementLogic movement;
	[SerializeField] private Weapon primaryWeapon;
	[SerializeField] private Weapon secondaryWeapon;

	private EntityHealthLogic health;

	public void Accept(IVisitor visitor)
	{
		visitor.Visit(this);
		if (health != null)
			visitor.Visit(health);
		if (movement != null)
			visitor.Visit(movement);
	}

	public void MoveToDirection(Vector3 direction)
	{
		movement.MoveToDirection(direction);
	}

	public void FacePosition(Vector3 position)
	{
		Vector3 forward = new Vector3(position.x - transform.position.x, 0, position.z - transform.position.z);
		transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
	}

	public void PrimaryFire(bool pressed)
	{
		if (primaryWeapon == null) return;

		if (pressed)
			primaryWeapon.Activate();
		else
			primaryWeapon.Deactivate();
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (healthData != null)
			health = new EntityHealthLogic(healthData);

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
