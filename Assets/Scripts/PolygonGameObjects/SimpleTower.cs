using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SimpleTower : PolygonGameObject, IFreezble
{
	//public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	private float shootAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	//private float cannonsRotatingSpeed = 0;
	
	private float currentAimAngle = 0;
	
	Rotaitor cannonsRotaitor;
	float accuracy;

	public void InitSimpleTower(PhysicalData physical, float cannonsRotatingSpeed, AccuracyData accData, float shootAngle)
	{
		InitPolygonGameObject (physical);

		this.shootAngle = shootAngle;

		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			StartCoroutine (AccuracyChanger (accData));

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
	}

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		cannonsRotaitor.Freeze(multipiler);
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);

		Brake (delta, 4f);

		SlowRotation (delta, 30f);

		RotateCannon(delta);

		CalculateAim ();

		TickGuns (delta);

		if(!Main.IsNull(target))
		{
			bool fastRotation = Mathf.Abs (rotation) > cannonsRotaitor.rotatingSpeed * 1.2f;
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < shootAngle || fastRotation) {
				Shoot ();
			}
		}
	}


	private void SlowRotation(float delta, float slowingSpeed)
	{
		if (rotation == 0)
			return;
		
		var deltaRotation = slowingSpeed * delta;
		var rotationAbs = Mathf.Abs (rotation);
		if(rotationAbs > deltaRotation)
		{
			rotation = Mathf.Sign(rotation) * (rotationAbs - deltaRotation);
		}
		else
		{
			rotation = 0;
		}	
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}
	
	private void CalculateAim()
	{
		if (!Main.IsNull (target)) {
			AimSystem aim = new AimSystem (target.position, accuracy * target.velocity, position, guns [0].BulletSpeedForAim);
			if (aim.canShoot) {
				currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
			}
		} else {
			currentAimAngle = cacheTransform.eulerAngles.z;
		}
	}

	private IEnumerator AccuracyChanger(AccuracyData data)
	{
		accuracy = data.startingAccuracy;
		if (!data.isDynamic) 
		{
			yield break;
		}

		float dtime = data.checkDtime;
		bool hasEstimatedPosition = false;
		Vector2 estimatedPosition = Vector2.zero;
		PolygonGameObject lastTarget = null;

		while(true)
		{
			if (lastTarget != target) {
				hasEstimatedPosition = false;
				accuracy = data.startingAccuracy;
			}

			if(!Main.IsNull(target))
			{
				if (!hasEstimatedPosition) 
				{
					estimatedPosition = target.position + target.velocity * dtime;
					hasEstimatedPosition = true;
				} 
				else 
				{
					float diffDistance = (target.position - estimatedPosition).magnitude;

					if (diffDistance >= data.thresholdDistance)
					{
						accuracy -= dtime * data.sub;
					}
					else 
					{
						accuracy += dtime * data.add;
					}

					accuracy = Mathf.Clamp (accuracy, data.bounds.x, data.bounds.y);
					//Debug.LogWarning (diffDistance);
					//Debug.LogWarning (accuracy);

					estimatedPosition = target.position + target.velocity * dtime;
				}
				//AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, ref lastVelocity, target, data);
			}

			lastTarget = target;
			yield return new WaitForSeconds(dtime);
		}
	}
}
