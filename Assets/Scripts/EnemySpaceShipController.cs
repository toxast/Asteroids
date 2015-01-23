using UnityEngine;
using System.Collections;

public class EnemySpaceShipController : InputController, IGotTarget
{
	PolygonGameObject thisShip;
	PolygonGameObject target;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;

	public EnemySpaceShipController(PolygonGameObject thisShip, PolygonGameObject target)
	{
		this.thisShip = thisShip;
		this.target = target;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(PolygonGameObject p)
	{
		accelerating = false;
		shooting = false;
		if(target != null)
		{
			accelerating = true;
			shooting = true;
			turnDirection = target.cacheTransform.position - thisShip.cacheTransform.position;
			if(turnDirection.sqrMagnitude < 400f)
			{
				turnDirection = -turnDirection;
			}
		}
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
