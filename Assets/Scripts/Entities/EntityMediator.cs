using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityServiceLocator;


public class EntityMediator : IVisitable
{
	private Entity entity;
	private IMovementLogic movement;
	private EntityHealthLogic health;
	private NavMeshAgent navigator;

	public EntityMediator(Entity entity, EntityHealthLogic health, IMovementLogic movement)
	{
		this.entity = entity;
		this.health = health;
		this.movement = movement;
		navigator = entity.GetComponent<NavMeshAgent>();
	}
	public ServiceLocator GetServiceLocator() => ServiceLocator.For(entity);
	public bool IsHostile(EntityMediator mediator) => mediator.entity != entity && mediator.entity.IsEnemy != entity.IsEnemy;
	public bool IsDead() => entity.IsDead;
	public Transform GetTransform() => entity?.transform;
	public void ActivateGuidanceMode()
	{
		navigator.updatePosition = false;
		navigator.updateRotation = false;
	}
	public void SetNavigatorTarget(Vector3 targetPosition)
	{
		navigator.SetDestination(targetPosition.With(y: 0)); // Always level planes
		MoveToDirection(navigator.steeringTarget);
	}
	public bool IsNavigating() => navigator.pathPending;
	public void UpdateNavigatorPosition(Vector3 position) => navigator.nextPosition = position;
	public float GetRemainingTravelDistance() => navigator.remainingDistance;

	public void Accept(IVisitor visitor)
	{
		entity?.Accept(visitor);
		health?.Accept(visitor);
		movement?.Accept(visitor);
	}

	public void DashToAim(float power, float slideTime, bool fullStun = true) => entity?.DashToAim(power, slideTime, fullStun);
	public void MoveToDirection(Vector3 direction) => movement?.MoveToDirection(direction);
	public void FacePosition(Vector3 position) => entity?.FacePosition(position);
	public void PrimaryFire(bool pressed) => entity?.PrimaryFire(pressed);
}
