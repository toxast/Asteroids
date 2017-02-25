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
	AIHelper.AccuracyChangerAdvanced accuracyChanger;

	public void InitTurret(PhysicalData physical, float cannonsRotatingSpeed, Func<Vector3> angelsRestriction, AccuracyData accData)
	{
		InitPolygonGameObject (physical);

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		this.anglesRestriction = angelsRestriction;

		accuracyChanger = new AIHelper.AccuracyChangerAdvanced (accData, this);
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

		accuracyChanger.Tick (delta);

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
			AimSystem aim = new AimSystem(target.position, target.velocity * accuracyChanger.accuracy, position, guns[0].BulletSpeedForAim);
			if(aim.canShoot)
			{
				currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
			}
		}
	}
}
