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
	public float accelerateValue01{ get{ return 1f;}} 

	float accuracy = 0.5f;

	public void Freeze(float m){ }

	public MissileController(SpaceShip thisShip, float accuracy)
	{
		this.thisShip = thisShip;
		this.accuracy = accuracy;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(float delta)
	{
		shooting = false;
		accelerating = true;

		if (Main.IsNull(target))
			return;

		RotateOnTarget ();
	}

	private void RotateOnTarget()
	{
		var aim = new SuicideAim(target, thisShip, accuracy);
		turnDirection = aim.direction;
    }

    public void SetSpawnParent(PolygonGameObject prnt) { }
}
