using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveWalk : IState
{
	private IMovementLogic movement;
	private float moveSpeed;
	private float previousSpeed;

	public MoveWalk(IMovementLogic movement, float moveSpeed)
	{
		this.movement = movement;
		this.moveSpeed = moveSpeed;
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

		movement.GetRigidbody().velocity = Vector3.zero;
		movement.GetRigidbody().AddForce(movement.GetTargetDirection() * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
	}

	public void PhysicsUpdate() { }


}