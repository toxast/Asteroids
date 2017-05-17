using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShootBeh : BaseBeh {
	float duration;
	Vector2 newDir;
	IDelayFlag delayAccelerationControl;
	float bulletsSpeed;
	RandomFloat attackDuration;

	public ShootBeh (BaseBeh.Data data, float bulletsSpeed, IDelayFlag delayAccelerationControl, RandomFloat attackDuration):base(data) {
		this.bulletsSpeed = bulletsSpeed;
		this.delayAccelerationControl = delayAccelerationControl;
		this.attackDuration = attackDuration;
	}

	public override bool IsUrgent () { return false; }
	public override bool CanBeInterrupted () { return true; }
	public override bool PassiveTickOtherBehs() {return true;}

	public override bool IsReadyToAct () {
		return !Main.IsNull (target);
	}

	public override void Start () {
		base.Start ();
		duration = attackDuration.RandomValue;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		duration -= delta;
		Shoot (data.accuracyChanger.accuracy, bulletsSpeed);
		delayAccelerationControl.Tick (delta);
		if (delayAccelerationControl.passed) {
			delayAccelerationControl.Set ();
			AcclerateControl ();
		}
	}

	public override void PassiveTick (float delta) {
		delayAccelerationControl.Tick (delta);
	}

	public override bool IsFinished () {
		return Main.IsNull (target) || (duration < 0 && !data.mainGun.IsFiring());
	}

	protected virtual float SelfSpeedAccuracy() {
		return 1;
	}

	void AcclerateControl() {
		var tickData = data.getTickData ();
		if (tickData == null) {
			return;
		}
		bool iaccelerate = false;
		float comfortDistMiddle = (data.comformDistanceMax + data.comformDistanceMin) / 2f;
		if (tickData.distEdge2Edge > comfortDistMiddle) {
			float approachingVelocity = tickData.vprojThis + tickData.vprojTarget;
			float timeToApproachComfort = (tickData.distEdge2Edge - comfortDistMiddle) / approachingVelocity;
			iaccelerate = approachingVelocity < 0 || timeToApproachComfort > 1f;
			if (approachingVelocity > 0 && !iaccelerate && Math2d.Chance(0.3f)) {
				FireBrake ();
			}
		}
		FireAccelerateChange (iaccelerate);
	}

	void Shoot(float accuracy, float bulletsSpeed)
	{
		if (Main.IsNull (target)) {
			return;
		}
		Vector2 turnDirection;
		bool shooting;
		Vector2 relativeVelocity = (target.velocity);
		AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity - SelfSpeedAccuracy() * Main.AddShipSpeed2TheBullet(thisShip), thisShip.position, bulletsSpeed);
		if(a.canShoot) {
			turnDirection = a.directionDist;
			var angleToRotate = Math2d.ClosestAngleBetweenNormalizedDegAbs (turnDirection.normalized, thisShip.cacheTransform.right);
			shooting = (angleToRotate < thisShip.shootAngle);
		} else {
			turnDirection = target.position - thisShip.position;
			shooting = false;
		}
		FireDirChange (turnDirection);
		FireShootChange (shooting);
	}
}
