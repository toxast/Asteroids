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
	float accuracy;

	public void InitSimpleTower(PhysicalData physical, float cannonsRotatingSpeed, AccuracyData accData)
	{
		InitPolygonGameObject (physical);

		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			StartCoroutine (AccuracyChanger (accData));

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		RotateCannon(delta);

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

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}
	
	private void CalculateAim()
	{
		if(!Main.IsNull(target))
		{
			AimSystem aim = new AimSystem(target.position, accuracy * target.velocity, position, guns[0].bulletSpeed);
			if(aim.canShoot)
			{
				currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
			}
		}
	}

	private IEnumerator AccuracyChanger(AccuracyData data)
	{
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds(dtime);
		}
	}
}
