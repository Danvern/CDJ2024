using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityServiceLocator;


public class Entity : MonoBehaviour, IVisitable
{
	public bool IsDead { get; private set; } = false;
	public bool IsEnemy { get { return isEnemy; } private set { isEnemy = value; } }
	[SerializeField] private IAgentFactory agentFactory;
	[SerializeField] private EntityHealthData healthData;
	[SerializeField] private float speed = 10;
	[SerializeField] private float acceleration = 100;
	[SerializeField] private Weapon primaryWeapon;
	[SerializeField] private Weapon secondaryWeapon;
	[SerializeField] private bool isEnemy = true;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private EntityMediator mediator;
	private IAgent agent;
	private const float deletionDelay = 1f;


	public bool IsHostile(Entity entity)
	{
		return entity != this && IsEnemy != entity.IsEnemy;

	}

	public void Accept(IVisitor visitor)
	{
		visitor.Visit(this);
	}

	public void DashToAim(float power, float slideTime, bool fullStun = true)
	{
		movement.MoveToDirection(transform.forward);
		movement.Dash(power, slideTime);
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

	private void Awake()
	{
		if (healthData != null)
		{
			health = new EntityHealthLogic(healthData);
			health.entityKilled += (killer) => { Kill(); }; // Use a Builder
		}

		movement = EntityMovementLogic.CreateMovementLogic(GetComponent<Rigidbody>());
		movement.SetSpeed(speed);
		movement.SetAcceleration(acceleration);
		movement.MoveToDirection(Vector3.forward); // Use a Builder

		this.GetOrAddComponent<ServiceLocator>();
		ServiceLocator.For(this).Register(mediator = new EntityMediator(this, health, movement));

		if (agentFactory != null)
			agent = agentFactory.CreateAgent(mediator);

		if (primaryWeapon != null)
			primaryWeapon.TakeOwnership(mediator);
		if (secondaryWeapon != null)
			secondaryWeapon.TakeOwnership(mediator);
	}

	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		agent?.Update();
		if (movement != null)
			movement.Update();
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
