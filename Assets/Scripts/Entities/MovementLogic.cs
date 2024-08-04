using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMovementLogic
{
	void MoveToDirection(Vector3 direction);
	float GetSpeed();
	void SetSpeed(float speed);
	Vector3 GetTargetDirection();
	Vector3 GetFacingDirection();
	void Dash(float power, float slideTime);
	Rigidbody GetRigidbody();
	void Update();

}

public class MovementLogic : IVisitable, IMovementLogic
{
	enum MoveTrigger { Walk, Dash, }
	public const float DEFAULT_DASH_MULTIPLIER = 5f;
	public float Speed { get { return speed; } set { speed = Mathf.Max(0, value); } }
	private MoveTrigger moveTrigger = MoveTrigger.Walk;
	private Rigidbody rb;
	private Vector3 targetDirection = Vector3.zero;
	private Vector3 facingDirection = Vector3.forward;
	private float speed = 100f;
	private float dashAcceleration = 100f;
	private float dashDuration = 0.5f;
	private StateMachine stateMachine;
	private MoveDash dashState;

	public float GetSpeed() { return speed; }
	public void SetSpeed(float speed) { this.speed = speed; }
	public Vector3 GetTargetDirection() { return targetDirection; }
	public Vector3 GetFacingDirection() { return facingDirection; }
	public Rigidbody GetRigidbody() { return rb; }

	public void Accept(IVisitor visitor) { visitor.Visit(this); }

	public MovementLogic(Rigidbody rb)
	{
		this.rb = rb;
		SetupStateMachine();
	}

	public static MovementLogic CreateMovementLogic(Rigidbody rb)
	{
		return new MovementLogic(rb);
	}

	// Change movement direction.
	public void MoveToDirection(Vector3 direction)
	{
		if (direction != Vector3.zero)
			facingDirection = direction;
		targetDirection = direction;
	}

	public void Dash(float power, float slideTime)
	{

		moveTrigger = MoveTrigger.Dash;
		dashState.UpdateParameters(power, slideTime);
	}

	public void Update()
	{


	}

	public void SetupStateMachine()
	{
		stateMachine = new StateMachine();
		void Af(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, new FunctionPredicate(condition));
		//void At(IState from, IState to, TriggerPredicate trigger) => stateMachine.AddTransition(from, to, trigger);

		MoveWalk walk = new MoveWalk(this, speed);
		MoveDash dash = new MoveDash(this, speed * DEFAULT_DASH_MULTIPLIER);

		Af(walk, dash, () => moveTrigger == MoveTrigger.Dash);
		Af(dash, walk, () => dash.Finished);

		stateMachine.SetState(walk);
	}
}
