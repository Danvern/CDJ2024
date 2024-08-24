using UnityEngine;

public static class OwlVector
{
	/// <summary>
	/// Sets any values of the Vector3
	/// </summary>
	public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
	}

	/// <summary>
	/// Adds to any values of the Vector3
	/// </summary>
	public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
	}

	public static Vector2 Add(this Vector2 vector, float? x = null, float? y = null)
	{
		return new Vector2(vector.x + (x ?? 0), vector.y + (y ?? 0));
	}

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
