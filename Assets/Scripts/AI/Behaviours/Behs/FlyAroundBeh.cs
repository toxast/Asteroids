using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FlyAroundBeh : CommonBeh {
	IEnumerator action;
	bool isFinished = false;

	public FlyAroundBeh (CommonBeh.Data data):base(data) { 	
		_passiveTickOthers = true;
	}
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

        var wait = WaitForSeconds(1);
        while (wait.MoveNext()) yield return true;

        FireAccelerateChange (false);

        wait = WaitForSeconds(2);
        while (wait.MoveNext()) yield return true;
    }

    public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = !action.MoveNext ();
	}

	public override bool IsFinished () {
		return isFinished;
	}
}
