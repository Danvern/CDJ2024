using UnityEngine;
using UnityEngine.PlayerLoop;

public enum ComboState { Perfect, Successful, Failed, Pending };
public interface IComboDefinition
{
	public float GetCooldown();
	public float GetIdealTiming();
	public float GetIdealTimingWindow();
	public int GetIndex();
}

public class AttackCombo : IState
{
	public ComboState Status { get; private set; } = ComboState.Failed;
	public int ComboStage { get; private set; } = 0;
	private Weapon weapon;
	private IComboDefinition[] comboData;
	private int maxCombo = 0;

	public AttackCombo(Weapon weapon, IComboDefinition[] comboData, int maxCombo)
	{
		this.weapon = weapon;
		this.comboData = comboData;
		this.maxCombo = maxCombo;
	}

	public void OnEnter()
	{
		Status = ComboState.Pending;
		UpdateCombo();
		// Debug.Log("Activated Attack Timing: " + (Time.time - weapon.GetLastAttackTime()) + "s");

		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		weapon.ResetCooldown();

		weapon.DeactivateAttack(comboData[ComboStage].GetIndex());
	}

	public void FrameUpdate()
	{
	}

	public void PhysicsUpdate() { }

	private void UpdateCombo()
	{
		if ((Time.time - weapon.GetLastAttackTime()) == comboData[ComboStage].GetIdealTiming())
			Status = ComboState.Perfect;
		else if (Mathf.Abs(Time.time - weapon.GetLastAttackTime() - comboData[ComboStage].GetIdealTiming()) <= comboData[ComboStage].GetIdealTimingWindow())
			Status = ComboState.Successful;
		else
			Status = ComboState.Failed;

		// Evaluate combo success
		if (Status == ComboState.Successful || Status == ComboState.Perfect)
		{
			ComboStage = (ComboStage + 1) % Mathf.Min(comboData.Length, maxCombo);
		}
		else if (Status == ComboState.Failed)
		{
			ComboStage = 0;
		}
		weapon.GetOwner().SetAnimationInt("ComboStage", ComboStage);
		weapon.ActivateAttack(comboData[ComboStage].GetIndex());
		weapon.UpdateTrackedAttack(comboData[ComboStage].GetIndex());
	}
}