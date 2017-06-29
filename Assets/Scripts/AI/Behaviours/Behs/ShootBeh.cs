using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ShootBeh : DelayedActionBeh {
	Vector2 newDir;
	IDelayFlag delayAccelerationControl;
	float bulletsSpeed;
	RandomFloat attackDuration;

	public ShootBeh (CommonBeh.Data data, IDelayFlag delay, IDelayFlag delayAccelerationControl, RandomFloat attackDuration):base(data, delay) {
		this.bulletsSpeed = data.mainGun.BulletSpeedForAim;
		this.delayAccelerationControl = delayAccelerationControl;
		this.attackDuration = attackDuration;
		_canBeInterrupted = true;
		_passiveTickOthers = true;
	}

	public override bool IsReadyToAct () {
		return TargetNotNull && base.IsReadyToAct ();
	}

	public override void Tick (float delta) {
		base.Tick (delta);

		delayAccelerationControl.Tick (delta);
		if (delayAccelerationControl.passed) {
			delayAccelerationControl.Set ();
			AcclerateControl ();
		}
	}

	public override void PassiveTick (float delta) {
		base.PassiveTick (delta);
		delayAccelerationControl.Tick (delta);
	}

	public override bool IsFinished () {
		return (base.IsFinished () && !data.mainGun.IsFiring ()) || TargetIsNull;
	}

	protected override IEnumerator Action () {
		var duration = attackDuration.RandomValue;
		var attack = AIHelper.TimerR (duration, DeltaTime, () => Shoot (data.accuracyChanger.accuracy, bulletsSpeed), () => TargetIsNull);
		while (attack.MoveNext ())
			yield return true;
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
			if (approachingVelocity > 0 && !iaccelerate && Math2d.Chance (0.05f)) {
				FireBrake ();
				//Debug.LogError ("brake");
			}
		} else if (tickData.distEdge2Edge < Mathf.Min(20, data.comformDistanceMin)) {
			iaccelerate = false;
			if (Math2d.Chance (0.3f)) {
				Debug.LogError ("brake min dist " + data.comformDistanceMin);
				FireBrake ();
			}
		} else {
			iaccelerate = Math2d.Chance(0.3f);
		}
		FireAccelerateChange (iaccelerate);
	}
}
