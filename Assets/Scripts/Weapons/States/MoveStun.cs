using System;
using UnityEngine;

public class MoveStun : IState
{
	public bool Finished { get; private set; } = true;
	public Action Finish;
	private IMovementLogic movement;
	private float moveSpeed = 2f;
	private float slideTime = 1f;
	private float previousSpeed;
	private float startTime;
	private float friction = 0.29f;
	private Vector3 stunnedDirection;

	public MoveStun(IMovementLogic movement, float moveSpeed)
	{
		this.movement = movement;
		this.moveSpeed = moveSpeed;
	}

	public void UpdateParameters(float power, float time, Vector3 direction)
	{
		moveSpeed = power;
		slideTime = time;
		stunnedDirection = direction;
		movement.GetRigidbody().AddForce(stunnedDirection * power, ForceMode2D.Impulse);
		//Debug.Log(movement.GetRigidbody().velocity.magnitude);
	}

	public void OnEnter()
	{
		Finished = false;
		previousSpeed = movement.GetSpeed();
		movement.SetSpeed(moveSpeed);
		startTime = Time.time;
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		if (!Finished)
			Finish?.Invoke();
		Finished = true;
		movement.SetSpeed(previousSpeed);
	}

	public void FrameUpdate()
	{
		if (Time.time - startTime > slideTime)
		{
			Finished = true;
			Finish?.Invoke();
			return;
		}
		if (movement.GetRigidbody() == null)
			return;

		movement.GetRigidbody().AddForce(movement.GetRigidbody().velocity.normalized * (-movement.GetRigidbody().velocity.magnitude) * movement.GetSpeed() * friction, ForceMode2D.Force);

		movement.GetRigidbody().AddForce(stunnedDirection * moveSpeed * Time.deltaTime, ForceMode2D.Force);
	}

	public void PhysicsUpdate() { }


}