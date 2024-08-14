public class DashEffect : IAttackEffect
{
	float power;
	float slideTime;
	bool controlled = true;
	bool invulnerable = true;
	float offBeatPenalty = 0.5f; //TODO: expose these via the factory
	float offBeatPenaltyTime = 0.65f;

	public DashEffect(float power, float slideTime, bool controlled, bool invulnerable)
	{
		this.power = power;
		this.slideTime = slideTime;
		this.controlled = controlled;
		this.invulnerable = invulnerable;
	}

	public void Activate(EntityMediator owner)
	{
		if (AudioManager.Instance.IsOnBeat())
			owner.DashToAim(power, slideTime, invulnerable);
		else
			owner.DashToAim(power * offBeatPenalty, slideTime * offBeatPenaltyTime, false);
	}

	public void Deactivate(EntityMediator owner)
	{
	}
}
