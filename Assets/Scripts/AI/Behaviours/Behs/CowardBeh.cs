using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CowardBeh : BaseBeh {

	public override bool IsUrgent () { return true; }

	IDelayFlag delay;
	bool lastHasShield = false;
	float approhimateDuration = 3;
	bool isFinished = false;
	IEnumerator action;

	public CowardBeh (BaseBeh.Data data, IDelayFlag delay):base(data) {
		this.delay = delay;
	}

	public override bool IsReadyToAct () {
		if (!delay.passed) {
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
		action = CowardAction ();
		isFinished = false;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = action.MoveNext ();
	}

	IEnumerator CowardAction(){
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
			yield return WaitForSeconds (duration);
		}
	}

	public override bool IsFinished () {
		return isFinished;
	}

	public override void PassiveTick (float delta) {
		delay.Tick (delta);
	}
}
