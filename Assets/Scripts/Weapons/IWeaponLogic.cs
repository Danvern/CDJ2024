using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponLogic
{
	bool IsAttackReady();

	float GetCooldown();

	void ResetCooldown();
}
