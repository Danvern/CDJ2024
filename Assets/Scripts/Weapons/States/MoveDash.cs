using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveDash : IState
{
	public bool Finished { get; private set; } = true;
	private IMovementLogic movement;
	private float moveSpeed;
	private float previousSpeed;

	public MoveDash(IMovementLogic movement, float moveSpeed)
	{
		this.movement = movement;
		this.moveSpeed = moveSpeed;
	}

	public void OnEnter()
	{
		Finished = false;
		previousSpeed = movement.GetSpeed();
		movement.SetSpeed(moveSpeed);
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		Finished = true;
		movement.SetSpeed(previousSpeed);
	}

	public void FrameUpdate()
	{
		if (movement.GetRigidbody() == null)
			return;

		movement.GetRigidbody().AddForce(movement.GetTargetDirection() * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
	}

	public void PhysicsUpdate() { }


}