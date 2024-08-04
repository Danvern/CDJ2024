using UnityEngine;

[CreateAssetMenu(fileName = "DashEffect", menuName = "GameplayDefinitions/AttackEffect/DashEffect", order = 1)]
public class DashEffectData : ScriptableObject, IAttackEffectData
{
	[SerializeField] float power;
	[SerializeField] float slideTime;
	[SerializeField] bool controlled = true;

	public IAttackEffect CreateEffect()
	{
		DashEffect effect = new DashEffect(power, slideTime, controlled);
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