using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

//TODO: randomize behaviour a bit
public class MSuicideBombController : BaseSpaceshipController {
    //AIHelper.Data tickData = new AIHelper.Data();

    float chargeDuration;
    float chargingDist;
    float accuracy;
    //float explosionRange = 30f;
	float delayBeforeExplode;

	public MSuicideBombController(SpaceShip thisShip, MSuicideBombSpaceshipData data) : base(thisShip) {
        if (thisShip.deathAnimation == null) {
            Debug.LogError("MSuicideBombController has to have explosion death");
            //explosionRange = 30f;
        } else {
            //explosionRange = thisShip.deathAnimation.GetFinalExplosionRadius();
        }

		delayBeforeExplode = data.delayBeforeExplode;

		var accData = data.accuracy;
        accuracy = accData.startingAccuracy;
        if (accData.isDynamic) {
            thisShip.StartCoroutine(AccuracyChanger(accData));
        }
        thisShip.StartCoroutine(Logic());
    }

    private IEnumerator Logic() {
        shooting = false;
        accelerating = true;
        while (true) {
            if (!Main.IsNull(target)) {
				var aim2 = new SuicideAim (target, thisShip, accuracy);
				float angle = aim2.angleBetweenCurrentAndBestSpeed;
				turnDirection = aim2.direction;
				if(aim2.canShoot) {
                    SetAcceleration(true);
					float dtime = delayBeforeExplode;
					float approximateTime = aim2.time;
//                    Debug.LogWarning("approximateTime " + approximateTime + " " + angle);
                    if (approximateTime < dtime) {
						SetAcceleration(false);
						yield return new WaitForSeconds(dtime);
                        thisShip.Kill();
                        yield break;
                    }
                    yield return null;
				}
                 else {
					if (thisShip.velocity.magnitude < thisShip.maxSpeed * 0.4f || (angle > 140f && Math2d.ClosestAngleBetweenNormalizedRad(thisShip.cacheTransform.right, aim2.direction.normalized) * Mathf.Rad2Deg < 20f)) {
//                        Debug.LogWarning("SetAcceleration backward " + angle);
                        SetAcceleration(true);
                    } else {
//                        Debug.LogWarning("Brake " + angle);
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
