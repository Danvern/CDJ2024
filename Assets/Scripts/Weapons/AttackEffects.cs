using UnityEngine;

[CreateAssetMenu(fileName = "DashEffect", menuName = "GameplayDefinitions/AttackEffect/DashEffect", order = 1)]
public class DashEffectData : ScriptableObject, IAttackEffectData
{
	[SerializeField] float power;
	[SerializeField] float slideTime;
	[SerializeField] float offBeatPenalty;
	[SerializeField] float offBeatTimePenalty;
	[SerializeField] bool controlled = true;
	[SerializeField] bool invulnerable = false;

	public IAttackEffect CreateEffect()
	{
		DashEffect effect = new DashEffect.Builder()
		.WithPower(power)
		.WithSlideTime(slideTime)
		.WithOffBeatPenalty(offBeatPenalty)
		.WithOffBeatTimePenalty(offBeatTimePenalty)
		.WithInvulnerable(invulnerable)
		.WithControlled(controlled)
		.Build();
		return effect;
	}
}

public interface IAttackEffectData
{
	public IAttackEffect CreateEffect();

}

// public class AttackEffectBuilder
// {
// 	public IAttackEffect BuildAttackEffect(DashEffectData dashEffectData)
// 	{
// 		DashEffect effect = new DashEffect(dashEffectData.power, dashEffectData.slideTime, dashEffectData.controlled);
// 		return effect;
// 	}

// }