using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class UncontrollableRotationBeh : DelayedActionBeh
{
	public UncontrollableRotationBeh(CommonBeh.Data data, IDelayFlag delay) : base(data,delay){
	}

	protected override IEnumerator Action () {
		float uncontrollableRotateDuration = new RandomFloat(2,3).RandomValue;
		//LogWarning("rotate uncontrollable " + uncontrollableRotateDuration);
		FireAccelerateChange(false);
		FireDirChange (Vector2.zero);
		while (uncontrollableRotateDuration > 0) {
			var delta = DeltaTime ();
			uncontrollableRotateDuration -= delta;
			if (thisShip.rotation > 10) {
				thisShip.TurnLeft (delta);
			} else if (thisShip.rotation < -10) {
				thisShip.TurnRight(delta);
			} else if (thisShip.lastRotationDirLeft) {
				thisShip.TurnLeft(delta);
			} else {
				thisShip.TurnRight(delta);
			}
			yield return true;
		}
	}
}
