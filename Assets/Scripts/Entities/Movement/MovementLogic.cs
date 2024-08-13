using System;
using Pathfinding;
using UnityEngine;
using UnityServiceLocator;

public interface IMovementLogic : IVisitable
{
	void MoveToDirection(Vector2 direction);
	float GetSpeed();
	void SetSpeed(float speed);
	float GetAcceleration();
	void SetAcceleration(float acceleration);
	Vector3 GetTargetDirection();
	Vector3 GetFacingDirection();
	float GetCurrentSpeed();
	bool IsMovingLeft();
	public bool IsFollowingPath();
	public bool IsPathPending();
	public float RemainingPathDistance();
	void Dash(float power, float slideTime);
	void KnockbackStun(float power, float slideTime, Vector3 direction);
	public void CalculatePath(Seeker navigator, Vector3 position, Vector3 target);
	public void CancelPath(Seeker navigator);
	public Vector2 GetNextPathNode(float stoppingDistance);
	Rigidbody2D GetRigidbody();
	void Update();

}

public class EntityMovementLogic : IMovementLogic
{
	public class Builder
	{
		private Rigidbody2D rb;
		private float speed;
		private float acceleration;
		public Builder(Rigidbody2D rb) {
			this.rb = rb;
		}
		public Builder WithSpeed(float speed){
			this.speed = speed;
			return this;
		}
		public Builder WithAcceleration(float acceleration){
			this.acceleration = acceleration;
			return this;
		}
		public EntityMovementLogic Build(){
			var logic = new EntityMovementLogic(rb)
			{
				acceleration = acceleration,
				speed = speed
			};
			logic.SetupStateMachine();
			return logic;
		}
	}
	enum MoveTrigger { Walk, Dash, Stun }
	public const float DEFAULT_DASH_MULTIPLIER = 5f;
	public float Speed { get { return speed; } set { speed = Mathf.Max(0, value); } }
	private MoveTrigger moveTrigger = MoveTrigger.Walk;
	private Rigidbody2D rb;
	private Vector3 targetDirection = Vector3.zero;
	private Vector3 facingDirection = Vector3.right;
	private float speed = 10f;
	private float acceleration = 100f;
	private float dashAcceleration = 100f;
	private float dashDuration = 0.5f;
	private StateMachine stateMachine;
	private MoveDash dashState;
	private MoveStun stunState;
	private bool isOnPath = false;
	private bool isPathCalculating = false;
	private int currentWaypoint = 0;
	private float pathRemaining = 0;
	private Path currentPath;

	public float GetAcceleration() { return acceleration; }
	public void SetAcceleration(float acceleration) { this.acceleration = acceleration; }
	public float GetSpeed() { return speed; }
	public void SetSpeed(float speed) { this.speed = speed; }
	public Vector3 GetTargetDirection() { return targetDirection; }
	public Vector3 GetFacingDirection() { return facingDirection; }
	public Rigidbody2D GetRigidbody() { return rb; }
	public bool IsFollowingPath() { return isOnPath; }
	public bool IsPathPending() { return isPathCalculating; }
	public float GetCurrentSpeed() { return rb.velocity.magnitude; }
	public bool IsMovingLeft() { return rb.velocity.x < 0; }
	public void CalculatePath(Seeker navigator, Vector3 position, Vector3 target)
	{
		currentPath = navigator.StartPath(position, target, onPathComplete); // Always level planes
		isPathCalculating = true;
		pathRemaining = Vector2.Distance(target, rb.position);

	}
	public void CancelPath(Seeker navigator)
	{
		navigator.CancelCurrentPathRequest();
		isPathCalculating = false;
		isOnPath = false;
		currentPath = null;
		pathRemaining = 0;

	}
	public float RemainingPathDistance()
	{
		return pathRemaining;
	}

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	private EntityMovementLogic(Rigidbody2D rb)
	{
		this.rb = rb;
	}

	// Change movement direction.
	public void MoveToDirection(Vector2 direction)
	{
		if (direction != Vector2.zero && moveTrigger != MoveTrigger.Stun)
			facingDirection = direction.normalized;
		targetDirection = direction.normalized;
	}

	public void Dash(float power, float slideTime)
	{
		moveTrigger = MoveTrigger.Dash;
		dashState.UpdateParameters(power, slideTime);
	}

	public void KnockbackStun(float power, float slideTime, Vector3 direction)
	{
		moveTrigger = MoveTrigger.Stun;
		facingDirection = direction;
		stunState.UpdateParameters(power, slideTime);
	}

	public void Update()
	{
		UpdatePathLogic();

		stateMachine.Update();
	}

	public void SetupStateMachine()
	{
		stateMachine = new StateMachine();
		void Af(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, new FunctionPredicate(condition));
		void Aaf(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, new FunctionPredicate(condition));
		//void At(IState from, IState to, TriggerPredicate trigger) => stateMachine.AddTransition(from, to, trigger);

		MoveWalk walk = new MoveWalk(this);
		dashState = new MoveDash(this, speed * DEFAULT_DASH_MULTIPLIER);
		stunState = new MoveStun(this, speed * DEFAULT_DASH_MULTIPLIER);
		dashState.Finish += () => moveTrigger = MoveTrigger.Walk;
		stunState.Finish += () => moveTrigger = MoveTrigger.Walk;

		Aaf(stunState, () => moveTrigger == MoveTrigger.Stun);
		Af(walk, dashState, () => moveTrigger == MoveTrigger.Dash);
		Af(dashState, walk, () => dashState.Finished);
		Af(stunState, walk, () => stunState.Finished);

		stateMachine.SetState(walk);
	}

	void UpdatePathLogic()
	{
		if (currentPath == null) return;

		if (currentWaypoint >= currentPath.vectorPath.Count)
		{
			isOnPath = false;
			pathRemaining = 0;
		}
		else
			isOnPath = true;

		pathRemaining = currentPath.GetTotalLength();
	}

	public Vector2 GetNextPathNode(float stoppingDistance)
	{
		Vector2 direction = ((Vector2)currentPath.vectorPath[currentWaypoint] - rb.position).normalized;

		float distance = Vector2.Distance(currentPath.vectorPath[currentWaypoint], rb.position);
		if (distance < stoppingDistance)
			currentWaypoint++;

		return direction;
	}

	void onPathComplete(Path path)
	{
		if (!path.error)
		{
			currentPath = path;
			currentWaypoint = 0;
		}
		else
		pathRemaining = 0;
		isPathCalculating = false;
	}
}
