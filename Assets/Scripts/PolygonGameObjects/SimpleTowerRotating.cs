using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SimpleTowerRotating : PolygonGameObject, IFreezble
{
	//public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	MTowerRotatingData data;
	private float shootAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	//private float cannonsRotatingSpeed = 0;

	private float currentAimAngle = 0;
	private float currentOffsetAimAngle = 0;

	Rotaitor cannonsRotaitorCurrent;
	Rotaitor cannonsRotaitorReload;
	Rotaitor cannonsRotaitorShoot;
	float accuracy;

	public void InitTowerRotating (MTowerRotatingData data) {
		this.data = data;
		InitPolygonGameObject (data.physical);
		this.shootAngle = data.shootAngle;

		accuracy = data.accuracy.startingAccuracy;
		if (data.accuracy.isDynamic) {
			StartCoroutine (AccuracyChanger (data.accuracy));
		}

		cannonsRotaitorReload = new Rotaitor(cacheTransform, data.rotationSpeed);
		cannonsRotaitorShoot = new Rotaitor(cacheTransform, data.rotationSpeedWhileShooting);
		cannonsRotaitorCurrent = cannonsRotaitorReload;
		StartCoroutine (FiringRoutine ());
	}

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		cannonsRotaitorReload.Freeze(multipiler);
		cannonsRotaitorShoot.Freeze (multipiler);
	}

	float lastDelta = 0;
	public override void Tick(float delta) 	{
		lastDelta = delta;
		base.Tick (delta);
		Brake (delta, 4f);
		SlowRotation (delta, 30f);
		CalculateAim ();
		TickGuns (delta);
	}

	IEnumerator FiringRoutine(){
		while (true) {
			yield return null;
			bool fastRotation = true;
			while (fastRotation) {
				fastRotation = Mathf.Abs (rotation) > cannonsRotaitorCurrent.rotatingSpeed * 1.2f;
				yield return null;
			}

			bool gunsInPosition = false;
			bool gunsAreReady = false;
			while (!(gunsInPosition && gunsAreReady && !Main.IsNull (target))) {
				RotateCannon (lastDelta, currentOffsetAimAngle);
				gunsInPosition = (Mathf.Abs (cannonsRotaitorCurrent.DeltaAngle (currentOffsetAimAngle)) < 5f);
				gunsAreReady = guns.TrueForAll (g => g.ReadyToShoot ());
				yield return null;
			}

			bool rotateRight = cannonsRotaitorCurrent.DeltaAngle (currentAimAngle) < 0;
			cannonsRotaitorCurrent = cannonsRotaitorShoot;
			Shoot ();
			yield return null;
			while (guns.Exists (g => g.IsFiring ())) {
				cannonsRotaitorCurrent.Rotate (lastDelta, rotateRight);
				yield return null;
			}
			cannonsRotaitorCurrent = cannonsRotaitorReload;
		}
	}

	private void SlowRotation(float delta, float slowingSpeed)
	{
		if (rotation == 0)
			return;

		var deltaRotation = slowingSpeed * delta;
		var rotationAbs = Mathf.Abs (rotation);
		if (rotationAbs > deltaRotation) {
			rotation = Mathf.Sign (rotation) * (rotationAbs - deltaRotation);
		} else {
			rotation = 0;
		}
	}

	private void RotateCannon(float deltaTime, float targetAngle)
	{
		cannonsRotaitorCurrent.Rotate(deltaTime, targetAngle);
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

		//offset aim angle by x deg, for tower to start shooting shootAngle away from the ship
		if(Mathf.Abs(cannonsRotaitorCurrent.DeltaAngle(currentAimAngle + data.shootAngle)) < 
			Mathf.Abs(cannonsRotaitorCurrent.DeltaAngle(currentAimAngle - data.shootAngle))) {
			currentOffsetAimAngle = currentAimAngle + data.shootAngle;
		} else {
			currentOffsetAimAngle = currentAimAngle - data.shootAngle;
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
