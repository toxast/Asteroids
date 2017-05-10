using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UserController : CommonController {


	bool evadeTargetAction = false;
	float untilEvade = 1f;
	float untilEvadeMax = 3f;
	float untilEvadeMin = 1f;

	public UserController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip, bullets, gun, accData)
	{
		thisShip.StartCoroutine (HackPullPowerups ());
		thisShip.StartCoroutine (UserBehavioursRandomTiming ());
	}

	public override bool shooting {
		get {
			return true;
		}
		protected set {
			base.shooting = value;
		}
	}

	private IEnumerator UserBehavioursRandomTiming()
	{
		while(true)
		{
			TickActionVariable(ref evadeTargetAction, ref untilEvade, untilEvadeMin, untilEvadeMax);
			yield return null;
		}
	}

	IEnumerator HackPullPowerups(){
		var drops = Singleton<Main>.inst.pDrops;
		while (true) {
			var pup = drops.Find (d => d is PowerUp);
			if (pup != null) {
				pup.Accelerate (lastDelta, 3f, 1f, 12f, 12f*12f, (thisShip.position - pup.position).normalized);
			}
			yield return null;
		}
	}

	protected override IEnumerator ComfortTurn () {
		if (Math2d.Chance (0.5f)) {
			yield return base.ComfortTurn ();
		} else {
			float duration = new RandomFloat (2f, 3f).RandomValue;
			bool acc = Math2d.Chance (0.35f);
			bool brake = acc ? false : Math2d.Chance (0.5f);
			yield return AIHelper.TimerR (duration, LastDelta, () => Shoot(acc, brake));
		}
	}

	protected override bool CanEvadeTargetNow(){
		return evadeTargetAction;
	}

	protected override IEnumerator OutOfComformTurn (bool far)
	{
		if (Math2d.Chance (0.5f)) {
			yield return base.OutOfComformTurn (far);
		} else {
			float duration = new RandomFloat (2f, 3f).RandomValue;
			bool brake = far ? false : Math2d.Chance (0.5f);
			yield return AIHelper.TimerR (duration, LastDelta, () => Shoot(far, brake));
		}
	}

	protected override IEnumerator NoTargetBeh (float duration)	{
		turnDirection = thisShip.cacheTransform.right;
		turnDirection = Math2d.MakeRight (turnDirection);
		SetAcceleration (true);
		yield return new WaitForSeconds (duration); 
	}

	float LastDelta(){
		return lastDelta;
	}

	override protected float SelfSpeedAccuracy() {
		return accuracy;
	}

	void Shoot(bool accelerate, bool brake) {
		if (accelerate) {
			SetAcceleration (accelerate);
		}
		if (brake) {
			Brake ();
		}
		Shoot (accuracy, bulletsSpeed);
	}
}