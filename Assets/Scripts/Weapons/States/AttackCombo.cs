using UnityEngine;

public enum ComboState {Perfect, Successful, Failed, Pending};
public class AttackCombo : IState
{
	public ComboState Status { get; private set; } = ComboState.Failed;
	private Weapon weapon;
	private int attackIndex;
	private float lastAttackTime;

	public AttackCombo(Weapon weapon, int attackIndex)
	{
		this.weapon = weapon;
		this.attackIndex = attackIndex;
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
	}

	public void PhysicsUpdate() {}
}