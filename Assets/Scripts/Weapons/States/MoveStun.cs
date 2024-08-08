using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class MoveStun : IState
{
	public bool Finished { get; private set; } = true;
	public Action Finish;
	private IMovementLogic movement;
	private float moveSpeed = 2f;
	private float slideTime = 1f;
	private float previousSpeed;
	private float startTime;
	private Vector3 stunnedDirection;

	public MoveStun(IMovementLogic movement, float moveSpeed)
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
		stunnedDirection = movement.GetFacingDirection();
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

		movement.GetRigidbody().AddForce(stunnedDirection * moveSpeed * Time.deltaTime, ForceMode2D.Force);
	}

	public void PhysicsUpdate() { }


}