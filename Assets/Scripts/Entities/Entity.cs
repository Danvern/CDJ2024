using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour
{
	[SerializeField] private EntityHealthData healthData;
	[SerializeField] private MovementLogic movement;
	[SerializeField] private Weapon primaryWeapon;
	[SerializeField] private Weapon secondaryWeapon;

	private EntityHealthLogic health;

	public void MoveToDirection(Vector3 direction)
	{
		movement.MoveToDirection(direction);
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
