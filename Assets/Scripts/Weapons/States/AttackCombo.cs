using UnityEngine;

public enum ComboState { Perfect, Successful, Failed };
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

	public AttackCombo(Weapon weapon, IComboDefinition[] comboData)
	{
		this.weapon = weapon;
		this.comboData = comboData;
	}

	public void OnEnter()
	{
		if ((Time.time - weapon.LastAttackTime) == comboData[ComboStage].GetIdealTiming())
			Status = ComboState.Perfect;
		else if (Mathf.Abs(Time.time - weapon.LastAttackTime - comboData[ComboStage].GetIdealTiming()) <= comboData[ComboStage].GetIdealTimingWindow())
			Status = ComboState.Successful;
		else
			Status = ComboState.Failed;

		weapon.ActivateAttack(comboData[ComboStage].GetIndex());
		
		//AudioManager.Instance.PlayOneShot(!_gun.GunCycle.IsNull ? _gun.GunCycle : FModEvents.Instance.GunshotGenericCycle, _soundOrigin.position);
	}

	public void OnExit()
	{
		weapon.LastAttackTime = Time.time;

		weapon.DeactivateAttack(comboData[ComboStage].GetIndex());
		weapon.UpdateTrackedAttack(comboData[ComboStage].GetIndex());

		// Evaluate combo success
		if (Status == ComboState.Successful || Status == ComboState.Perfect)
		{
			ComboStage = (ComboStage + 1) % comboData.Length;
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