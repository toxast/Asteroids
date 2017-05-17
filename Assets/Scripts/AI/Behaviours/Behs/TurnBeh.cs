using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnBeh : BaseBeh {

	IDelayFlag delay;
	AIHelper.Data tickData;
	bool isFinished = false;
	IEnumerator action;
	Gun mainGun;
	public override bool CanBeInterrupted () {return true;}

	public TurnBeh (BaseBeh.Data data, IDelayFlag delay):base(data) {
		this.delay = delay;
	}

	public override bool IsReadyToAct () {
		tickData = data.getTickData ();
		return (delay.passed && tickData != null);
	}

	public override void Start () {
		base.Start ();
		delay.Set ();
		if (tickData.distEdge2Edge > data.comformDistanceMax || tickData.distEdge2Edge < data.comformDistanceMin) {
			bool far = tickData.distEdge2Edge > data.comformDistanceMax;
			data.accuracyChanger.ExternalChange(-0.2f);
			action = OutOfComformTurn (far);
		} else {
			data.accuracyChanger.ExternalChange(0.1f);
			action = ComfortTurn ();
		}
		isFinished = false;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = action.MoveNext ();
	}

	public override bool IsFinished () {
		return isFinished;
	}

	public override void PassiveTick (float delta) {
		delay.Tick (delta);
	}

	protected virtual IEnumerator ComfortTurn() {
		float duration;
		Vector2 newDir;
		AIHelper.ComfortTurn (data.comformDistanceMax, data.comformDistanceMin, tickData, out duration, out newDir);
		SetFlyDir (newDir);
		yield return WaitForSeconds (duration);
	}

	protected virtual IEnumerator OutOfComformTurn(bool far) {
		float duration;
		Vector2 newDir;
		AIHelper.OutOfComformTurn (thisShip, data.comformDistanceMax, data.comformDistanceMin, tickData, out duration, out newDir);
		SetFlyDir (newDir);
		yield return WaitForSeconds (duration);
	}
}
