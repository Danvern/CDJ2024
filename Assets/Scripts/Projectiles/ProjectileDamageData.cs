using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDamageData", menuName = "GameplayDefinitions/Projectile/DamageData", order = 1)]
public class ProjectileDamageData : ScriptableObject
{
    public float DamageMax = 0;
    public float DamageMin = 1;
	public int Piercing = 0;
}
