using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class DelayedActionBeh : CommonBeh {
	protected IDelayFlag delay;
	bool isFinished = true;
	IEnumerator action;

	public DelayedActionBeh(CommonBeh.Data data, IDelayFlag delay):base(data) {
		this.delay = delay;
	}

	public override bool IsReadyToAct () {
		if (!delay.passed) {
			return false;
		}
		return true;
	}

	public override void Start () {
		base.Start ();
		delay.Set ();
		action = Action ();
		isFinished = false;
	}

	protected abstract IEnumerator Action ();

	public override void Tick (float delta) {
		base.Tick (delta);
		isFinished = !action.MoveNext ();
	}

	public override bool IsFinished () {
		return isFinished;
	}

	public override void PassiveTick (float delta) {
		delay.Tick (delta);
	}

}
