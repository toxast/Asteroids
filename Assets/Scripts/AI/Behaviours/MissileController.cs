using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileController : InputController, IGotTarget
{
	SpaceShip thisShip;
	IPolygonGameObject target;
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }

	float maxVelocity;
	float accuracy = 0.5f; //TODO

	public MissileController(SpaceShip thisShip, float maxVelocity, float accuracy)
	{
		this.maxVelocity = maxVelocity;
		this.thisShip = thisShip;
		this.accuracy = accuracy;
	}

	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(PolygonGameObject p)
	{
		shooting = false;
		accelerating = true;

		if (Main.IsNull (target))
			return;

		RotateOnTarget ();
	}

	private void RotateOnTarget()
	{
		var aimVelocity = (target.velocity* 1.5f - thisShip.velocity) * accuracy;
		AimSystem aim = new AimSystem (target.position, aimVelocity, thisShip.position, maxVelocity);  
		turnDirection = aim.direction;
	}
}
