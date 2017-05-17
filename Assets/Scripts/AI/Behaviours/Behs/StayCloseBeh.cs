using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StayCloseBeh : BaseBeh {
	PolygonGameObject defendObject;
	IEnumerator action;
	bool isFinished = false;

	public StayCloseBeh (BaseBeh.Data data, PolygonGameObject defendObject):base(data) {
		this.defendObject = defendObject;
	}

	public override bool CanBeInterrupted ()  { return true; }

	public override bool IsReadyToAct () {
		return !Main.IsNull (defendObject);
	}

	public override void Start () {
		base.Start ();
		action = Action ();
		isFinished = false;
	}

	IEnumerator Action(){
		float actDuration = 1.5f;
		float dist = defendObject.polygon.R + thisShip.polygon.R + 15f;
		float angle = UnityEngine.Random.Range (1, 360) * Mathf.Deg2Rad;
		Vector2 defPosition = defendObject.position + dist * new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
		if ((thisShip.position - defPosition).sqrMagnitude < thisShip.originalMaxSpeed * actDuration) {
			FireBrake ();
			FireShootChange (false);
            var wait = WaitForSeconds(0.5f);
            while (wait.MoveNext()) yield return true;
        } else {
			var newDir = defPosition - thisShip.position;
			SetFlyDir (newDir);
            var wait = WaitForSeconds(actDuration);
            while (wait.MoveNext()) yield return true;
		}
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = !action.MoveNext ();
	}

	public override bool IsFinished () {
		return isFinished;
	}
}
