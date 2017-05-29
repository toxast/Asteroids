using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeTargetBeh : DelayedActionBeh {
	float duration;
	Vector2 newDir;

	public EvadeTargetBeh (CommonBeh.Data data, IDelayFlag delay):base(data, delay) {
		_isUrgent = true;
	}

	public override bool IsReadyToAct () {
		var tickData = data.getTickData ();
		if (tickData == null) {
			return false;
		}
		return AIHelper.EvadeTarget (thisShip, thisShip.target, tickData, out duration, out newDir);
	}

	protected override IEnumerator Action () {
		data.accuracyChanger.ExternalChange(-0.3f);
		SetFlyDir (newDir);
		var wait = WaitForSeconds (duration);
		while (wait.MoveNext ()) yield return true;
	}
}


