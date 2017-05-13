using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SimpleTower : PolygonGameObject, IFreezble
{
	MTowerData data;
	private float shootAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	private float currentAimAngle = 0;
	Rotaitor cannonsRotaitor;
	protected AIHelper.AccuracyChangerAdvanced accuracyChanger;
	protected float accuracy { get { return accuracyChanger.accuracy; } }

	public void InitSimpleTower (MTowerData data) {
		this.data = data;
		InitPolygonGameObject (data.physical);
		this.shootAngle = data.shootAngle;
		accuracyChanger = new AIHelper.AccuracyChangerAdvanced(data.accuracy, this);
		cannonsRotaitor = new Rotaitor(cacheTransform, data.rotationSpeed);
	}

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		cannonsRotaitor.Freeze(multipiler);
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		accuracyChanger.Tick (delta);
		Brake (delta, 4f);
		SlowRotation (delta, 30f);
		if (data.rotateWhileShooting || !guns.Exists (g => g.IsFiring ())) {
			RotateCannon (delta);
		}
		CalculateAim ();
		TickGuns (delta);
		if(!Main.IsNull(target)) {
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
		if (rotationAbs > deltaRotation) {
			rotation = Mathf.Sign (rotation) * (rotationAbs - deltaRotation);
		} else {
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

}


