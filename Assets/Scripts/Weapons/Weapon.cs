using UnityEngine;

public class Weapon : MonoBehaviour
{
	public float LastAttackTime { get; set; } = -999;
	[SerializeField] private WeaponData data;
	private Attack[] attacks = new Attack[0];
	private Entity owner;
	private IWeaponLogic logic;
	private StateMachine stateMachine;
	private bool active = false;

	public void TakeOwnership(Entity owner)
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
		active = true;
		logic.ResetCooldown();
	}

	public void Deactivate()
	{
		if (logic == null) return;

		// foreach (Attack attack in attacks)
		// {
		// 	attack.Deactivate();
		// }
		active = false;
	}

	public void ActivateAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Activate();
	}

	public void DeactivateAttack(int index)
	{
		if (attacks.Length <= index)
			return;

		attacks[index].Deactivate();
	}

	public float GetCooldown()
	{
		return logic.GetCooldown();
	}

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

			Af(cooldown, charge, () => cooldown.Finished && active);
			Af(charge, combo, () => charge.Status == ComboState.Failed && !active);
			Af(charge, cooldown, () => (charge.Status == ComboState.Perfect || charge.Status == ComboState.Successful) && !active);
			Af(combo, cooldown, () => true);
		}
		else
		{
			Af(cooldown, combo, () => cooldown.Finished && active);
			Af(combo, cooldown, () => true);
		}


		stateMachine.SetState(cooldown);
	}

	void Update()
	{
		stateMachine.FrameUpdate();
	}

	void FixedUpdate()
	{
		stateMachine.PhysicsUpdate();
	}
}
