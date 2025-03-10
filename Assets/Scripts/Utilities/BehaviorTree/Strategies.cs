using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinding.BehaviourTrees
{
	public interface IStrategy
	{
		Node.Status Process();

		void Reset()
		{
			// Noop
		}
	}

	public class ActionStrategy : IStrategy
	{
		readonly Action doSomething;

		public ActionStrategy(Action doSomething)
		{
			this.doSomething = doSomething;
		}

		public Node.Status Process()
		{
			doSomething();
			return Node.Status.Success;
		}
	}

	public class Condition : IStrategy
	{
		readonly Func<bool> predicate;

		public Condition(Func<bool> predicate)
		{
			this.predicate = predicate;
		}

		public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
	}

	public class PatrolStrategy : IStrategy
	{
		readonly EntityMediator entity;
		readonly List<Transform> patrolPoints;
		readonly float patrolSpeed;
		int currentIndex;
		bool isPathCalculated;

		public PatrolStrategy(EntityMediator entity, List<Transform> patrolPoints, float patrolSpeed = 2f)
		{
			this.entity = entity;
			this.patrolPoints = patrolPoints;
			this.patrolSpeed = patrolSpeed;
		}

		public Node.Status Process()
		{
			if (currentIndex == patrolPoints.Count) return Node.Status.Success;

			var target = patrolPoints[currentIndex];
			entity.NavigatePathTo(target.position);

			if (isPathCalculated && entity.GetRemainingTravelDistance() < 0.1f)
			{
				currentIndex++;
				isPathCalculated = false;
			}

			isPathCalculated = entity.IsNavigating();

			return Node.Status.Running;
		}

		public void Reset() => currentIndex = 0;
	}

	public class RandomPatrolStrategy : IStrategy
	{
		float lastCalcTime = 0;
		readonly EntityMediator entity;
		readonly List<Transform> patrolPoints;
		readonly float patrolRadius;
		readonly float boredTime = 3f;
		int currentIndex;
		bool isPathCalculated;

		public RandomPatrolStrategy(EntityMediator entity, float patrolRadius = 2f)
		{
			this.entity = entity;
			this.patrolRadius = patrolRadius;
		}

		public Node.Status Process()
		{
			if (!isPathCalculated || Time.time - lastCalcTime > boredTime)
			{
				if (currentIndex > 0)
					return Node.Status.Success;
				entity.NavigatePathTo(entity.GetTransform().position.Add(x: Random.Range(-patrolRadius, patrolRadius), y: Random.Range(-patrolRadius, patrolRadius)));
				lastCalcTime = Time.time;
				currentIndex++;
			}

			if (!entity.IsNavigatorActive() && entity.GetRemainingTravelDistance() < entity.GetWaypointCloseness())
			{
				return Node.Status.Success;
			}

			isPathCalculated = entity.IsNavigatorActive();

			return Node.Status.Running;
		}

		public void Reset()
		{
			currentIndex = 0;
		}
	}
	public class WaitStrategy : IStrategy
	{
		float lastCalcTime = 0;
		float waitingTime;
		bool isWaiting = false;
		readonly EntityMediator entity;

		public WaitStrategy(float waitTime = 1f)
		{
			this.waitingTime = waitTime;
		}

		public Node.Status Process()
		{
			if (!isWaiting)
			{

				lastCalcTime = Time.time;
				isWaiting = true;
			}
			if (Time.time - lastCalcTime > waitingTime)
			{
				return Node.Status.Success;
			}
			return Node.Status.Running;
		}

		public void Reset()
		{
			isWaiting = false;
		}
	}

	public class ChargeToTarget : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Vector2> target;
		bool isLookingForward;
		float clearDistance = 1f;
		bool interruptable;
		Vector2? initialDirection;

		public ChargeToTarget(EntityMediator entity, Func<Vector2> target, bool isLookingForward = true, bool interruptable = true)
		{
			this.entity = entity;
			this.target = target;
			this.isLookingForward = isLookingForward;
			this.interruptable = interruptable;
		}

		public Node.Status Process()
		{
			if (entity.IsNavigating())
				entity.CancelPath();
			if (initialDirection == null)
				initialDirection = (target() - entity.GetPosition()).normalized;
			bool clearPath = !Physics2D.Raycast(entity.GetPosition(), direction: initialDirection ?? Vector2.right, distance: clearDistance, layerMask: LayerMask.GetMask("EnviromentObstacles"));
			// if (Vector3.Distance(entity.GetPosition(), target()) < 1f)
			// {
			// 	return Node.Status.Success;
			// }
			if (!clearPath)
			{
				return Node.Status.Failure;
			}

			entity.MoveToDirection(initialDirection ?? Vector2.zero);
			if (isLookingForward)
				entity.FacePosition(initialDirection ?? Vector2.zero);

			// if (interruptable)
			// 	return Node.Status.Success;
			// else
			return Node.Status.Running;
		}

		public void Reset() { initialDirection = null; }
	}

	public class DashFromTarget : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Vector3> target;
		bool isLookingForward;
		bool interruptable;
		float power;
		float duration;
		IMovementStrategy movement;

		public DashFromTarget(EntityMediator entity, Func<Vector3> target, float power, float duration, IMovementStrategy movement, bool isLookingForward = false, bool interruptable = true)
		{
			this.entity = entity;
			this.target = target;
			this.isLookingForward = isLookingForward;
			this.interruptable = interruptable;
			this.power = power;
			this.duration = duration;
			this.movement = movement;
		}

		public Node.Status Process()
		{
			if (entity.IsNavigating())
				entity.CancelPath();
			if (Vector3.Distance(entity.GetTransform().position, target()) < 1f)
			{
				return Node.Status.Success;
			}

			entity.DashToDirection(GetDirection(entity.GetTransform().position, target()), power, duration);
			if (isLookingForward)
				entity.FacePosition(target() - entity.GetTransform().position);

			if (interruptable)
				return Node.Status.Success;
			else
				return Node.Status.Running;
		}

		private Vector3 GetDirection(Vector3 position, Vector3 targetPosition)
		{
			return movement.GetDirection(position, targetPosition);
		}

		public void Reset() { }
	}
	public class MoveToTarget : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Vector3> target;
		bool isLookingForward;
		bool interruptable;

		public MoveToTarget(EntityMediator entity, Func<Vector3> target, bool isLookingForward = true, bool interruptable = true)
		{
			this.entity = entity;
			this.target = target;
			this.isLookingForward = isLookingForward;
			this.interruptable = interruptable;
		}

		public Node.Status Process()
		{
			if (entity.IsNavigating())
				entity.CancelPath();
			if (Vector3.Distance(entity.GetTransform().position, target()) < 1f)
			{
				return Node.Status.Success;
			}

			entity.MoveToDirection(target() - entity.GetTransform().position);
			if (isLookingForward)
				entity.FacePosition(target() - entity.GetTransform().position);

			if (interruptable)
				return Node.Status.Success;
			else
				return Node.Status.Running;
		}

		public void Reset() { }
	}

	public class NavigateToTarget : IStrategy
	{
		readonly EntityMediator entity;
		readonly Transform target;
		bool isPathCalculated;

		public NavigateToTarget(EntityMediator entity, Transform target)
		{
			this.entity = entity;
			this.target = target;
		}

		public Node.Status Process()
		{
			if (Vector3.Distance(entity.GetTransform().position, target.position) < 1f)
			{
				return Node.Status.Success;
			}

			entity.NavigatePathTo(target.position);
			//entity.LookAt(target.position.With(y:entity.position.y));

			isPathCalculated = entity.IsNavigating();
			return Node.Status.Running;
		}

		public void Reset() => isPathCalculated = false;
	}

	public class NavigateToTargetDynamic : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Transform> target;
		bool isPathCalculated;
		float lastCalcTime = 0f;

		public NavigateToTargetDynamic(EntityMediator entity, Func<Transform> target)
		{
			this.entity = entity;
			this.target = target;
		}

		public Node.Status Process()
		{
			if (target().OrNull() == null)
				return Node.Status.Failure;
			if (Vector3.Distance(entity.GetTransform().position, target().position) < 1f)
			{
				return Node.Status.Success;
			}

			if (!isPathCalculated)
			{
				entity.NavigatePathTo(target().position);
				lastCalcTime = Time.time;
			}
			if (Time.time - lastCalcTime > 1f && !entity.IsNavigatorCalculating())
			{
				return Node.Status.Success;
			}
			//entity.LookAt(target.position.With(y:entity.position.y));

			isPathCalculated = entity.IsNavigatorActive();
			return Node.Status.Running;
		}

		public void Reset() => isPathCalculated = false;
	}
	public class StopMoving : IStrategy
	{
		readonly EntityMediator entity;

		public StopMoving(EntityMediator entity)
		{
			this.entity = entity;
		}

		public Node.Status Process()
		{
			if (entity.IsNavigating())
				entity.CancelPath();
			entity.MoveToDirection(Vector3.zero);

			return Node.Status.Success;
		}

		public void Reset() { }
	}

	public class AttackTowardsDirection : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Vector3> target;
		bool primary = true;
		bool fired = false;
		//float lastCalcTime = 0f;

		public AttackTowardsDirection(EntityMediator entity, Func<Vector3> target, bool primary = true)
		{
			this.entity = entity;
			this.target = target;
			this.primary = primary;
		}

		public Node.Status Process()
		{
			entity.FacePosition(target());
			if (primary)
				entity.PrimaryFire(!fired);
			else
				entity.SecondaryFire(!fired);
			fired = !fired;
			// entity.LookAt(target.position.With(y:entity.position.y));
			// Debug.Log("Fired: " + fired);

			if (!fired)
			{
				return Node.Status.Success;
			}
			return Node.Status.Running;
		}

		public void Reset() => fired = false;
	}
}