using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityServiceLocator;


public class EntityMediator : IVisitable
{
	private Entity entity;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private Seeker navigator;

	public EntityMediator(Entity entity, EntityHealthLogic health, IMovementLogic movement)
	{
		this.entity = entity;
		this.health = health;
		this.movement = movement;
		navigator = entity.GetComponent<Seeker>();
	}
	public ServiceLocator GetServiceLocator() => ServiceLocator.For(entity);
	public bool IsHostile(EntityMediator mediator) => mediator.entity != entity && mediator.entity.IsEnemy != entity.IsEnemy;
	public bool IsDead() => entity.IsDead;
	public Transform GetTransform() => entity.OrNull() != null ? entity.transform : null;
	public Vector2 GetPosition() => entity.transform.position;
	public void ActivateGuidanceMode()
	{
		// Bootstrap seeker
	}
	public void NavigatePathTo(Vector2 targetPosition)
	{
		movement.CalculatePath(navigator, entity.transform.position, targetPosition);
//		MoveToDirection(navigator.steeringTarget);
		MoveToDirection(targetPosition);
	}
	public void CancelPath() => movement.CancelPath(navigator);
	public bool IsNavigating() => movement.IsFollowingPath(); //navigator.pathPending;
	public bool IsNavigatorActive() => movement.IsPathPending() || movement.IsFollowingPath(); //navigator.pathPending;
	public bool IsNavigatorCalculating() => movement.IsPathPending(); //navigator.pathPending;
	//public void UpdateNavigatorPosition(Vector3 position) {} //navigator.nextPosition = position;
	public float GetRemainingTravelDistance() => movement.RemainingPathDistance();
	public float GetWaypointCloseness() => entity.GetWaypointCloseness();
//	public float GetRemainingTravelDistance() => navigator.remainingDistance;
public void AddObserver(IEntityObserver observer) => entity?.AddObserver(observer);
public void RemoveObserver(IEntityObserver observer) => entity?.RemoveObserver(observer);

	public void Accept(IVisitor visitor)
	{
		entity?.Accept(visitor);
		health?.Accept(visitor);
		movement?.Accept(visitor);
	}

	public void DashToAim(float power, float slideTime, bool fullStun = true) => entity?.DashToAim(power, slideTime, fullStun);
	public void MoveToDirection(Vector2 direction) => movement?.MoveToDirection(direction);
	public void FacePosition(Vector2 position) => entity?.FacePosition(position);
	public void PrimaryFire(bool pressed) => entity?.PrimaryFire(pressed);

	
}
