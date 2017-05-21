using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StayCloseBeh : DelayedActionBeh {
	PolygonGameObject defendObject;

	public StayCloseBeh (CommonBeh.Data data, PolygonGameObject defendObject):base(data, new NoDelayFlag()) {
		this.defendObject = defendObject;
		_canBeInterrupted = true;
	}

	public override bool IsReadyToAct () {
		return !Main.IsNull (defendObject) && base.IsReadyToAct();
	}

	protected override IEnumerator Action(){
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
}
