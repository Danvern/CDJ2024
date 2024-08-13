public interface IAmmunitionSource
{
	int GetAmmo(AmmoType type);

	int GetAmmoMax(AmmoType type);
	void AddAmmo(AmmoType type, int amount, float maxMultiplier = 1f);
}
