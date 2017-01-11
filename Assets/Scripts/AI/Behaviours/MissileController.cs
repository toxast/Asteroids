using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileController : InputController, IGotTarget
{
	SpaceShip thisShip;
	PolygonGameObject target;
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }

	float maxVelocity;
	float accuracy = 0.5f;

	public MissileController(SpaceShip thisShip, float maxVelocity, float accuracy)
	{
		this.maxVelocity = maxVelocity;
		this.thisShip = thisShip;
		this.accuracy = accuracy;
	}

	public void SetTarget(PolygonGameObject target)
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
        AimSystem aim = new AimSystem(target.position, accuracy * (1.3f * target.velocity - thisShip.velocity), thisShip.position, maxVelocity);
        if (aim.canShoot) {
            turnDirection = aim.direction;
        } else {
            turnDirection = target.position - thisShip.position;
        }
    }
}
