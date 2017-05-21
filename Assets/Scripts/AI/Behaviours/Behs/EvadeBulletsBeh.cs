using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeBulletsBeh : DelayedActionBeh {
	List<PolygonGameObject> bullets;
	float duration;
	Vector2 newDir;

	public EvadeBulletsBeh (CommonBeh.Data data,  List<PolygonGameObject> bullets, IDelayFlag delay):base(data, delay) {
		this.bullets = bullets;
		_isUrgent = true;
	}

	public override bool IsReadyToAct () {
		if (!base.IsReadyToAct()) {
			return false;
		}
		delay.SetOnMin ();
		return AIHelper.EvadeBullets (thisShip, bullets, out duration, out newDir);
	}

	public override void Start () {
		base.Start ();
		data.accuracyChanger.ExternalChange(-0.3f);
	}

	protected override IEnumerator Action () {
		SetFlyDir (newDir);
		var wait = WaitForSeconds (duration);
		yield return wait.MoveNext ();
	}
}
