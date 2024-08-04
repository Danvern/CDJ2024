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
	Rigidbody GetRigidbody();

}

public class MovementLogic : IVisitable, IMovementLogic
{
	enum MoveTrigger { Walk, Dash, }
	public float Speed { get { return speed; } set { speed = Mathf.Max(0, value); } }
	private MoveTrigger moveTrigger = MoveTrigger.Walk;
	private Rigidbody rb;
	private Vector3 targetDirection = Vector3.zero;
	private float speed = 100;
	private StateMachine stateMachine;
	private IPredicate dashTrigger;

	public float GetSpeed() { return speed; }
	public void SetSpeed(float speed) { this.speed = speed; }
	public Vector3 GetTargetDirection() { return targetDirection; }
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
		targetDirection = direction;
	}

	public void Dash(float power, float slideTime)
	{

		moveTrigger = MoveTrigger.Dash;
	}

	public void Update(float deltaTime)
	{


	}

	public void SetupStateMachine()
	{
		stateMachine = new StateMachine();
		void Af(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, new FunctionPredicate(condition));
		//void At(IState from, IState to, TriggerPredicate trigger) => stateMachine.AddTransition(from, to, trigger);

		MoveWalk walk = new MoveWalk(this, speed);
		MoveDash dash = new MoveDash(this, speed);

		Af(walk, dash, () => moveTrigger == MoveTrigger.Dash);
		Af(dash, walk, () => dash.Finished);

		stateMachine.SetState(walk);
	}
}
