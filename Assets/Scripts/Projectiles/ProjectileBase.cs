using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using Debug = UnityEngine.Debug;

public class ProjectileBase : MonoBehaviour
{
	[SerializeField] ProjectileDamageData projectileDamageData;
	ProjectileDamageLogic damageLogic;
	private bool active = true;
	private float timeCreated;
	private EntityMediator owner;
	private Transform anchor;
	private Rigidbody2D rb;
	private const float deletionDelay = 1f;

	public void TakeOwnership(EntityMediator owner)
	{
		this.owner = owner;
		if (owner == null)
			Debug.LogWarning("unowned projectile generated somehow");
	}

	public void TrackTransform(Transform anchor)
	{
		this.anchor = anchor;
	}

	public void Propell(Vector3 velocity)
	{
		if (rb == null)
			return;
		rb.AddForce(velocity, ForceMode2D.Impulse);
	}

	// Start is called before the first frame update
	void Start()
	{
		damageLogic = new ProjectileDamageLogic(projectileDamageData);
		timeCreated = Time.time;
		rb = GetComponent<Rigidbody2D>();
		Propell(transform.up * damageLogic.GetSpeed());
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

		if (damageLogic.CheckCollisons(transform, owner))
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
		Gizmos.DrawRay(transform.position, transform.up * rayLength);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, rayArc / 2) * transform.rotation * Vector3.up * rayLength);
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, -rayArc / 2) * transform.rotation * Vector3.up * rayLength);

	}

	private void Kill()
	{
		if (!active) return;

		active = false;
		if (rb != null)
			rb.velocity = Vector3.zero;
		foreach (SpriteRenderer item in GetComponentsInChildren<SpriteRenderer>())
		{
			item.color = Color.clear;
		}
		StartCoroutine(DestroyAfterDelay(deletionDelay));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
