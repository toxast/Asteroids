using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisibleSpaceshipController : BaseSpaceshipController, IGotTarget
{
	//MInvisibleSpaceshipData.InvisibleData invisData;
	ChangeInvisibilityBeh changeInvisiblilityBeh;

	public InvisibleSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData, MInvisibleSpaceshipData.InvisibleData invisData) : base(thisShip, accData)
    {
		//this.invisData = invisData;
        thisShip.increaceAlphaOnHitAndDropInvisibility = true;

		float evadeDuration = (90f / thisShip.originalTurnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.originalMaxSpeed * 0.8f);
		var evadeBullets = evadeDuration < 1.2f;
		var turnBehEnabled = evadeDuration < 3f;

		Debug.Log(thisShip.name + " evadeDuration " + evadeDuration + " turnBehEnabled: " + turnBehEnabled + " evadeBullets: " + evadeBullets);

		var comformDistanceMax = gun.Range;
		var comformDistanceMin = comformDistanceMax * 0.5f;
		CommonBeh.Data behData = new CommonBeh.Data {
			accuracyChanger = accuracyChanger,
			comformDistanceMax = comformDistanceMax,
			comformDistanceMin = comformDistanceMin,
			getTickData = GetTickData,
			mainGun = gun,
			thisShip = thisShip,
		};

		changeInvisiblilityBeh = new ChangeInvisibilityBeh (behData, new NoDelayFlag (), invisData);
		if (changeInvisiblilityBeh.IsReadyToAct ()) {
			changeInvisiblilityBeh.Start ();
		}

		EvadeTargetBeh evadeBeh = new EvadeTargetBeh(behData, new NoDelayFlag());
		logics.Add(evadeBeh);

		if (evadeBullets) {
			DelayFlag cowardDelay = new DelayFlag(true, 12, 20);
			CowardBeh cowardBeh = new CowardBeh(behData, cowardDelay); 
			logics.Add(cowardBeh);
		}

		if (evadeBullets) {
			DelayFlag checkBulletsDelay = new DelayFlag(false, 1 , 4);
			EvadeBulletsBeh evadeBulletsBeh = new EvadeBulletsBeh(behData, bullets, checkBulletsDelay);
			logics.Add(evadeBulletsBeh);
		}

		InvisCowardBeh cowardAfrerShoot = new InvisCowardBeh (behData, new NoDelayFlag (), IsInvisibleBeh);
		logics.Add(cowardAfrerShoot);

		var untilCheckAccelerationMin = evadeDuration / 6f;
		var untilCheckAccelerationMax = untilCheckAccelerationMin * 2f;
		DelayFlag accDelay = new DelayFlag(true, untilCheckAccelerationMin, untilCheckAccelerationMax);
		var attackMin = Mathf.Max(2f, evadeDuration * 2f);
		var attackMax = attackMin * 1.8f;
		var shootBeh = new InvisShootBeh(behData, new DelayFlag(true, 0.1f, 0.2f), accDelay, new RandomFloat(attackMin, attackMax), IsInvisibleBeh);
		shootBeh.SetPassiveTickOthers (true);
		logics.Add (shootBeh);

		var turnBeh = new TurnBeh (behData, new NoDelayFlag ());
		turnBeh.SetPassiveTickOthers (true);
		logics.Add (turnBeh);

		FlyAroundBeh flyAround = new FlyAroundBeh(behData);
		logics.Add(flyAround);

		AssignCurrentBeh(null);
    }

	bool IsInvisibleBeh(){
		return changeInvisiblilityBeh.IsInvis;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		changeInvisiblilityBeh.Tick (delta);
	}


	class ChangeInvisibilityBeh : DelayedActionBeh {
		bool invisibleBehaviour = true;
		MInvisibleSpaceshipData.InvisibleData invisData;
		public ChangeInvisibilityBeh (CommonBeh.Data data, IDelayFlag delay, MInvisibleSpaceshipData.InvisibleData invisData):base(data, delay) {
			this.invisData = invisData;
		}

		public bool IsInvis{get {return invisibleBehaviour;} }

		protected override IEnumerator Action ()
		{
			while (true) {
				invisibleBehaviour = true;
				thisShip.invisibilityComponent.SetState (true);

				var wait = WaitForSeconds (invisData.fadeOutDuration + invisData.invisibleDuration + new RandomFloat(0, 0.5f).RandomValue);
				while (wait.MoveNext ()) yield return true;

				thisShip.invisibilityComponent.SetState (false);

				wait = WaitForSeconds (invisData.fadeInDuration);
				while (wait.MoveNext ()) yield return true;
				//attack
				invisibleBehaviour = false;

				wait = WaitForSeconds (invisData.attackDutation + new RandomFloat(0, 0.5f).RandomValue);
				while (wait.MoveNext ()) yield return true;
			}
		}

		public override bool IsReadyToAct () {
			return true;
		}
		public override bool IsFinished () {
			return false;
		}
	}
}

