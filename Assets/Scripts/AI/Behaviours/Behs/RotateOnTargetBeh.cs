using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RotateOnTargetBeh : DelayedActionBeh
{
	Func<Vector2> getAimDirection;
	public RotateOnTargetBeh(CommonBeh.Data data, IDelayFlag delay, Func<Vector2> getAimDirection) : base(data,delay){
		this.getAimDirection = getAimDirection;
		_canBeInterrupted = true;
	}

	public override bool IsReadyToAct ()
	{
		return base.IsReadyToAct () && !TargetNULL();
	}

	protected override IEnumerator Action () {
		var duration = Random.Range (180f - 60f, 180f - 30f) / data.thisShip.originalTurnSpeed;
		//LogWarning("rotate on target " + duration);
		FireBrake ();
		var wait = WaitForSeconds (duration);
		while (wait.MoveNext() && !TargetNULL()) {
			FireDirChange (getAimDirection ());
			yield return true;
		}
	}
}
