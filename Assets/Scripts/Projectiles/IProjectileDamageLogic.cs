using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileDamageLogic : IVisitor
{
	public abstract float GetDamageMax();

	public abstract float GetDamageMin();

	public abstract float GetDamageRandom();

	public abstract bool DoesPierce(int resistance);

	public abstract void DecreasePierce(int resistance);

	public abstract float GetCollisionRadius();

	public abstract float GetCollisionArc();
}
