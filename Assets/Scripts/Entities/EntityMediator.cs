using BlackboardSystem;
using Pathfinding;
using UnityEngine;
using UnityServiceLocator;


public class EntityMediator : IVisitable, ILootMediator, IAmmunitionSource, IHealth
{
	private Entity entity;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private Seeker navigator;
	private Animator animator;
	private AmmoInventory ammunition;

	public EntityMediator(Entity entity, EntityHealthLogic health, IMovementLogic movement, Animator animator, AmmoInventory ammo)
	{
		this.entity = entity;
		this.health = health;
		this.movement = movement;
		navigator = entity.GetComponent<Seeker>();
		this.animator = animator;
		ammunition = ammo;
	}
	public Entity GetEntity()
	{
		return entity;

	}
	public Vector3 GetTargetPosition()
	{
		Blackboard blackboard = GetServiceLocator().Get<BlackboardController>().GetBlackboard();
		var targetKey = blackboard.GetOrRegisterKey("Target");
		if (blackboard == null) return Vector3.zero;
			if (blackboard.TryGetValue(targetKey, out EntityMediator target))
				return target.GetTransform().position;
		return Vector3.zero;
	}
	public float GetHealth() { return health.GetHealthCurrent(); }
	public float GetHealthMax() { return health.GetHealthMax(); }
	public void SetInvulnerable(bool invulnerable, InvincibilitySource source) { health.SetInvulnerable(invulnerable, source); }
	public void AddHealth(float value)
	{
		health.Heal(value);

	}
	public void AddShield(float shield)
	{
		return;

	}
	public void AddScore(int score) => entity.AddPersonalScore(score);
	public void AddAmmo(AmmoType type, int amount, float maxMultiplier = 1f) { ammunition.AddAmmo(type, amount, maxMultiplier); }
	public bool IsUsingPickups() => entity.IsUsingPickups();
	public int GetAmmo(AmmoType type) { return ammunition.GetAmmo(type); }
	public int GetAmmoMax(AmmoType type) { return ammunition.GetAmmoMax(type); }
	public void SetAmmo(AmmoType type, int amount) { ammunition.SetAmmo(type, amount); }
	public void SetAmmoMax(AmmoType type, int amount) { ammunition.SetAmmoMax(type, amount); }
	public IUpgradeStats GetUpgradeStats() { return null; }
	public IAffinity GetAffinity() { return null; }
	public ServiceLocator GetServiceLocator() => ServiceLocator.For(entity);
	public bool IsHostile(EntityMediator mediator) => mediator.entity != entity && mediator.entity.IsEnemy != entity.IsEnemy;
	public bool IsDead() => entity.OrNull() == null || entity.IsDead;
	public Transform GetTransform() => entity.OrNull() != null ? entity.transform : null;
	public Vector2 GetPosition() => entity.transform.position;
	public void ActivateGuidanceMode()
	{
		// Bootstrap seeker
	}
	public void NavigatePathTo(Vector2 targetPosition)
	{
		movement.CalculatePath(navigator, entity.transform.position, targetPosition);
		//		MoveToDirection(navigator.steeringTarget);
		MoveToDirection(targetPosition);
	}
	public void CancelPath() => movement.CancelPath(navigator);
	public bool IsNavigating() => movement.IsFollowingPath(); //navigator.pathPending;
	public bool IsNavigatorActive() => movement.IsPathPending() || movement.IsFollowingPath(); //navigator.pathPending;
	public bool IsNavigatorCalculating() => movement.IsPathPending(); //navigator.pathPending;
																	  //public void UpdateNavigatorPosition(Vector3 position) {} //navigator.nextPosition = position;
	public float GetRemainingTravelDistance() => movement.RemainingPathDistance();
	public float GetWaypointCloseness() => entity.GetWaypointCloseness();
	//	public float GetRemainingTravelDistance() => navigator.remainingDistance;
	public void AddObserver(IEntityObserver observer) => entity?.AddObserver(observer);
	public void RemoveObserver(IEntityObserver observer) => entity?.RemoveObserver(observer);
	public void PlayAnimation(string animationName)
	{
		if (animator == null) return;

		animator?.Play(animationName);
	}
	public void SetAnimationFloat(string valueName, float value)
	{
		if (animator == null) return;

		animator?.SetFloat(valueName, value);
	}
	public void SetAnimationInt(string valueName, int value)
	{
		if (animator == null) return;

		animator?.SetInteger(valueName, value);
	}
	public void SetAnimationBool(string valueName, bool value)
	{
		if (animator == null) return;

		animator?.SetBool(valueName, value);
	}

	public void Accept(IVisitor visitor)
	{
		entity?.Accept(visitor);
		health?.Accept(visitor);
		movement?.Accept(visitor);
		visitor.Visit(this);
	}

	public void DashToAim(float power, float slideTime, bool invulnerable = false) => entity?.DashToAim(power, slideTime, invulnerable);
	public void DashToDirection(Vector3 direction, float power, float slideTime, bool invulnerable = false) => entity?.DashToDirection(direction, power, slideTime, invulnerable);
	public void MoveToDirection(Vector2 direction) => movement?.MoveToDirection(direction);
	public void FacePosition(Vector2 position) => entity?.FacePosition(position);
	public void PrimaryFire(bool pressed) => entity?.PrimaryFire(pressed);
	public void SecondaryFire(bool pressed) => entity?.SecondaryFire(pressed);


}
