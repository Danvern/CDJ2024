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
		delay = weapon.GetCooldown();
		Finished = false;
		countdown = new OwlCountdown(delay);
		countdown.Expired += FinishCycle;
	}

	public void OnExit() 
	{
		Finished = true;
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