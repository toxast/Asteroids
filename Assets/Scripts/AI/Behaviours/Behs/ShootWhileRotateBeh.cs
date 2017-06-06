using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ShootWhileRotateBeh : RotateOnTargetBeh {
	float durationShoot;
	float durationWait;
	public ShootWhileRotateBeh(CommonBeh.Data data, IDelayFlag delay, Func<Vector2> getAimDirection, float durationShoot, float durationWait) : base(data, delay, getAimDirection){
		this.durationShoot = durationShoot;
		this.durationWait = durationWait;
	}

	protected override float GetTotalDuration () {
		return durationWait + durationShoot;
	}

	public override void Tick (float delta)	{
		base.Tick (delta);
		FireShootChange (GetTimeLeft () > durationWait);
	}

	public override void Stop () {
		base.Stop ();
		FireShootChange (false);
	}
}
