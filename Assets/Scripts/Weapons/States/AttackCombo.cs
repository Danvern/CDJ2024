using UnityEngine;

public enum ComboState {Perfect, Successful, Failed, Pending};
public class AttackCombo : IState
{
	public ComboState Status { get; private set; } = ComboState.Failed;
	Weapon weapon;
	Transform _soundOrigin;
	OwlCountdown _countdown;

	public AttackCombo(Weapon weapon)
	{
		this.weapon = weapon;
	}

	public void OnEnter()
	{

		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit() 
	{
		// Evaluate combo success and fire weapon
	}

	public void FrameUpdate()
	{
		_countdown.Update(Time.deltaTime);
	}

	public void PhysicsUpdate() {}

	private void FinishCycle()
	{
		//Finished = true;
	}

}