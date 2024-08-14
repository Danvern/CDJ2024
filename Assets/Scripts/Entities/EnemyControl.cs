using UnityEngine;

public class EnemyControl : MonoBehaviour
{
	private Vector3 moveDirection = Vector2.zero;
	private Entity entity;
	private Vector3 aimPosition = Vector3.zero;
	private Entity targetPlayer;

	private void Awake()
	{
		entity = GetComponent<Entity>();
	}

	/// <summary>
	/// Update the position of the targeting cursor as well as the hovered target.
	/// </summary>
	/// <returns>
	/// Returns the target GameObject which was hovered.
	/// </returns>
	private GameObject TargetCursor()
	{
		if (targetPlayer == null)
		{
			GameObject potentialPlayer = GameObject.FindWithTag("Player");
			if (potentialPlayer != null)
				targetPlayer = potentialPlayer.GetComponent<Entity>();
		}
		else if (targetPlayer.IsDead)
		{
			targetPlayer = null;
		}
		else
		{
			moveDirection = new Vector3(targetPlayer.transform.position.x - transform.position.x, 0, targetPlayer.transform.position.z - transform.position.z);
			aimPosition = targetPlayer.transform.position;
			return targetPlayer.gameObject;
		}

		return null;
	}

	private void Update()
	{
		if (entity == null)
			return;

		entity.MoveToDirection(new Vector3(moveDirection.x, 0, moveDirection.z));
		TargetCursor();
		entity.FacePosition(aimPosition);
		entity.PrimaryFire(true);
	}

}
