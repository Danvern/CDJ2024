using UnityEngine;

public class OwlVector : MonoBehaviour
{
	public static void LimitRigidBodyVelocity(Rigidbody rb, float maxVelocity)
	{
		float overspeed = maxVelocity - rb.velocity.magnitude;
		if (overspeed < 0f)
		{
			rb.AddForce(rb.velocity.normalized * overspeed, ForceMode.VelocityChange);
		}
	}

	public static Vector3 PositionAroundPivot(Vector3 pivot, Quaternion rotation, float distance = 0, float angle = 0)
	{
		return pivot + Quaternion.Euler(0, angle, 0) * rotation * Vector3.forward * distance;
	}
}
