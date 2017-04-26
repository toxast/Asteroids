using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ChargerController : BaseSpaceshipController
{
	MChargerSpaseshipData chData;

    float chargeDuration;
    float chargingDist;

	public ChargerController (SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData, MChargerSpaseshipData chData) : base(thisShip, accData)
	{
		//logs = true;

		this.chData = chData;
		float newMaxSpeed = thisShip.originalMaxSpeed * chData.chargeEffect.multiplyMaxSpeed;
		float newThrust = thisShip.thrust * chData.chargeEffect.multiplyThrust;
		chargeDuration = chData.chargeEffect.duration;
		chargingDist = Math2d.GetDistance (chargeDuration, 0, newMaxSpeed, newThrust);
		LogWarning ("chargingDist " + chargingDist);
        thisShip.StartCoroutine (Logic ());
    }

	private IEnumerator Logic()
	{
		shooting = false;
		accelerating = true;
		float duration;
		Vector2 newDir;
		while (true) {
			if (!Main.IsNull (target)) {
				GetDirectionForChargePosition (out newDir, out duration);
				yield return thisShip.StartCoroutine (SetFlyDir (newDir, duration, accelerating: true));
				Brake ();
				duration = Random.Range (180f - 60f, 180f - 30f) / thisShip.originalTurnSpeed;
				yield return RotateOnTarget (duration);

				yield return thisShip.StartCoroutine (ChargeAttack (chargeDuration));
			} else {
				Brake ();
				yield return new WaitForSeconds (0.5f); 
			}
		}
	}

	private void GetDirectionForChargePosition(out Vector2 dir, out float time) {
		
		float estimateSeconds = Random.Range(1f,2f);
		Vector2 estimateTargetPos = target.position + estimateSeconds * target.velocity;

		float maxTravelDuration = 3f;
		float R = chargingDist * 0.5f;

		//now try to position ship at R distance away from estimateTargetPos in less then maxTravelDuration.
		float maxTravelDistance = Math2d.GetDistance (maxTravelDuration, 0, thisShip.originalMaxSpeed, thisShip.thrust);

		Vector2 estimateDir = estimateTargetPos - thisShip.position;
		Vector2 dirNorm = estimateDir.normalized;
		float distToTarget = estimateDir.magnitude;

		float d = maxTravelDistance;
		if (distToTarget - maxTravelDistance > R) { //too far from target
			dir = dirNorm;
			time = maxTravelDuration;
		} else if(distToTarget + maxTravelDistance < R){ //too close to target
			dir = -dirNorm;
			time = maxTravelDuration;
		} else {
			float maxTravelAngle = Mathf.Acos((Mathf.Abs(R*R - d*d - distToTarget*distToTarget)) / (2f * d * distToTarget));
			LogWarning("maxTravelAngle " + Mathf.Rad2Deg * maxTravelAngle);
			Vector2 travelDirNorm = dirNorm * Mathf.Sign (distToTarget - R);
			float travelAngle = Random.Range (-maxTravelAngle, maxTravelAngle);
			dir = Math2d.RotateVertex(travelDirNorm, travelAngle);
			float delta = Mathf.Abs (distToTarget - R);
			float travelDist = delta + (d - delta) * (travelAngle/maxTravelAngle);
			travelDist = Mathf.Abs (travelDist);
			time = Math2d.GetDuration (travelDist, 0, thisShip.originalMaxSpeed, thisShip.thrust);
			//Debug.DrawLine (thisShip.position, thisShip.position + dir * travelDist, Color.magenta, 5f);
			//Debug.DrawLine (target.position, estimateTargetPos, Color.green, 5f);
		}

		LogWarning("fly to charge position " + time);
		time = Mathf.Clamp (time, 0, maxTravelDuration);
		LogWarning("fly to charge position " + time);
	}


	private IEnumerator RotateOnTarget(float duration) {
		LogWarning("rotate on target " + duration);
		while (duration > 0 && !Main.IsNull(target)) {
			turnDirection = getAimDirection();
			duration -= Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator UncontrollableRotation(float angle) {
		SetAcceleration(false);

		float uncontrollableRotateDuration = angle / thisShip.originalTurnSpeed;
		LogWarning("rotate uncontrollable " + uncontrollableRotateDuration);
		turnDirection = Vector2.zero;
		while (uncontrollableRotateDuration > 0) {
			uncontrollableRotateDuration -= Time.deltaTime;
			if (thisShip.lastRotationDirLeft) {
				thisShip.TurnLeft(Time.deltaTime);
			} else {
				thisShip.TurnRight(Time.deltaTime);
			}
			yield return null;
		}
	}

    private IEnumerator ChargeAttack(float duration) {
		LogWarning("charge attack " + duration);
        if (chData.shootWhenCharge) {
            shooting = true;
        }
		bool allowBreak = Math2d.Chance (0.5f);
        SetAcceleration(true);
		//chData.chargeEffect.duration = duration;
		var effect = new PhysicalChangesEffect (chData.chargeEffect);
		thisShip.AddEffect (effect);
        float startingDuration = duration;
        while (duration > 0 && !Main.IsNull(target)) {
            turnDirection = getAimDirection();
            duration -= Time.deltaTime;
			if (allowBreak && duration < startingDuration * 0.5f && IsBetterToStop()) {
                LogWarning("break charge");
				effect.ForceFinish ();
				if (Math2d.Chance (0.5f)) {
					float angle = Random.Range (200f, 320f);
					yield return UncontrollableRotation (angle);
				}
                break;
            }
            yield return null;
        }
		effect.ForceFinish ();
		LogWarning("charge finished");
        shooting = false;
    }

	private bool IsBetterToStop() {
		if (Main.IsNull (target))
			return true;

		var toTarget = target.position - thisShip.position;
		return (Vector2.Dot (thisShip.velocity, toTarget) < 0);
	}

    private Vector2 getAimDirection() {
        var targetPos = target.position;
        if(chData.aimOffset != 0) {
            if (target.velocity != Vector2.zero) {
                targetPos += target.velocity.normalized * chData.aimOffset;
            } else {
                targetPos += (Vector2)target.cacheTransform.right * chData.aimOffset;
            }
        }
        SuicideAim aim = new SuicideAim(targetPos, target.velocity, thisShip.position, thisShip.velocity, thisShip.turnSpeed, accuracy);
        return aim.direction;
    }
}

