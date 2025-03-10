public interface IProjectileDamageLogic : IVisitor
{
	public abstract float GetLifetime();
	public abstract float GetDamageMax();

	public abstract float GetDamageMin();

	public abstract float GetDamageRandom();

	public abstract bool DoesPierce(int resistance);

	public abstract void DecreasePierce(int resistance);

	public abstract float GetCollisionRadius();

	public abstract float GetCollisionArc();
	public abstract float GetKnockback();
	public abstract float GetKnockbackDelay();
}
