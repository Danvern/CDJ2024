using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityServiceLocator;


public class Entity : MonoBehaviour, IVisitable
{
	public bool IsDead { get; private set; } = false;
	public bool IsEnemy { get { return isEnemy; } private set { isEnemy = value; } }
	[SerializeField] private IAgentFactory agentFactory;
	[SerializeField] private EntityHealthData healthData;
	[SerializeField] private float waypointCloseness = 0.75f;
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


	public float GetWaypointCloseness() => waypointCloseness;

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
		Vector2 forward = Quaternion.Euler(0, 0, 0f) * new Vector2(position.x - transform.position.x, position.y - transform.position.y);
		transform.rotation = Quaternion.LookRotation(Vector3.forward, forward);
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

		movement = new EntityMovementLogic.Builder(GetComponent<Rigidbody2D>())
			.WithSpeed(speed)
			.WithAcceleration(acceleration)
			.Build();

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
		movement?.Update();
		// if (agent != null)
			// ServiceLocator.For(this).Get<EntityMediator>().UpdateNavigatorPosition(transform.position);
	}

	private void Kill()
	{
		//TODO: Temp music switch
		AudioManager.Instance.SetCombatActive(true);
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
