using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommonController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> bullets;
	float bulletsSpeed;
	float comformDistanceMin, comformDistanceMax;
	float accuracy = 0f;
	bool turnBehEnabled = true;
	bool evadeBullets = true;
	bool isLazerShip = false;

	LazerGun lazerGun = null;
	AIHelper.Data tickData = new AIHelper.Data();

	public CommonController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip)
	{
		this.bulletsSpeed = gun.BulletSpeedForAim;
		this.bullets = bullets;
		thisShip.StartCoroutine (Logic ());

		if (gun is LazerGun) {
			isLazerShip = true;
			lazerGun = gun as LazerGun;
		}

		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			thisShip.StartCoroutine (AccuracyChanger (accData));

		thisShip.StartCoroutine (BehavioursRandomTiming ());
		comformDistanceMax = gun.Range;
		comformDistanceMin = comformDistanceMax * 0.5f;
		
		float evadeDuration = (90f / thisShip.turnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.maxSpeed * 0.8f);
		evadeBullets = evadeDuration < 1.2f;
		turnBehEnabled = evadeDuration < 3f;
		if(turnBehEnabled)		{
			untilTurnMin = Mathf.Max(2f, Mathf.Sqrt(evadeDuration) * 2.5f);
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

					if (!behaviourChosen) {
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
								Debug.LogWarning ("coward action");
								int turnsTotal = UnityEngine.Random.Range (2, 5);
								int turns = turnsTotal;
								while (turns > 0) {
									turns--;
									duration = 3f / turnsTotal + UnityEngine.Random.Range (-0.3f, 0.5f);
									float angle = UnityEngine.Random.Range (120f, 180f);
									newDir = Math2d.RotateVertexDeg (tickData.dirNorm, tickData.evadeSign * angle);
									yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
									tickData.Refresh (thisShip, target);
								}
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

					if (turnBehEnabled && !behaviourChosen && timeForTurnAction && !(isLazerShip && lazerGun.IsFiring())) {
						behaviourChosen = true;
						if (tickData.distEdge2Edge > comformDistanceMax || tickData.distEdge2Edge < comformDistanceMin) {
							AIHelper.OutOfComformTurn (thisShip, comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
							yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
						} else {
							AIHelper.ComfortTurn (comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
							yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
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
					if ((thisShip.position - defPosition).sqrMagnitude < thisShip.maxSpeed * actDuration) {
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
					Brake ();
					shooting = false;
					yield return new WaitForSeconds (0.5f); 
					checkBehTime -= 0.5f;
				}
			}
		}
	}

	private IEnumerator AccuracyChanger(AccuracyData data) {
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while (true) {
			if (!Main.IsNull (target)) {
				AIHelper.ChangeAccuracy (ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds (dtime);
		}
	}

}
