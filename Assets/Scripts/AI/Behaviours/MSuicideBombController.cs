using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

//TODO: randomize behaviour a bit
public class MSuicideBombController : BaseSpaceshipController {
    //AIHelper.Data tickData = new AIHelper.Data();
	MSuicideBombSpaceshipData data;
    float chargeDuration;
    float chargingDist;
    float accuracy;
    //float explosionRange = 30f;
	float delayBeforeExplode;

	bool timeForTurnAction = false;
	float untilTurn = 0f;
	float untilTurnMax = 4.5f;
	float untilTurnMin = 2.5f;

	public MSuicideBombController(SpaceShip thisShip, MSuicideBombSpaceshipData data) : base(thisShip) {
        if (thisShip.deathAnimation == null) {
            Debug.LogError("MSuicideBombController has to have explosion death");
            //explosionRange = 30f;
        } else {
            //explosionRange = thisShip.deathAnimation.GetFinalExplosionRadius();
        }
		this.data = data;
		delayBeforeExplode = data.delayBeforeExplode;

		var accData = data.accuracy;
        accuracy = accData.startingAccuracy;
        if (accData.isDynamic) {
            thisShip.StartCoroutine(AccuracyChanger(accData));
        }
        thisShip.StartCoroutine(Logic());

		thisShip.StartCoroutine (BehavioursRandomTiming ());
    }

	private IEnumerator BehavioursRandomTiming()
	{
		while(true) {
			TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
			yield return null;
		}
	}

    private IEnumerator Logic() {
        shooting = false;
        accelerating = true;
        while (true) {
            if (!Main.IsNull(target)) {
				var aim = new SuicideAim (target, thisShip, accuracy);
				float angle = aim.angleBetweenCurrentAndBestSpeed;
				turnDirection = aim.direction;
				if(aim.canShoot) {
                    SetAcceleration(true);
					float dtime = delayBeforeExplode;
					float approximateTime = aim.time;
//                    Debug.LogWarning("approximateTime " + approximateTime + " " + angle);
					if (approximateTime < dtime) {
						SetAcceleration (false);
						var timerEffect = data.explodeTimerEffect.Clone ();
						timerEffect.overrideSize = (2f * thisShip.polygon.R) * 1.8f;
						thisShip.AddParticles (new List<ParticleSystemsData>{ timerEffect });
						yield return new WaitForSeconds (dtime);
						thisShip.Kill ();
						yield break;
					} else {
						if (timeForTurnAction) {
							AIHelper.Data tickData = new AIHelper.Data();
							tickData.Refresh (thisShip, target);
							timeForTurnAction = false;
							float angle2 = UnityEngine.Random.Range (30, 80) * Math2d.RandomSign();
							var newDir = Math2d.RotateVertexDeg(tickData.dirNorm, angle2);
							var duration =  UnityEngine.Random.Range(0.8f, 1.5f);
							yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration)); 
						}
					}
                    yield return null;
				} else {
					if (thisShip.velocity.magnitude < thisShip.maxSpeed * 0.4f || (angle > 140f && Math2d.ClosestAngleBetweenNormalizedRad(thisShip.cacheTransform.right, aim.direction.normalized) * Mathf.Rad2Deg < 20f)) {
                        SetAcceleration(true);
                    } else {
                        Brake();
                    }
                    yield return null;
                }
 
            } else {
                Brake();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator AccuracyChanger(AccuracyData data) {
        Vector2 lastDir = Vector2.one; //just not zero
        float dtime = data.checkDtime;
        while (true) {
            if (!Main.IsNull(target)) {
                AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
            }
            yield return new WaitForSeconds(dtime);
        }
    }
}
