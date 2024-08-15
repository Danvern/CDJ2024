using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
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
	[SerializeField] private int magicStorage = 10;
	[SerializeField] private int scoreValue = 100;
	[SerializeField] private Weapon primaryWeapon;
	[SerializeField] private Weapon secondaryWeapon;
	[SerializeField] bool isSecondaryTriggeredOnDeath;
	[SerializeField] private Weapon dashWeapon;
	[SerializeField] private bool isEnemy = true;
	[SerializeField] private bool isUsingPickups = false;
	[SerializeField] private Animator animator;
	private Vector2 aimTarget;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private EntityMediator mediator;
	private int personalScore;
	private IAgent agent;
	[SerializeField] bool isHeavy = false;
	private const float deletionDelay = 1f;

	public bool IsHeavy() => isHeavy;
	public Vector2 GetAimTarget() => aimTarget;
	public void SetAimTarget(Vector2 value) => aimTarget = value;

	public bool IsUsingPickups() => isUsingPickups;
	public float GetWaypointCloseness() => waypointCloseness;

	public bool IsHostile(Entity entity)
	{
		return entity != this && IsEnemy != entity.IsEnemy;

	}

	public int GetScoreReward() => scoreValue;
	public int GetPersonalScore() => personalScore;
	public int AddPersonalScore(int score) => personalScore += score;

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

	public void DashToDirection(Vector3 direction, float power, float slideTime, bool invulnerable = false)
	{
		movement.MoveToDirection(direction);
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

		primaryWeapon.SetAnimationTag("IsAttacking");
		if (pressed)
			primaryWeapon.Activate();
		else
			primaryWeapon.Deactivate();
	}
	public void SecondaryFire(bool pressed)
	{
		if (secondaryWeapon == null) return;

		secondaryWeapon.SetAnimationTag("IsAttackingSecondary");
		if (pressed)
			secondaryWeapon.Activate();
		else
			secondaryWeapon.Deactivate();
	}
	public void DashActivate(bool pressed)
	{
		if (dashWeapon == null) return;

		dashWeapon.SetAnimationTag("IsAttackingDash");
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
		mediator.SetAmmoMax(AmmoType.Magic, magicStorage);

		DropController drops = GetComponent<DropController>();
		if (drops != null)
			health.EntityKilled += drops.DropReward;
		health.EntityKilled += (source) => {
			if (source.OrNull() == null) return;
			var owner = source.GetOwner();
			if (owner != null && !owner.IsDead())
				source.GetOwner().AddScore(scoreValue);

		};

		health.EntityDamaged += (damage, source) =>
		{
			var data = new EntityData
			{
				CurrentHealth = health.GetHealthCurrent(),
				MaxHealth = health.GetHealthMax(),
				CurrentMana = mediator.GetAmmo(AmmoType.Magic),
				MaxMana = mediator.GetAmmoMax(AmmoType.Magic),
				Score = GetPersonalScore(),
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
			CurrentMana = mediator.GetAmmo(AmmoType.Magic),
			MaxMana = mediator.GetAmmoMax(AmmoType.Magic),
		};
		return data;
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		agent?.Update();
		movement?.Update();
		health?.Update();
		mediator.SetAnimationFloat("Speed", movement.GetCurrentSpeed());
		//mediator.SetAnimationBool("IsMovingLeft", movement.IsMovingLeft());
		if (animator != null)
		{
			bool isLookingLeft = transform.rotation.eulerAngles.z > 0f && transform.rotation.eulerAngles.z  < 180f;
			if (movement.IsMovingLeft())
			animator.transform.localScale = transform.localScale.With(x: -1);
			else if (movement.GetCurrentSpeed() > 0)
				animator.transform.localScale = transform.localScale.With(x: 1);
			else if (isLookingLeft)
				animator.transform.localScale = transform.localScale.With(x: -1);
			else
				animator.transform.localScale = transform.localScale.With(x: 1);

		}
		// if (agent != null)
		// ServiceLocator.For(this).Get<EntityMediator>().UpdateNavigatorPosition(transform.position);
	}

	private void Kill()
	{
		if (IsDead) return;

		if (isSecondaryTriggeredOnDeath)
			SecondaryFire(true);

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
