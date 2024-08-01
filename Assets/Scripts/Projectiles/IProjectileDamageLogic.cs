using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileDamageLogic
{
	public virtual float GetDamageMax() { return 0; }

	public virtual float GetDamageMin() { return 0; }

	public virtual float GetDamageRandom() { return GetDamageMin() + Random.value * (GetDamageMax() - GetDamageMin()); }

	public abstract bool DoesPierce(int resistance);

	public abstract void DecreasePierce(int resistance);
}
