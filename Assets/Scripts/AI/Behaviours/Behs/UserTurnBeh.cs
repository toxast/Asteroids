using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UserTurnBeh : TurnBeh {
	bool lastBrake = false;

	public UserTurnBeh (CommonBeh.Data data, IDelayFlag delay):base(data, delay) {
	}

	protected override float SelfSpeedAccuracy () {
		return data.accuracyChanger.accuracy;
	}

	protected override IEnumerator ComfortTurn () {
		if (Math2d.Chance (0.5f)) {
			lastBrake = false;
			return base.ComfortTurn ();
		} else {
			float duration = new RandomFloat (2f, 3f).RandomValue;
			bool acc = Math2d.Chance (0.35f);
			bool brake = (acc || lastBrake) ? false : Math2d.Chance (0.5f);
			lastBrake = brake;
			return AIHelper.TimerR (duration, DeltaTime, () => Shoot(acc, brake));
		}
	}

	protected override IEnumerator OutOfComformTurn (bool far)
	{
		if (Math2d.Chance (0.5f)) {
			lastBrake = false;
			return base.OutOfComformTurn (far);
		} else {
			float duration = new RandomFloat (2f, 3f).RandomValue;
			bool brake = (far || lastBrake) ? false : Math2d.Chance (0.5f);
			lastBrake = brake;
			return AIHelper.TimerR (duration, DeltaTime, () => Shoot(far, brake));
		}
	}

	void Shoot(bool accelerate, bool brake) {
		if (accelerate) {
			FireAccelerateChange (accelerate);
		}
		if (brake) {
			FireBrake ();
		}
		Shoot (accuracy, data.mainGun.BulletSpeedForAim);
	}
}