public class DashEffect : IAttackEffect
{
	float power;
	float slideTime;
	bool controlled = true;
	bool invulnerable = true;
	float offBeatPenalty = 0.5f; //TODO: expose these via the factory
	float offBeatTimePenalty = 0.65f;

	private DashEffect()
	{
	}

	public void Activate(EntityMediator owner)
	{
		if (AudioManager.Instance.IsOnBeat())
			owner.DashToAim(power, slideTime, invulnerable);
		else
			owner.DashToAim(power * offBeatPenalty, slideTime * offBeatTimePenalty, false);
	}

	public void Deactivate(EntityMediator owner)
	{
	}

	public class Builder
    {
        private float _power;
        private float _slideTime;
        private bool _controlled = true;
        private bool _invulnerable = true;
        private float _offBeatPenalty = 0.5f;
        private float _offBeatTimePenalty = 0.65f;

        public Builder WithPower(float power)
        {
            _power = power;
            return this;
        }

        public Builder WithSlideTime(float slideTime)
        {
            _slideTime = slideTime;
            return this;
        }

        public Builder WithControlled(bool controlled)
        {
            _controlled = controlled;
            return this;
        }

        public Builder WithInvulnerable(bool invulnerable)
        {
            _invulnerable = invulnerable;
            return this;
        }

        public Builder WithOffBeatPenalty(float offBeatPenalty)
        {
            _offBeatPenalty = offBeatPenalty;
            return this;
        }

        public Builder WithOffBeatTimePenalty(float offBeatTimePenalty)
        {
            _offBeatTimePenalty = offBeatTimePenalty;
            return this;
        }

        public DashEffect Build()
        {
            return new DashEffect
            {
                power = _power,
                slideTime = _slideTime,
                controlled = _controlled,
                invulnerable = _invulnerable,
                offBeatPenalty = _offBeatPenalty,
                offBeatTimePenalty = _offBeatTimePenalty
            };
        }
    }


}
