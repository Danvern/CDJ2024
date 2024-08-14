public interface IEntityHealthLogic
{
	public abstract float GetHealthMax();

	public abstract float GetHealthCurrent();

	public abstract void DoDamage(float damage);

	public abstract void DoDamage(float damage, ProjectileBase source);
}
