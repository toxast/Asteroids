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
		thisShip.StartCoroutine (BehavioursRandomTiming ());

        float chargeToMaxDuration = thisShip.maxSpeed * speedMultiplier / (thisShip.thrust * thrustMultiplier);
        chargingDist = thisShip.thrust * chargeToMaxDuration * chargeToMaxDuration * 0.5f;

        chargeDuration = chargeToMaxDuration + 1f;

        agility = 360f / thisShip.turnSpeed + 80f / thisShip.maxSpeed; 
    }

	private IEnumerator BehavioursRandomTiming()
	{
		while(true)
		{
			//TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
			//TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);
			yield return null;
		}
	}

	private IEnumerator Logic()
	{
		shooting = false;
		accelerating = true;

		//float checkBehTimeInterval = 0.1f;
		//float checkBehTime = 0;
		//bool behaviourChosen = false;

		float duration;
		Vector2 newDir;
		while(true)
		{
			if(!Main.IsNull(target))
			{
                //fly to charge position
				tickData.Refresh(thisShip, target);
                if(tickData.distEdge2Edge > chargingDist * 1.2f) {
                    float angle = Random.Range(0f, 35f) * Mathf.Sign(Random.Range(-1f, 1f));
                    newDir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
                } else if(tickData.distEdge2Edge < chargingDist * 0.8f) {
                    float angle = Random.Range(0f, 35f) * Mathf.Sign(Random.Range(-1f, 1f));
                    newDir = Math2d.RotateVertex(-tickData.dirNorm, angle * Mathf.Deg2Rad);
                } else {
                    float angle = Random.Range(70f, 110f) * Mathf.Sign(Random.Range(-1f, 1f));
                    newDir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
                }
                duration = agility * 0.4f;
                duration = Random.Range(duration * 0.8f, duration * 1.2f);
                Debug.LogWarning("fly to charge position " + duration);
                yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration, accelerating: true));

                if (!Main.IsNull(target)) {
                    tickData.Refresh(thisShip, target);
                    //if (IsBetterToStop())
                    {
                        Brake();
                        Debug.LogWarning("brake");
                    }
                    //rotate on target
                    duration = Random.Range(180f - 30f, 180f + 30f) / thisShip.turnSpeed;
                    Debug.LogWarning("rotate on target " + duration);
                    while (duration > 0 && !Main.IsNull(target)) {
                        turnDirection = getAimDiraction();
                        duration -= Time.deltaTime;
                        yield return null;
                    }

                    //charge attack
                    Debug.LogWarning("charge attack " + chargeDuration);
                    yield return thisShip.StartCoroutine(ChargeAttack(chargeDuration));

                    //rotate uncontrollable for few seconds
                    SetAcceleration(false);
                    if (IsBetterToStop()) {
                        Debug.LogWarning("brake");
                        Brake();
                    }
                    float uncontrollableRotateDuration = Random.Range(200f, 320f) / thisShip.turnSpeed;
                    Debug.LogWarning("rotate uncontrollable " + uncontrollableRotateDuration);
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
			}
			else
			{
				Brake();
				yield return new WaitForSeconds(0.5f); 
			}
		}
	}

    private bool IsBetterToStop() {
        float deltaSecond = 0.01f;
        float sqrDistInOneSecond = (target.position + deltaSecond * target.velocity - (thisShip.position + deltaSecond * thisShip.velocity)).sqrMagnitude;
        float sqrDistInOneSecondIfStop = (target.position + deltaSecond * target.velocity - thisShip.position).sqrMagnitude;
        return sqrDistInOneSecondIfStop < sqrDistInOneSecond;
    }
    
    private IEnumerator ChargeAttack(float duration) {
        
        SetAcceleration(true);
        thisShip.maxSpeed *= speedMultiplier;
        thisShip.thrust *= thrustMultiplier;
        float startingDuration = duration;
        while (duration > 0 && !Main.IsNull(target)) {
            turnDirection = getAimDiraction();
            duration -= Time.deltaTime;
            if (duration < startingDuration * 0.5f && IsBetterToStop()) {
                Debug.LogWarning("break charge");
                break;
            }
                
            yield return null;
        }

        thisShip.maxSpeed /= speedMultiplier;
        thisShip.thrust /= thrustMultiplier;
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
