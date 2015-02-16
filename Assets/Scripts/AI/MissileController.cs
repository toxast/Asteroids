using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileController : InputController, IGotTarget
{
	IPolygonGameObject thisShip;
	IPolygonGameObject target;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;

	public MissileController(IPolygonGameObject thisShip)
	{
		this.thisShip = thisShip;
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = false;
		accelerating = true;

		Vector2 dir = target.position - thisShip.position;
		turnDirection = dir;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public Vector2 TurnDirection ()
	{
		return turnDirection;
	}
	
	public bool IsShooting()
	{
		return shooting;
	}
	
	public bool IsAccelerating()
	{
		return accelerating;
	}
}
