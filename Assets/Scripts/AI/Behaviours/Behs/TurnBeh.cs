using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnBeh : DelayedActionBeh {

	AIHelper.Data tickData;
	Gun mainGun;

	public TurnBeh (CommonBeh.Data data, IDelayFlag delay):base(data, delay) {
		_canBeInterrupted = true;
	}

	public override bool IsReadyToAct () {
		if (!base.IsReadyToAct ()) {
			return false;
		}
		tickData = data.getTickData ();
		return tickData != null;
	}

	protected override IEnumerator Action ()
	{
		if (tickData.distEdge2Edge > data.comformDistanceMax || tickData.distEdge2Edge < data.comformDistanceMin) {
			bool far = tickData.distEdge2Edge > data.comformDistanceMax;
			data.accuracyChanger.ExternalChange(-0.2f);
			return OutOfComformTurn (far);
		} else {
			data.accuracyChanger.ExternalChange(0.1f);
			return ComfortTurn ();
		}
	}

	protected virtual IEnumerator ComfortTurn() {
		float duration;
		Vector2 newDir;
		AIHelper.ComfortTurn (data.comformDistanceMax, data.comformDistanceMin, tickData, out duration, out newDir);
		SetFlyDir (newDir);
        var wait = WaitForSeconds(duration);
        while (wait.MoveNext()) yield return true;
	}

	protected virtual IEnumerator OutOfComformTurn(bool far) {
		float duration;
		Vector2 newDir;
		AIHelper.OutOfComformTurn (thisShip, data.comformDistanceMax, data.comformDistanceMin, tickData, out duration, out newDir);
		SetFlyDir (newDir);
        var wait = WaitForSeconds(duration);
        while (wait.MoveNext()) yield return true;
	}
}

