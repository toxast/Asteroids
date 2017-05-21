using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CowardBeh : DelayedActionBeh {
	bool lastHasShield = false;
	float approhimateDuration = 3;

	public CowardBeh (CommonBeh.Data data, IDelayFlag delay):base(data, delay) {
		_isUrgent = true;
	}

	public override bool IsReadyToAct () {
		if (!base.IsReadyToAct ()) {
			return false;
		}
		bool readyToAct = false;
		bool haveShield = thisShip.GetShield () != null && thisShip.GetShield ().currentShields > 0;
		if (thisShip.GetLeftHealthPersentage () < 0.3f || (lastHasShield && !haveShield)) {
			delay.Set ();
			if (Math2d.Chance (0.6f)) {
				readyToAct = true;
			}
		}
		lastHasShield = haveShield;
		return readyToAct;
	}

	public override void Start () {
		base.Start ();
		data.accuracyChanger.ExternalChange(-0.3f);
	}


	protected override IEnumerator Action () {
		int turnsTotal = UnityEngine.Random.Range (2, 5);
		int turns = turnsTotal;
		while (turns > 0) {
			turns--;
			float duration = approhimateDuration / turnsTotal + UnityEngine.Random.Range (-0.3f, 0.5f);
			float angle = UnityEngine.Random.Range (120f, 180f);
			var tickData = data.getTickData ();
			if (tickData == null) {
				yield break;
			}
			var newDir = Math2d.RotateVertexDeg (tickData.dirNorm, tickData.evadeSign * angle);
			SetFlyDir (newDir);

            var wait = WaitForSeconds(duration);
            while (wait.MoveNext()) yield return true;
		}
	}
}
