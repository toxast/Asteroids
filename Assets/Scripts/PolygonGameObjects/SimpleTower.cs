using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SimpleTower : PolygonGameObject, IFreezble
{
	MTowerData data;
	private float shootAngle; //if angle to target bigger than this - dont even try to shoot

	AdvancedTurnComponent turnComponent;
	private Vector2 aimDirNorm;

	protected AIHelper.AccuracyChangerAdvanced accuracyChanger;
	protected float accuracy { get { return accuracyChanger.accuracy; } }

	public void InitSimpleTower (MTowerData data) {
		this.data = data;
		InitPolygonGameObject (data.physical);
		this.shootAngle = data.shootAngle;
		accuracyChanger = new AIHelper.AccuracyChangerAdvanced(data.accuracy, this);
		turnComponent = new AdvancedTurnComponent (this, data.rotationSpeed);
		aimDirNorm = cacheTransform.right;
	}

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		turnComponent.Freeze (multipiler);
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		if (data.rotateWhileShooting || !guns.Exists (g => g.IsFiring ())) {
			CalculateAim ();
		}
		accuracyChanger.Tick (delta);
		Brake (delta, 4f);
		TickGuns (delta);
		if(!Main.IsNull(target)) {
			if (!turnComponent.isFastRotation()) {
				if (turnComponent.inAngleRange(aimDirNorm, shootAngle)) {
					Shoot ();
				}
			}
		}
	}

	protected override void ApplyRotation (float dtime) {
		turnComponent.TurnByDirection (aimDirNorm, dtime);
	}
	
	private void CalculateAim()
	{
		if (TargetNotNull) {
			AimSystem aim = new AimSystem (target.position, accuracy * target.velocity, position, guns [0].BulletSpeedForAim);
			if (aim.canShoot) {
				aimDirNorm = aim.directionDist.normalized;
			}
		} else {
			aimDirNorm = cacheTransform.right;
		}
	}

}


