using UnityEngine;
using System;
using System.Collections;

public class Turret : PolygonGameObject
{
	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	private float rangeAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	private float currentAimAngle = 180;
	Rotaitor cannonsRotaitor;
	
	private Func<Vector3> anglesRestriction;
	
	public void InitTurret(PhysicalData physical, float cannonsRotatingSpeed, Func<Vector3> angelsRestriction)
	{
		InitPolygonGameObject (physical);

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		this.anglesRestriction = angelsRestriction;
	}

	Vector2 lastRestrictDir = new Vector2(1,0);
	float lastAllowed = 180;
	
	private void RotateCannonWithRestrictions(float deltaTime)
	{
		float angle = currentAimAngle;
		Vector3 restrict = anglesRestriction();
		lastRestrictDir = restrict;
		lastAllowed = restrict.z;
		float dangle = Math2d.DeltaAngleDeg(Math2d.GetRotationRad(lastRestrictDir)*Mathf.Rad2Deg, currentAimAngle);
		if(Mathf.Abs(dangle) < lastAllowed)
		{
			cannonsRotaitor.Rotate(deltaTime, angle);
		}
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		RotateCannonWithRestrictions(delta);
		
		CalculateAim ();
		
		TickGuns (delta);
		
		if(!Main.IsNull(target))
		{
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
			{
				Shoot();
			}
		}
	}

	private void CalculateAim()
	{
		if(!Main.IsNull(target))
		{
			AimSystem aim = new AimSystem(target.position, target.velocity, position, guns[0].bulletSpeed);
			if(aim.canShoot)
			{
				currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
			}
		}
	}
}
