using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
	[SerializeField] ProjectileDamageData projectileDamageData;
	ProjectileDamageLogic damageLogic;
	bool active = true;
	float deletionDelay = 1f;
	private Entity owner;

	public void TakeOwnership(Entity owner)
	{
		this.owner = owner;

	}

    // Start is called before the first frame update
    void Start()
    {
        damageLogic = new ProjectileDamageLogic(projectileDamageData);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!active) return;

        if (damageLogic.CheckCollisons(transform.position, owner))
		{
			Kill();
		}
    }

	private void Kill()
	{
		if (!active) return;

		active = false;
		StartCoroutine(DestroyAfterDelay(deletionDelay));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
	}
}
