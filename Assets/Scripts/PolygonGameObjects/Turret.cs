using UnityEngine;
using System;
using System.Collections;

public class Turret : PolygonGameObject
{
	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	private float rangeAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	//private float cannonsRotatingSpeed = 0;
	private float currentAimAngle = 180;
	Rotaitor cannonsRotaitor;
	private bool smartAim = false;
	
	private Func<Vector3> anglesRestriction;
	
	public void InitTurret(PhysicalData physical, bool smartAim, float cannonsRotatingSpeed, Func<Vector3> angelsRestriction)
	{
		InitPolygonGameObject (physical);

		this.smartAim = smartAim;
		//this.cannonsRotatingSpeed = cannonsRotatingSpeed;
		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		this.anglesRestriction = angelsRestriction;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		RotateCannonWithRestrictions(delta);
		
		TickGuns (delta);
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
	
	private void TickGuns(float delta)
	{
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].Tick(delta);
		}
		
		if(!Main.IsNull(target))
		{
			if(smartAim)
			{
				AimSystem aim = new AimSystem(target.position, target.velocity, position, guns[0].bulletSpeed);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
				}
			}
			else
			{
				currentAimAngle = Math2d.GetRotationDg(target.position - position);
			}
			
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
			{
				for (int i = 0; i < guns.Count; i++) 
				{
					guns[i].ShootIfReady();
				}
			}
		}
		else
		{
			currentAimAngle = Math2d.GetRotationDg(lastRestrictDir);
		}
	}
}
