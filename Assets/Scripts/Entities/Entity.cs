using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityServiceLocator;

public class Entity : EntitySubject, IVisitable
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
	[SerializeField] private Weapon dashWeapon;
	[SerializeField] private bool isEnemy = true;
	[SerializeField] private bool isUsingPickups = false;
	[SerializeField] private Animator animator;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private EntityMediator mediator;
	private IAgent agent;
	private const float deletionDelay = 1f;

	public bool IsUsingPickups() => isUsingPickups;
	public float GetWaypointCloseness() => waypointCloseness;

	public bool IsHostile(Entity entity)
	{
		return entity != this && IsEnemy != entity.IsEnemy;

	}

	public void Accept(IVisitor visitor)
	{
		visitor.Visit(this);
	}

	public void DashToAim(float power, float slideTime, bool invulnerable = false)
	{
		movement.MoveToDirection(transform.forward);
		movement.Dash(power, slideTime);
		mediator.SetInvulnerable(invulnerable, InvincibilitySource.Dashing);
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
	public void SecondaryFire(bool pressed)
	{
		if (secondaryWeapon == null) return;

		if (pressed)
			secondaryWeapon.Activate();
		else
			secondaryWeapon.Deactivate();
	}
	public void DashActivate(bool pressed)
	{
		if (dashWeapon == null) return;

		if (pressed)
			dashWeapon.Activate();
		else
			dashWeapon.Deactivate();
	}

	private void Awake()
	{
		if (healthData != null)
		{
			health = new EntityHealthLogic(healthData);
			health.EntityKilled += (killer) => { Kill(); }; // Use a Builder
		}

		movement = new EntityMovementLogic.Builder(GetComponent<Rigidbody2D>())
			.WithSpeed(speed)
			.WithAcceleration(acceleration)
			.Build();

		this.GetOrAddComponent<ServiceLocator>();
		ServiceLocator.For(this).Register(mediator = new EntityMediator(this, health, movement, animator, new AmmoInventory()));

		movement.DashFinished += (movement, arguments) =>
		{
			mediator.SetInvulnerable(false, InvincibilitySource.Dashing);
		};
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (agentFactory != null)
			agent = agentFactory.CreateAgent(mediator);
		if (primaryWeapon != null)
			primaryWeapon.TakeOwnership(mediator);
		if (secondaryWeapon != null)
			secondaryWeapon.TakeOwnership(mediator);
		if (dashWeapon != null)
			dashWeapon.TakeOwnership(mediator);

		DropController drops = GetComponent<DropController>();
		if (drops != null)
			health.EntityKilled += drops.DropReward;

		health.EntityDamaged += (damage, source) =>
		{
			var data = new EntityData
			{
				CurrentHealth = health.GetHealthCurrent(),
				MaxHealth = health.GetHealthMax(),
				CurrentMana = health.GetHealthCurrent(),
			};
			NotifyObservers(data);
		};
		NotifyObservers(GetData());
	}

	EntityData GetData()
	{
		var data = new EntityData
		{
			CurrentHealth = health.GetHealthCurrent(),
			MaxHealth = health.GetHealthMax(),
			CurrentMana = health.GetHealthCurrent(),
		};
		return data;
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		agent?.Update();
		movement?.Update();
		mediator.SetAnimationFloat("Speed", movement.GetCurrentSpeed());
		mediator.SetAnimationBool("IsMovingLeft", movement.IsMovingLeft());
		if (animator != null)
		{
			if (movement.IsMovingLeft())
				animator.transform.localScale = transform.localScale.With(x: -1);
			else if (movement.GetCurrentSpeed() > 0)
				animator.transform.localScale = transform.localScale.With(x: 1);
		}
		// if (agent != null)
		// ServiceLocator.For(this).Get<EntityMediator>().UpdateNavigatorPosition(transform.position);
	}

	private void Kill()
	{
		if (IsDead) return;

		//TODO: Temp music switch
		AudioManager.Instance.SetCombatActive(true);
		mediator.SetAnimationBool("IsDead", true);
		IsDead = true;
		StartCoroutine(DestroyAfterDelay(deletionDelay));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
