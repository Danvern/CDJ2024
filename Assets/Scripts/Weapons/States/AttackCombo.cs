using UnityEngine;

public enum ComboState { Perfect, Successful, Failed };
public class AttackCombo : IState
{
	public ComboState Status { get; private set; } = ComboState.Failed;
	public int ComboStage { get; private set; } = 0;
	private Weapon weapon;
	private int[] attackIndices;
	private float idealTiming;
	private float idealTimingWindow;

	public AttackCombo(Weapon weapon, int[] attackIndices, float idealTiming, float idealTimingWindow)
	{
		this.weapon = weapon;
		this.attackIndices = attackIndices;
		this.idealTiming = idealTiming;
		this.idealTimingWindow = idealTimingWindow;
	}

	public void OnEnter()
	{
		if ((Time.time - weapon.LastAttackTime) == idealTiming)
			Status = ComboState.Perfect;
		else if (Mathf.Abs(Time.time - weapon.LastAttackTime - idealTiming) <= idealTimingWindow)
			Status = ComboState.Successful;
		else
			Status = ComboState.Failed;

		weapon.ActivateAttack(attackIndices[ComboStage]);
		
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		weapon.LastAttackTime = Time.time;

		weapon.DeactivateAttack(attackIndices[ComboStage]);

		// Evaluate combo success
		if (Status == ComboState.Successful || Status == ComboState.Perfect)
		{
			ComboStage = (ComboStage + 1) % attackIndices.Length;
		}
		else if (Status == ComboState.Failed)
		{
			ComboStage = 0;
		}
	}

	public void FrameUpdate()
	{
	}

	public void PhysicsUpdate() { }
}