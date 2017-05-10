using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommonController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> bullets;
	protected float bulletsSpeed;
	float comformDistanceMin, comformDistanceMax;
	bool turnBehEnabled = true;
	bool evadeBullets = true;

	Gun mainGun = null;
	AIHelper.Data tickData = new AIHelper.Data();

	public CommonController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip, accData)
	{
		mainGun = gun;
		this.bulletsSpeed = gun.BulletSpeedForAim;
		this.bullets = bullets;
		thisShip.StartCoroutine (Logic ());

		thisShip.StartCoroutine (BehavioursRandomTiming ());
		comformDistanceMax = gun.Range;
		comformDistanceMin = comformDistanceMax * 0.5f;
		
		float evadeDuration = (90f / thisShip.originalTurnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.originalMaxSpeed * 0.8f);
		evadeBullets = evadeDuration < 1.2f;
		turnBehEnabled = evadeDuration < 3f;
		if(turnBehEnabled)		{
			untilTurnMin = Mathf.Max(2f, evadeDuration * 2f);
			untilTurnMax = untilTurnMin * 1.8f;
		}

		Debug.Log (thisShip.name + " evadeDuration " + evadeDuration + " turnBehEnabled: " + turnBehEnabled + " evadeBullets: " + evadeBullets);

		untilCheckAccelerationMin = evadeDuration / 6f;
		untilCheckAccelerationMax = untilCheckAccelerationMin * 2f;
	}

	bool timeForTurnAction = false;
	float untilTurn = 0f;
	float untilTurnMax = 4.5f;
	float untilTurnMin = 2.5f;

	bool timeForCowardActionPassed = true;
	float untilCoward = 0f;
	float untilCowardMax = 20f;
	float untilCowardMin = 12f;

	bool checkBulletsAction = false;
	float untilBulletsEvade = 1f;
	float untilBulletsEvadeMax = 4f;
	float untilBulletsEvadeMin = 1f;

	bool checkAccelerationAction = false;
	float untilCheckAcceleration = 0f;
	float untilCheckAccelerationMax = 0.3f;
	float untilCheckAccelerationMin = 0.1f;

	private IEnumerator BehavioursRandomTiming()
	{
		untilCoward = UnityEngine.Random.Range (untilCowardMin, untilCowardMax);
		while(true)
		{
			TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);

			TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);

			TickActionVariable(ref checkAccelerationAction, ref untilCheckAcceleration, untilCheckAccelerationMin, untilCheckAccelerationMax);

			TickActionVariable(ref timeForCowardActionPassed, ref untilCoward, untilCowardMin, untilCowardMax);

			yield return null;
		}
	}

	protected virtual bool CanEvadeTargetNow(){
		return true;
	}

	private IEnumerator Logic()
	{
		float checkBehTimeInterval = 0.1f;
		float checkBehTime = 0;
		bool behaviourChosen = false;
		float duration;
		Vector2 newDir;
		bool lastHasShield = false;
		while (true) {
			if (!Main.IsNull (target)) {
				behaviourChosen = false;
				checkBehTime -= Time.deltaTime;

				if (checkBehTime <= 0) {
					checkBehTime = checkBehTimeInterval;

					tickData.Refresh (thisShip, target);

					if (checkAccelerationAction) {
						checkAccelerationAction = false;
						
						bool iaccelerate = false;
						float comfortDistMiddle = (comformDistanceMax + comformDistanceMin) / 2f;
						if (tickData.distEdge2Edge > comfortDistMiddle) {
							float approachingVelocity = tickData.vprojThis + tickData.vprojTarget;
							float timeToApproachComfort = (tickData.distEdge2Edge - comfortDistMiddle) / approachingVelocity;
							iaccelerate = approachingVelocity < 0 || timeToApproachComfort > 1f;
							if (approachingVelocity > 0 && !iaccelerate && Math2d.Chance(0.5f)) {
								Brake ();
							}
						}
						SetAcceleration (iaccelerate);
					}

					if (!behaviourChosen && CanEvadeTargetNow()) {
						if (AIHelper.EvadeTarget (thisShip, target, tickData, out duration, out newDir)) {
							behaviourChosen = true;
							yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
						}
					}

					if (evadeBullets && !behaviourChosen && timeForCowardActionPassed) {
						bool haveShield = thisShip.GetShield () != null && thisShip.GetShield ().currentShields > 0;
						if (thisShip.GetLeftHealthPersentage () < 0.3f || (lastHasShield && !haveShield)) {
							timeForCowardActionPassed = false;
							if (Math2d.Chance (0.6f)) {
								behaviourChosen = true;
								//Debug.LogWarning ("coward action");
								int turnsTotal = UnityEngine.Random.Range (2, 5);
								yield return CowardAction (tickData, turnsTotal);
							}
						}
						lastHasShield = haveShield;
					}

					if (!behaviourChosen && evadeBullets && checkBulletsAction) {
						if (AIHelper.EvadeBullets (thisShip, bullets, out duration, out newDir)) {
							behaviourChosen = true;
							yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration));
						}
						checkBulletsAction = false;
						if (!behaviourChosen) {
							untilBulletsEvade = untilBulletsEvadeMin;
						}
					}

					if (turnBehEnabled && !behaviourChosen && timeForTurnAction && !mainGun.IsFiring()) {
						behaviourChosen = true;
						if (tickData.distEdge2Edge > comformDistanceMax || tickData.distEdge2Edge < comformDistanceMin) {
							bool far = tickData.distEdge2Edge > comformDistanceMax;
							accuracyChanger.ExternalChange(-0.2f);
							yield return OutOfComformTurn (far);
						} else {
							yield return ComfortTurn ();
						}
						timeForTurnAction = false;
					}
				}

				if (!behaviourChosen) {
					Shoot (accuracy, bulletsSpeed);
					yield return null;
				}
			} else {
				if (!Main.IsNull (defendObject)) {
					float actDuration = 1.5f;
					float dist = defendObject.polygon.R + thisShip.polygon.R + 15f;
					float angle = UnityEngine.Random.Range (1, 360) * Mathf.Deg2Rad;
					Vector2 defPosition = defendObject.position + dist * new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
					if ((thisShip.position - defPosition).sqrMagnitude < thisShip.originalMaxSpeed * actDuration) {
						Brake ();
						shooting = false;
						yield return new WaitForSeconds (0.5f);
						checkBehTime -= 0.5f;
					} else {
						newDir = defPosition - thisShip.position;
						yield return thisShip.StartCoroutine (SetFlyDir (newDir, actDuration)); 
						checkBehTime -= actDuration;
					}
				} else {
					yield return NoTargetBeh (0.5f);
					checkBehTime -= 0.5f;
				}
			}
		}
	}

	protected virtual IEnumerator NoTargetBeh(float duration) {
		Brake ();
		shooting = false;
		yield return new WaitForSeconds (duration); 
	}


	protected virtual IEnumerator ComfortTurn() {
		float duration;
		Vector2 newDir;
		AIHelper.ComfortTurn (comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
		yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
	}

	protected virtual IEnumerator OutOfComformTurn(bool far) {
		float duration;
		Vector2 newDir;
		AIHelper.OutOfComformTurn (thisShip, comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
		yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
	}
}


