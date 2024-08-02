using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour, IVisitable
{
	public bool IsDead { get; private set; } = false;
	public bool IsEnemy { get { return isEnemy; } private set { isEnemy = value; } }
	[SerializeField] private EntityHealthData healthData;
	[SerializeField] private float speed = 100;
	[SerializeField] private Weapon primaryWeapon;
	[SerializeField] private Weapon secondaryWeapon;
	[SerializeField] private bool isEnemy = true;
	private MovementLogic movement;
	private EntityHealthLogic health;
	private const float deletionDelay = 1f;


	public bool IsHostile(Entity entity)
	{
		return entity != this && IsEnemy != entity.IsEnemy;

	}

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
		{
			health = new EntityHealthLogic(healthData);
			health.entityKilled += (killer) => { Kill(); };
		}

		movement = new MovementLogic(GetComponent<Rigidbody>(), speed);

		movement.MoveToDirection(Vector3.forward);

		if (primaryWeapon != null)
			primaryWeapon.TakeOwnership(this);
		if (secondaryWeapon != null)
			secondaryWeapon.TakeOwnership(this);
	}

	// Update is called once per frame
	private void FixedUpdate()
	{

		if (movement != null)
			movement.Update(Time.deltaTime);
	}

	private void Kill()
	{
		if (IsDead) return;

		IsDead = true;
		StartCoroutine(DestroyAfterDelay(deletionDelay));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
