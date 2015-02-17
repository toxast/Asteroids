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

	float maxVelocity;

	public MissileController(IPolygonGameObject thisShip, float maxVelocity)
	{
		this.maxVelocity = maxVelocity;
		this.thisShip = thisShip;
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = false;
		accelerating = true;

		if(Main.IsNull(target))
			return;

		RotateOnTarget ();
	}

	public void SetTarget(IPolygonGameObject target)
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


	private void RotateOnTarget()
	{
		var aimVelocity = (target.velocity - thisShip.velocity) * 0.5f;
		//var aimVelocity = target.velocity * 1.5f - velocity; imba
		AimSystem aim = new AimSystem (target.cacheTransform.position, aimVelocity, thisShip.cacheTransform.position, maxVelocity);  
		turnDirection = aim.direction;
	}
}
