using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SuicideController : BaseSpaceshipController
{
	List<PolygonGameObject> bullets;

	AIHelper.Data tickData = new AIHelper.Data();


    float chargeDuration = 2.5f;
    float speedMultiplier = 2;
    float thrustMultiplier = 2;
    float chargingDist;

 //   bool timeForTurnAction = false;
	//float untilTurn = 0f;
	//float untilTurnMax = 3.5f;
	//float untilTurnMin = 1.5f;
	
	//bool checkBulletsAction = false;
	//float untilBulletsEvade = 1f;
	//float untilBulletsEvadeMax = 3f;
	//float untilBulletsEvadeMin = 1f;
	float accuracy;

    float agility;

    public SuicideController (SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData) : base(thisShip)
	{
		this.bullets = bullets;

		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			thisShip.StartCoroutine (AccuracyChanger (accData));

		thisShip.StartCoroutine (Logic ());

        float chargeToMaxDuration = thisShip.maxSpeed * speedMultiplier / (thisShip.thrust * thrustMultiplier);
        chargingDist = thisShip.thrust * chargeToMaxDuration * chargeToMaxDuration * 0.5f;

        chargeDuration = chargeToMaxDuration + 1f;

        agility = 360f / thisShip.turnSpeed + 80f / thisShip.maxSpeed; 
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
			durations = new List<float>{1.7f, 2f, 1.7f };
		}

		var indx = Math2d.Roll (weights);
		if(indx == 0) {
			float angle = Random.Range(0f, 35f) * Mathf.Sign(Random.Range(-1f, 1f));
			dir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
		} else if( indx == 2) {
			float angle = Random.Range(0f, 35f) * Mathf.Sign(Random.Range(-1f, 1f));
			dir = Math2d.RotateVertex(-tickData.dirNorm, angle * Mathf.Deg2Rad);
		} else {
			float angle = Random.Range(70f, 110f) * Mathf.Sign(Random.Range(-1f, 1f));
			dir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
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
        thisShip.maxSpeed *= speedMultiplier;
        thisShip.thrust *= thrustMultiplier;

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

        thisShip.maxSpeed /= speedMultiplier;
        thisShip.thrust /= thrustMultiplier;
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
        return aim.direction;
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
