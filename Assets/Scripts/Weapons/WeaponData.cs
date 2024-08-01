using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameplayDefinitions/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
	public float Cooldown = 1;
}
