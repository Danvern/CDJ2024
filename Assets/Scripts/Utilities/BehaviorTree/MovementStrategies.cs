using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public interface IMovementStrategy
{
	public Vector3 GetDirection(Vector3 position, Vector3 targetPosition);
}
public class MovementStrategyBackwards : IMovementStrategy
{
	public Vector3 GetDirection(Vector3 position, Vector3 targetPosition)
	{
		return position - targetPosition;
	}
}
public class MovementStrategyBackwardsRandom : IMovementStrategy
{
	int maxRays = 7;
	float angleRange = 170f;

	public Vector3 GetDirection(Vector3 position, Vector3 targetPosition)
	{
			Vector3[] directions = new Vector3[maxRays];
		for (int i = 0; i < (maxRays - 1); i++)
		{
			directions[i] = Quaternion.Euler(x: 0, y: 0, z: angleRange / 2f - angleRange / maxRays * i) * targetPosition; 
		}
		directions.Shuffle();
		for (int i = 0; i < (maxRays - 1); i++)
		{
			if (IsDirectionClear(position, directions[i]))
				return directions[i];
		}
		return directions[maxRays / 2];
	}

	bool IsDirectionClear(Vector3 position, Vector3 targetDirection)
	{
		return !Physics2D.Raycast(position, targetDirection, 3f, layerMask: LayerMask.GetMask("EnvironmentObstacles"));
	}
}