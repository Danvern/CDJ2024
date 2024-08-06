using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityMediator : IVisitable
{
	private Entity entity;
	private IMovementLogic movement;
	private EntityHealthLogic health;

	public EntityMediator(Entity entity, EntityHealthLogic health, IMovementLogic movement)
	{
		this.entity = entity;
		this.health = health;
		this.movement = movement;
	}

	public bool IsHostile(EntityMediator mediator) => mediator.entity != entity && mediator.entity.IsEnemy != entity.IsEnemy;
	public bool IsDead() => entity.IsDead;

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
