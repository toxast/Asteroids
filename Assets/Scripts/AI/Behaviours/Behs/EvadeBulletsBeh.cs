using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeBulletsBeh : BaseBeh {

	public override bool IsUrgent () { return true; }

	IDelayFlag delay;
	List<PolygonGameObject> bullets;
	float duration;
	Vector2 newDir;

	public EvadeBulletsBeh (BaseBeh.Data data,  List<PolygonGameObject> bullets, IDelayFlag delay):base(data) {
		this.bullets = bullets;
		this.delay = delay;
	}

	public override bool IsReadyToAct () {
		if (!delay.passed) {
			return false;
		}
		delay.SetOnMin ();
		return AIHelper.EvadeBullets (thisShip, bullets, out duration, out newDir);
	}

	public override void Start () {
		base.Start ();
		delay.Set ();
		data.accuracyChanger.ExternalChange(-0.3f);
		SetFlyDir (newDir);
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		duration -= delta;
	}

	public override bool IsFinished () {
		return duration <= 0;
	}

	public override void PassiveTick (float delta) {
		delay.Tick (delta);
	}
}
