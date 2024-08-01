using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

public class ProjectileBase : MonoBehaviour
{
	[SerializeField] ProjectileDamageData projectileDamageData;
	ProjectileDamageLogic damageLogic;
	private bool active = true;
	private float timeCreated;
	private Entity owner;
	private Transform anchor;
	private const float deletionDelay = 1f;

	public void TakeOwnership(Entity owner)
	{
		this.owner = owner;
	}

	public void TrackTransform(Transform anchor)
	{
		this.anchor = anchor;
	}

    // Start is called before the first frame update
    void Start()
    {
        damageLogic = new ProjectileDamageLogic(projectileDamageData);
		timeCreated = Time.time;
    }

	void Update()
	{
		if (anchor != null)
			transform.position = anchor.position;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!active) return;

        if (damageLogic.CheckCollisons(transform.position, owner))
		{
			Kill();
		}

		if (Time.time - timeCreated > damageLogic.GetLifetime())
		{
			Kill();
		}
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		float rayLength = 3;
		float rayArc = 90;
		if (damageLogic != null)
		{
			rayLength = damageLogic.GetCollisionRadius();
			rayArc = damageLogic.GetCollisionArc();
		}
		Gizmos.DrawRay(transform.position, transform.forward * rayLength);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, rayArc / 2, 0) * transform.rotation * Vector3.forward * rayLength);
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -rayArc / 2, 0) * transform.rotation * Vector3.forward * rayLength);

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
		Destroy(gameObject);
	}
}
