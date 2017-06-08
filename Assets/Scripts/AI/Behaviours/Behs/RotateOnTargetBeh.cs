using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RotateOnTargetBeh : DelayedActionBeh
{
	float timeLeft = 0;
	Func<Vector2> getAimDirection;
	public RotateOnTargetBeh(CommonBeh.Data data, IDelayFlag delay, Func<Vector2> getAimDirection) : base(data,delay){
		this.getAimDirection = getAimDirection;
		_canBeInterrupted = true;
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && TargetNotNull;
	}

	protected virtual float GetTotalDuration() {
		return Random.Range (180f - 60f, 180f - 30f) / data.thisShip.originalTurnSpeed;
	}

	public float GetTimeLeft() {
		return timeLeft;
	}

	protected override IEnumerator Action () {
		timeLeft = GetTotalDuration ();
		//LogWarning("rotate on target " + duration);
		FireBrake ();
		while (timeLeft >= 0 && TargetNotNull) {
			timeLeft -= DeltaTime ();
			FireDirChange (getAimDirection ());
			yield return true;
		}
		timeLeft = 0;
		/*var wait = WaitForSeconds (duration);
		while (wait.MoveNext() && TargetNotNull) {
			FireDirChange (getAimDirection ());
			yield return true;
		}*/

	}
}

