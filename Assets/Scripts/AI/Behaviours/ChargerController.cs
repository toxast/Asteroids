using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ChargerController : BaseSpaceshipController
{
	AIHelper.Data tickData = new AIHelper.Data();
	MChargerSpaseshipData chData;

    float chargeDuration;
    float chargingDist;
	float accuracy;

	public ChargerController (SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData, MChargerSpaseshipData chData) : base(thisShip)
	{
		this.chData = chData;
		accuracy = accData.startingAccuracy;
		float chargeToMaxDuration = thisShip.maxSpeed * chData.chargeEffect.multiplyMaxSpeed / (thisShip.thrust * chData.chargeEffect.multiplyThrust);
        chargingDist = thisShip.thrust * chargeToMaxDuration * chargeToMaxDuration * 0.5f;
        chargeDuration = chargeToMaxDuration + 1f;
        if (accData.isDynamic) {
            thisShip.StartCoroutine(AccuracyChanger(accData));
        }
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
				duration = Random.Range (180f - 30f, 180f + 30f) / thisShip.turnSpeed;
				yield return RotateOnTarget (duration);

				yield return thisShip.StartCoroutine (ChargeAttack (chargeDuration));
                
				float angle = Random.Range (200f, 320f);
				yield return UncontrollableRotation (angle);
			} else {
				Brake ();
				yield return new WaitForSeconds (0.5f); 
			}
		}
	}

	private void GetDirectionForChargePosition(out Vector2 dir, out float time) {
		List<float> weights;
		List<float> durations;
		//randomize direction behaviour
		tickData.Refresh(thisShip, target);
		if(tickData.distEdge2Edge > chargingDist * 1.2f) {
			weights = new List<float>{ 3, 1, 0 };
			durations = new List<float>{1.4f * (tickData.distEdge2Edge - chargingDist) / thisShip.maxSpeed, 1.7f, 0 };
		} else if(tickData.distEdge2Edge < chargingDist * 0.8f) {
			weights  = new List<float>{ 0, 1, 3 };
			durations = new List<float>{0, 1.7f, 1.4f * (chargingDist - tickData.distEdge2Edge) / thisShip.maxSpeed };
		} else {
			weights  = new List<float>{ 1, 3, 1 };
			durations = new List<float>{1.7f, 2.2f, 1.7f };
		}

		var indx = Math2d.Roll (weights);
		if(indx == 0) {
			float angle = Random.Range(0f, 35f) * Math2d.RandomSign();
			dir = Math2d.RotateVertexDeg(tickData.dirNorm, angle);
		} else if( indx == 2) {
			float angle = Random.Range(0f, 35f) * Math2d.RandomSign();
			dir = Math2d.RotateVertexDeg(-tickData.dirNorm, angle);
		} else {
			float angle = Random.Range(70f, 110f) * Math2d.RandomSign();
			dir = Math2d.RotateVertexDeg(tickData.dirNorm, angle);
		}

		time = Random.Range(durations[indx] * 0.7f, durations[indx] * 1.3f);
		time = Mathf.Clamp (time, 1f, 3f);
		LogWarning("fly to charge position " + time + " indx " + indx);
	}
    
	private IEnumerator RotateOnTarget(float duration) {
		LogWarning("rotate on target " + duration);
		while (duration > 0 && !Main.IsNull(target)) {
			turnDirection = getAimDiraction();
			duration -= Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator UncontrollableRotation(float angle) {
		SetAcceleration(false);
		if (IsBetterToStop()) {
			LogWarning("brake");
			Brake();
		}

		float uncontrollableRotateDuration = angle / thisShip.turnSpeed;
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
		LogWarning("charge attack " + chargeDuration);
        SetAcceleration(true);
		chData.chargeEffect.duration = duration;
		thisShip.AddEffect (new PhysicalChangesEffect(chData.chargeEffect));
        float startingDuration = duration;
        while (duration > 0 && !Main.IsNull(target)) {
            turnDirection = getAimDiraction();
            duration -= Time.deltaTime;
            if (duration < startingDuration * 0.5f && IsBetterToStop()) {
                LogWarning("break charge");
                break;
            }
            yield return null;
        }
    }

	private bool IsBetterToStop() {
		if (Main.IsNull (target))
			return true;
		
		float deltaSecond = 0.01f;
		float sqrDistInDeltaSecond = (target.position + deltaSecond * target.velocity - (thisShip.position + deltaSecond * thisShip.velocity)).sqrMagnitude;
		float sqrDistInDeltaSecondIfStop = (target.position + deltaSecond * target.velocity - thisShip.position).sqrMagnitude;
		return sqrDistInDeltaSecondIfStop < sqrDistInDeltaSecond;
	}

    private Vector2 getAimDiraction() {
        var aimVelocity = accuracy * (1.3f * target.velocity - thisShip.velocity);
        AimSystem aim = new AimSystem(target.position, aimVelocity, thisShip.position, thisShip.maxSpeed);
        return aim.directionDist;
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
}
