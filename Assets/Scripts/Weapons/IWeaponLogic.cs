using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponLogic
{
	void SetTrackedAttack(int index);

	bool IsAttackReady();

	float GetCooldown();
	float GetLastAttackTime();

	void ResetCooldown();
}
