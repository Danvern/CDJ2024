using UnityEngine;
using UnityEngine.PlayerLoop;

public class AttackHold : IState
{
	public ComboState Status { get; private set; } = ComboState.Failed;
	private Weapon weapon;
	private IComboDefinition comboData;
	private float chargeStartTime;

	public AttackHold(Weapon weapon, IComboDefinition comboData, int maxCombo)
	{
		this.weapon = weapon;
		this.comboData = comboData;
	}

	public void OnEnter()
	{
		Status = ComboState.Pending;
		chargeStartTime = Time.time;

		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		// Evaluate combo success
		if (Status == ComboState.Successful || Status == ComboState.Perfect)
		{
			weapon.ActivateAttack(comboData.GetIndex());
			weapon.UpdateTrackedAttack(comboData.GetIndex());
			weapon.LastAttackTime = Time.time;
			weapon.DeactivateAttack(comboData.GetIndex());
			//Debug.Log("Activated Hold Attack Timing: " + (Time.time - chargeStartTime) + "s");
		}
		else if (Status == ComboState.Failed)
		{
			//Debug.Log("Failed Hold Attack Timing: " + (Time.time - chargeStartTime) + "s");
		}
	}

	public void FrameUpdate()
	{
		EvaluateCharge();
	}

	public void PhysicsUpdate() { }

	private void EvaluateCharge()
	{
		if ((Time.time - chargeStartTime) == comboData.GetIdealTiming())
			Status = ComboState.Perfect;
		else if (Mathf.Abs(Time.time - chargeStartTime - comboData.GetIdealTiming()) <= comboData.GetIdealTimingWindow())
			Status = ComboState.Successful;
		else
			Status = ComboState.Failed;		
	}
}