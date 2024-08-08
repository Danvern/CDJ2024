using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveWalk : IState
{
	private IMovementLogic movement;
	private float previousSpeed;
	private float stoppingDistance = 0.25f;

	public MoveWalk(IMovementLogic movement)
	{
		this.movement = movement;
	}

	public void OnEnter()
	{
		previousSpeed = movement.GetSpeed();
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

		if (movement.IsFollowingPath())
			movement.MoveToDirection(movement.GetNextPathNode(movement.GetSpeed() * stoppingDistance));
		movement.GetRigidbody().velocity = movement.GetTargetDirection() * movement.GetRigidbody().velocity.magnitude; // Sharp Turns

		if (movement.GetTargetDirection() != Vector3.zero)
			movement.GetRigidbody().AddForce(movement.GetTargetDirection() * movement.GetAcceleration() * Time.deltaTime, ForceMode2D.Force);
		if (movement.GetRigidbody().velocity.magnitude > movement.GetSpeed())
			movement.GetRigidbody().velocity = movement.GetRigidbody().velocity.normalized * movement.GetSpeed();
	}

	public void PhysicsUpdate() { }


}