using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisCowardBeh : DelayedActionBeh {
	Func<bool> isInvisibleBeh;
	bool wasCowardOnCurrentInvis = true;
	float approhimateDuration = 1.5f;
	public InvisCowardBeh (CommonBeh.Data data, IDelayFlag delay, Func<bool> isInvisibleBeh):base(data, delay) {
		this.isInvisibleBeh = isInvisibleBeh;
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && !wasCowardOnCurrentInvis && isInvisibleBeh();
	}

	public override void Start () {
		base.Start ();
		wasCowardOnCurrentInvis = true;
	}

	public override void PassiveTick (float delta) {
		base.PassiveTick (delta);
		if (!isInvisibleBeh()) {
			wasCowardOnCurrentInvis = false;
		}
	}

	protected override IEnumerator Action () {
		int turnsTotal = 2;
		var wait = CowardAction(approhimateDuration, turnsTotal);
		while (wait.MoveNext()) yield return true;
	}
}

