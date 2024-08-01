using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityHealthLogic
{
	public abstract float GetHealthMax();

	public abstract float GetHealthCurrent();

	public abstract void DoDamage(float damage);

	public virtual string ToString() { return "" + GetHealthCurrent() + "/" + GetHealthMax(); }
}
