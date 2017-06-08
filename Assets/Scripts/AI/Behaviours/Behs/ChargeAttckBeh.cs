using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ChargeAttckBeh : DelayedActionBeh
{
	Func<Vector2> getAimDirection;
	float chargeDuration;
	bool shootWhenCharge;
	PhysicalChangesEffect.Data chargeEffect;
	public ChargeAttckBeh(CommonBeh.Data data, IDelayFlag delay, Func<Vector2> getAimDirection, 
		float chargeDuration, bool shootWhenCharge, PhysicalChangesEffect.Data chargeEffect) : base(data,delay)
	{
		this.getAimDirection = getAimDirection;
		this.chargeDuration = chargeDuration;
		this.shootWhenCharge = shootWhenCharge;
		this.chargeEffect = chargeEffect;
		_canBeInterrupted = false;
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && !Main.IsNull(target);
	}

	protected override IEnumerator Action () {
		var duration = chargeDuration;
		Debug.LogWarning("charge attack " + duration);
		if (shootWhenCharge) {
			FireShootChange (true);
		}
		bool allowBreak = Math2d.Chance (0.5f);
		FireAccelerateChange(true);
		var effect = new PhysicalChangesEffect (chargeEffect);
		thisShip.AddEffect (effect);
		float startingDuration = duration;
		while (duration > 0 && !Main.IsNull(target)) {
			FireDirChange(getAimDirection());
			duration -= DeltaTime();
			if (allowBreak && duration < startingDuration * 0.5f && IsBetterToStop()) {
				Debug.LogWarning("break charge");
				break;
			}
			yield return true;
		}
		effect.ForceFinish ();
		if (thisShip.rotation != 0 && thisShip.velocity.magnitude > thisShip.maxSpeed*0.4f && Math2d.Chance (0.8f)) {
			Debug.LogWarning ("rotate " + thisShip.rotation);
			var rotation = UncontrollableRotation ();
			while (rotation.MoveNext ()) {yield return true;}
		}
		Debug.LogWarning("charge finished");
		FireShootChange(false);
	}

	private IEnumerator UncontrollableRotation(){
		UncontrollableRotationBeh notcontrol = new UncontrollableRotationBeh (data, new NoDelayFlag());
		if (notcontrol.IsReadyToAct ()) {
			Subscribe (notcontrol);
			notcontrol.Start ();
			while(!notcontrol.IsFinished()){
				notcontrol.Tick (DeltaTime ());
				yield return true;
			}
			notcontrol.Stop ();
			Unsubscribe(notcontrol);
		}
	}

	private bool IsBetterToStop() {
		if (TargetIsNull)
			return true;

		var toTarget = target.position - thisShip.position;
		return (Vector2.Dot (thisShip.velocity, toTarget) < 0);
	}
}
