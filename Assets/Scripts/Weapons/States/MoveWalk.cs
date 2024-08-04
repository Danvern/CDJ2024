using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveWalk : IState
{
	private IMovementLogic movement;
	private float moveSpeed;
	private float acceleration;
	private float previousSpeed;

	public MoveWalk(IMovementLogic movement, float moveSpeed, float acceleration = 100f)
	{
		this.movement = movement;
		this.moveSpeed = moveSpeed;
		this.acceleration = acceleration;
	}

	public void OnEnter()
	{
		previousSpeed = movement.GetSpeed();
		movement.SetSpeed(moveSpeed);
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		movement.SetSpeed(previousSpeed);
	}

	public void FrameUpdate()
	{
		if (movement.GetRigidbody() == null)
			return;

		movement.GetRigidbody().velocity = movement.GetTargetDirection() * moveSpeed; // Sharp Turns
		movement.GetRigidbody().AddForce(movement.GetTargetDirection() * acceleration * Time.deltaTime, ForceMode.VelocityChange);
		if (movement.GetRigidbody().velocity.magnitude > moveSpeed)
			movement.GetRigidbody().velocity = movement.GetRigidbody().velocity.normalized * moveSpeed;
	}

	public void PhysicsUpdate() { }


}