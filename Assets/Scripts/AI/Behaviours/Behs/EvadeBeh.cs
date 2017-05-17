using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeBeh : BaseBeh {
	float duration;
	Vector2 newDir;

	public override bool IsUrgent () { return true; }

	public EvadeBeh (BaseBeh.Data data):base(data) {
	}

	public override bool IsReadyToAct () {
		var tickData = data.getTickData ();
		if (tickData == null) {
			return false;
		}
		return AIHelper.EvadeTarget (thisShip, thisShip.target, tickData, out duration, out newDir);
	}

	public override void Start () {
		base.Start ();
		data.accuracyChanger.ExternalChange(-0.3f);
		SetFlyDir (newDir);
	}

	public override void Tick (float delta) {
		duration -= delta;
	}

	public override bool IsFinished () {
		return duration <= 0;
	}
}


public class FlyAroundBeh : BaseBeh {
	IEnumerator action;
	bool isFinished = false;

	public FlyAroundBeh (BaseBeh.Data data):base(data) { 	}
	public override bool CanBeInterrupted ()  { return true; }
	public override bool IsReadyToAct () { return true; }

	public override void Start () {
		base.Start ();
		action = Action ();
		isFinished = false;
	}

	IEnumerator Action(){
		float angle = UnityEngine.Random.Range (1, 360) * Mathf.Deg2Rad;
		Vector2 dir = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
		SetFlyDir (dir);
		yield return WaitForSeconds (1f);
		FireAccelerateChange (false);
		yield return WaitForSeconds (2f);
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = action.MoveNext ();
	}

	public override bool IsFinished () {
		return isFinished;
	}
}
