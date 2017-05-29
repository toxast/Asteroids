using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisShootBeh : ShootBeh {
	Func<bool> isInvisibleBeh;
	public InvisShootBeh (CommonBeh.Data data, IDelayFlag delay, IDelayFlag delayAccelerationControl, RandomFloat attackDuration, Func<bool> isInvisibleBeh)
		:base(data, delay, delayAccelerationControl, attackDuration) {
		this.isInvisibleBeh = isInvisibleBeh;
	}

	public override bool IsFinished (){
		return base.IsFinished () || isInvisibleBeh ();
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && !isInvisibleBeh();
	}
}
