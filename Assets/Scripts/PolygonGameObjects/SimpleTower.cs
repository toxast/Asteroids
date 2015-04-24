using UnityEngine;
using System;
using System.Collections;

public class SimpleTower : PolygonGameObject
{
	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	private float rangeAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	//private float cannonsRotatingSpeed = 0;
	
	private float currentAimAngle = 0;
	
	Rotaitor cannonsRotaitor;
	private bool smartAim = false;


	public void Init(bool smartAim, float cannonsRotatingSpeed)
	{
		this.smartAim = smartAim;
		//this.cannonsRotatingSpeed = cannonsRotatingSpeed;

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		RotateCannon(delta);

		TickGuns (delta);
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
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
	}
}
