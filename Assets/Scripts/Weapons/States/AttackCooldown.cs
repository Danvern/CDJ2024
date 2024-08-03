using UnityEngine;

public class AttackCooldown : IState
{
	public bool Finished { get; private set; } = false;
	private OwlCountdown countdown;
	private Weapon weapon;
	private float delay;

	public AttackCooldown(Weapon weapon, float delay)
	{
		this.weapon = weapon;
		this.delay = delay;
	}

	public void OnEnter()
	{
		Finished = false;
		countdown = new OwlCountdown(delay);
		countdown.Expired += FinishCycle;
	}

	public void OnExit() 
	{
		Finished = false;
		countdown.Expired -= FinishCycle;
		countdown = null;
	}

	public void FrameUpdate()
	{
		countdown.Update(Time.deltaTime);
	}

	public void PhysicsUpdate() {}

	private void FinishCycle()
	{
		Finished = true;
	}
}