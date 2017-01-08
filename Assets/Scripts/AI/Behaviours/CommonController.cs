using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommonController : BaseSpaceshipController, IGotTarget
{
	public PolygonGameObject defendObject;
	List<PolygonGameObject> bullets;
	float bulletsSpeed;
	float comformDistanceMin, comformDistanceMax;
	float accuracy = 0f;
	bool turnBehEnabled = true;
	bool evadeBullets = true;
	bool isLazerShip = false;

	AIHelper.Data tickData = new AIHelper.Data();

	public CommonController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip)
	{
		this.bulletsSpeed = gun.BulletSpeedForAim;
		this.bullets = bullets;
		thisShip.StartCoroutine (Logic ());

		if (gun is LazerGun)
			isLazerShip = true;

		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			thisShip.StartCoroutine (AccuracyChanger (accData));

		thisShip.StartCoroutine (BehavioursRandomTiming ());
		comformDistanceMax = gun.Range;
		comformDistanceMin = 30;
		
		float evadeDuration = (90f / thisShip.turnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.maxSpeed * 0.8f);
		evadeBullets = evadeDuration < 1.2f;
		turnBehEnabled = evadeDuration < 3f;
		if(turnBehEnabled)
		{
			untilTurnMin = Mathf.Max(2f, Mathf.Sqrt(evadeDuration) * 2.5f);
			untilTurnMax = untilTurnMin * 1.8f;
		}

		untilCheckAccelerationMin = evadeDuration / 6f;
		untilCheckAccelerationMax = untilCheckAccelerationMin * 2f;
	}

	bool timeForTurnAction = false;
	float untilTurn = 0f;
	float untilTurnMax = 4.5f;
	float untilTurnMin = 2.5f;

	bool checkBulletsAction = false;
	float untilBulletsEvade = 1f;
	float untilBulletsEvadeMax = 4f;
	float untilBulletsEvadeMin = 1f;

	bool checkAccelerationAction = false;
	float untilCheckAcceleration = 0f;
	float untilCheckAccelerationMax = 0.1f;
	float untilCheckAccelerationMin = 0.0f;

	private IEnumerator BehavioursRandomTiming()
	{
		while(true)
		{
			TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);

			TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);

			TickActionVariable(ref checkAccelerationAction, ref untilCheckAcceleration, untilCheckAccelerationMin, untilCheckAccelerationMax);

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

		while(true)
		{
			if(!Main.IsNull(target))
			{
				behaviourChosen = false;
				checkBehTime -= Time.deltaTime;

				if(checkBehTime <= 0)
				{
					checkBehTime = checkBehTimeInterval;

					comformDistanceMin = Mathf.Min(target.polygon.R + thisShip.polygon.R, comformDistanceMax * 0.7f); // TODO on target change
					//Debug.LogWarning(comformDistanceMin + " " + comformDistanceMax);

					tickData.Refresh(thisShip, target);

					if(checkAccelerationAction)
					{
						checkAccelerationAction = false;
						
						bool iaccelerate = false;
						if(tickData.distEdge2Edge > (comformDistanceMax + comformDistanceMin)/2f)
						{
							iaccelerate = (tickData.vprojThis + tickData.vprojTarget) < 0 ;
						}
						SetAcceleration(iaccelerate);
					}

					if(!behaviourChosen)
					{
						if(AIHelper.EvadeTarget(thisShip, target, tickData, out duration, out newDir))
						{
							behaviourChosen = true;
							yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration)); 
						}
					}

					if (!behaviourChosen && evadeBullets && checkBulletsAction)
					{
						if(AIHelper.EvadeBullets(thisShip, bullets, out duration, out newDir))
						{
							behaviourChosen = true;
							yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
						}
						checkBulletsAction = false;
						if(!behaviourChosen)
						{
							untilBulletsEvade = untilBulletsEvadeMin;
						}
					}

					if(turnBehEnabled && !behaviourChosen && timeForTurnAction && !(isLazerShip && shooting))
					{
						behaviourChosen = true;
						if(tickData.distEdge2Edge > comformDistanceMax || tickData.distEdge2Edge < comformDistanceMin)
						{
							AIHelper.OutOfComformTurn(thisShip, comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
							yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration)); 
						}
						else
						{
							AIHelper.ComfortTurn(comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
							yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration)); 
						}
						timeForTurnAction = false;
					}
				}

				if(!behaviourChosen)
				{
					Shoot(accuracy, bulletsSpeed);
					yield return null;
				}
			}
			else
			{
				if (!Main.IsNull(defendObject)) {
					float actDuration = 1.5f;
					float dist = defendObject.polygon.R + thisShip.polygon.R + 15f;
					float angle = UnityEngine.Random.Range (1, 360) * Mathf.Deg2Rad;
					Vector2 defPosition = defendObject.position + dist * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
					if ((thisShip.position - defPosition).sqrMagnitude < thisShip.maxSpeed * actDuration) {
						Brake ();
						shooting = false;
						yield return new WaitForSeconds (0.5f);
						checkBehTime -= 0.5f;
					} else {
						newDir = defPosition - thisShip.position;
						yield return thisShip.StartCoroutine(SetFlyDir(newDir, actDuration)); 
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

	private IEnumerator AccuracyChanger(AccuracyData data)
	{
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds(dtime);
		}
	}

}
