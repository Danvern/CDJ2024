using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
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

	public class MoveToTarget : IStrategy
	{
		readonly EntityMediator entity;
		readonly Transform target;
		bool isPathCalculated;

		public MoveToTarget(EntityMediator entity, Transform target)
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

	public class MoveToTargetAction : IStrategy
	{
		readonly EntityMediator entity;
		readonly Func<Transform> target;
		bool isPathCalculated;
		float lastCalcTime = 0f;

		public MoveToTargetAction(EntityMediator entity, Func<Transform> target)
		{
			this.entity = entity;
			this.target = target;
		}

		public Node.Status Process()
		{
			if (Vector3.Distance(entity.GetTransform().position, target().position) < 1f)
			{
				return Node.Status.Success;
			}

			if (Time.time - lastCalcTime > 1f)
			{
				entity.NavigatePathTo(target().position);
				lastCalcTime = Time.time;
			}
			//entity.LookAt(target.position.With(y:entity.position.y));

			isPathCalculated = entity.IsNavigating();
			return Node.Status.Running;
		}

		public void Reset() => isPathCalculated = false;
	}
}