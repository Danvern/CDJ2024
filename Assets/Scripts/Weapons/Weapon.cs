using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
// using Blackboard; No do a mediator

public class Weapon : MonoBehaviour
{
	[SerializeField] private WeaponData data;
	private Attack[] attacks = new Attack[0];
	private EntityMediator owner;
	private IWeaponLogic logic;
	private StateMachine stateMachine;
	private bool triggerPressed = false;
	private string animationTag = "IsAttacking";

	public EntityMediator GetOwner() => owner;

	public void TakeOwnership(EntityMediator owner)
	{
		this.owner = owner;
		foreach (Attack attack in attacks)
		{
			attack.TakeOwnership(owner);
		}
	}

	public void Activate()
	{
		if (logic == null) return;
		if (!logic.IsAttackReady()) return;

		// foreach (Attack attack in attacks)
		// {
		// 	attack.Activate();
		// }
		if (data.FireDelay > 0)
		{

			GetOwner().SetAnimationBool(GetAnimationTag()+"Started", true);
			StartCoroutine(TriggerAfterDelay(true, data.FireDelay));

		}
		else
			triggerPressed = true;
		logic.ResetCooldown();
	}

	public void Deactivate()
	{
		if (logic == null) return;

		// foreach (Attack attack in attacks)
		// {
		// 	attack.Deactivate();
		// }
		if (data.FireDelay > 0)
		{
			GetOwner().SetAnimationBool(GetAnimationTag()+"Started", false);
			StartCoroutine(TriggerAfterDelay(false, data.FireDelay));

		}
		else
			triggerPressed = false;
	}

	IEnumerator TriggerAfterDelay(bool pressed, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		triggerPressed = pressed;
	}

	public void ResetCooldown()
	{
		logic.ResetCooldown();
	}
	public float GetCooldown()
	{
		return logic.GetCooldown();
	}
	public string GetAnimationTag() => animationTag;
	public void SetAnimationTag(string value) => animationTag = value;

	public void ActivateAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Activate();
		GetOwner().SetAnimationBool(GetAnimationTag(), true);
	}

	public void DeactivateAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Deactivate();
		GetOwner().SetAnimationBool(GetAnimationTag(), false);
	}
	public float GetLastAttackTime() => logic.GetLastAttackTime();
	public void UpdateTrackedAttack(int index)
	{
		logic.SetTrackedAttack(index);
	}

	void Awake()
	{
		attacks = GetComponentsInChildren<Attack>();
		logic = new WeaponLogic(data);

		stateMachine = new StateMachine();
		void Af(IState from, IState to, System.Func<bool> condition) => stateMachine.AddTransition(from, to, new FunctionPredicate(condition));
		//void At(IState from, IState to, TriggerPredicate trigger) => stateMachine.AddTransition(from, to, trigger);

		AttackCooldown cooldown = new AttackCooldown(this, logic.GetCooldown());
		AttackCombo combo = new AttackCombo(this, data.GetComboDefinitions(), data.MaxCombo);

		AttackDefinition[] chargeDefinitions = data.GetChargeDefinitions();
		if (chargeDefinitions.Length > 0) //TODO: Make this more versatile
		{
			AttackHold charge = new AttackHold(this, chargeDefinitions[0], data.MaxCombo);

			Af(cooldown, charge, () => cooldown.Finished && triggerPressed);
			Af(charge, cooldown, () => (charge.Status == ComboState.Perfect || charge.Status == ComboState.Successful) && !triggerPressed);
			Af(charge, combo, () => charge.Status == ComboState.Failed && !triggerPressed);
			Af(combo, cooldown, () => true);
		}
		else
		{
			Af(cooldown, combo, () => cooldown.Finished && triggerPressed);
			Af(combo, cooldown, () => true);
		}


		stateMachine.SetState(cooldown);
	}

	void Update()
	{
		stateMachine.Update();
	}

	void FixedUpdate()
	{
		stateMachine.PhysicsUpdate();
	}
}
