using System;
using UnityEngine;

public class MoveDash : IState
{
	public bool Finished { get; private set; } = true;
	public Action Finish;
	private IMovementLogic movement;
	private float moveSpeed = 2f;
	private float slideTime = 1f;
	private float previousSpeed;
	private float startTime;

	public MoveDash(IMovementLogic movement, float moveSpeed)
	{
		this.movement = movement;
		this.moveSpeed = moveSpeed;
	}

	public void UpdateParameters(float power, float time)
	{
		moveSpeed = power;
		slideTime = time;
	}

	public void OnEnter()
	{
		Finished = false;
		previousSpeed = movement.GetSpeed();
		movement.SetSpeed(moveSpeed);
		movement.SetPhasing(true);
		startTime = Time.time;
		movement.GetRigidbody().AddForce(movement.GetFacingDirection() * moveSpeed, ForceMode2D.Impulse);
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		Finished = true;
		Finish?.Invoke();
		movement.SetSpeed(previousSpeed);
		movement.SetPhasing(false);
	}

	public void FrameUpdate()
	{
		if (Time.time - startTime > slideTime)
		{
			Finished = true;
			return;
		}
		if (movement.GetRigidbody() == null)
			return;

		if (movement.GetRigidbody().velocity.magnitude > moveSpeed * 3f)
			movement.GetRigidbody().AddForce(movement.GetRigidbody().velocity.normalized * (moveSpeed - movement.GetRigidbody().velocity.magnitude) * movement.GetRigidbody().mass, ForceMode2D.Force);

	}

	public void PhysicsUpdate() { }


}