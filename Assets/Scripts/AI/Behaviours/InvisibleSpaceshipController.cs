using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InvisibleSpaceshipController : BaseSpaceshipController, IGotTarget
{
    List<PolygonGameObject> bullets;
    float bulletsSpeed;
    float comformDistanceMin, comformDistanceMax;
    float accuracy = 0f;
    bool turnBehEnabled = true;
    bool evadeBullets = true;

    float attackDutation = 5f;
    float invisibleDuration = 8f;
	float fadeOutSpeedPerSecond = 2f; 
	float fadeOutAfterHitSpeedPerSecond = 2f; 
	float fadeInSpeedPerSecond = 2f; 
	float currentfadeOutSpeed;
	float fadeOutDuration;
	float fadeInDuration;

    AIHelper.Data tickData = new AIHelper.Data();
	Gun mainGun;

	public InvisibleSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData, InvisibleData invisData) : base(thisShip)
    {
		mainGun = gun;
        thisShip.increaceAlphaOnHitAndDropInvisibility = true;
        attackDutation = invisData.attackDutation;
		invisibleDuration = invisData.invisibleDuration;

		fadeInDuration = invisData.fadeInDuration;
		fadeOutDuration = invisData.fadeOutDuration;
		fadeOutSpeedPerSecond = 1f / invisData.fadeOutDuration;
		fadeInSpeedPerSecond = 1f /invisData.fadeInDuration;
		fadeOutAfterHitSpeedPerSecond = fadeOutSpeedPerSecond / 5f;
		currentfadeOutSpeed = fadeOutSpeedPerSecond;

		this.bulletsSpeed = mainGun.BulletSpeedForAim;
        this.bullets = bullets;
        thisShip.StartCoroutine (ControlInvisibility ());
        thisShip.StartCoroutine (Logic ());

        accuracy = accData.startingAccuracy;
        if (accData.isDynamic) {
            thisShip.StartCoroutine (AccuracyChanger (accData));
        }

		thisShip.StartCoroutine (BehaviourChangeLogic ());
        thisShip.StartCoroutine (BehavioursRandomTiming ());
		comformDistanceMax = mainGun.Range;
        comformDistanceMin = 30;

		float evadeDuration = (90f / thisShip.originalTurnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.originalMaxSpeed * 0.8f);
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
			TickActionVariable (ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
            TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);
            TickActionVariable(ref checkAccelerationAction, ref untilCheckAcceleration, untilCheckAccelerationMin, untilCheckAccelerationMax);
            yield return null;
        }
    }

	bool invisibleBehaviour = true;
	bool shouldBeInvisible = true;
	bool disappering = false; 

	private IEnumerator BehaviourChangeLogic()
	{
		while (true) {
			//fade out
			currentfadeOutSpeed = fadeOutSpeedPerSecond;
			invisibleBehaviour = true;
			shouldBeInvisible = true;
			timeForTurnAction = true;
			disappering = true;
			Debug.LogError ("fade out " + fadeOutDuration);
			yield return new WaitForSeconds (fadeOutDuration);
			disappering = false;
			currentfadeOutSpeed = fadeOutAfterHitSpeedPerSecond;
			//invisible duration
			Debug.LogError ("invisibleDuration " + invisibleDuration);
			yield return new WaitForSeconds (invisibleDuration);
			//fade in
			shouldBeInvisible = false;
			Debug.LogError ("fadeInDuration " + fadeInDuration);
			yield return new WaitForSeconds (fadeInDuration);
			//attack
			invisibleBehaviour = false;
			timeForTurnAction = false;
			untilTurn = new RandomFloat(untilTurnMax * 0.7f, untilTurnMax).RandomValue;
			Debug.LogError ("attackDutation " + attackDutation);
			yield return new WaitForSeconds (attackDutation);
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
                    comformDistanceMin = Mathf.Min(target.polygon.R + thisShip.polygon.R, comformDistanceMax * 0.5f); // TODO on target change
                    tickData.Refresh(thisShip, target);
		
					if (invisibleBehaviour) {
						shooting = false;
					}

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

					if (invisibleBehaviour || (turnBehEnabled && !behaviourChosen && timeForTurnAction && !mainGun.IsFiring())) {
						behaviourChosen = true;
						if ( disappering && tickData.distEdge2Edge < (comformDistanceMin + comformDistanceMax) * 0.5f) {
							disappering = false;
							yield return CowardAction (tickData, 2, 1f);
						} else {
							if (tickData.distEdge2Edge > comformDistanceMax || tickData.distEdge2Edge < comformDistanceMin) {
								AIHelper.OutOfComformTurn (thisShip, comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
								yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
							} else {
								AIHelper.ComfortTurn (comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
								yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
							}
						}
						timeForTurnAction = false;
					}

                }

				if(!invisibleBehaviour && !behaviourChosen)
				{
					Shoot(accuracy, bulletsSpeed);
				}
				yield return null;
            }
            else
            {
                Brake();
                shooting = false;
                yield return new WaitForSeconds(0.5f); 
                checkBehTime -= 0.5f;
            }
        }
    }

    private IEnumerator AccuracyChanger(AccuracyData data)
    {
        Vector2 lastDir = Vector2.one; //just not zero
        float dtime = data.checkDtime;
        while (true) {
            if (!Main.IsNull (target)) {
                AIHelper.ChangeAccuracy (ref accuracy, ref lastDir, target, data);
            }
            yield return new WaitForSeconds (dtime);
        }
    }


    private IEnumerator ControlInvisibility()
    {
        while (true) {
            var currentAlpha = thisShip.GetAlpha();
			if (shouldBeInvisible) {
				if (currentAlpha > 0) {
					float newAlpha = Mathf.Clamp(currentAlpha - currentfadeOutSpeed * Time.deltaTime, 0f, 1f);
					thisShip.SetAlphaAndInvisibility(newAlpha);
				}
            } else {
				if (currentAlpha < 1) {
					float newAlpha = Mathf.Clamp(currentAlpha + fadeInSpeedPerSecond * Time.deltaTime, 0f, 1f);
					thisShip.SetAlphaAndInvisibility(newAlpha);
				}
            }
            yield return null;
        }
    }
}