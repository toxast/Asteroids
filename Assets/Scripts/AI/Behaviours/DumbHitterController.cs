using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DumbHitterController : BaseSpaceshipController {
    List<PolygonGameObject> bullets;

    AIHelper.Data tickData = new AIHelper.Data();

    bool timeForTurnAction = false;
//    float untilTurn = 0f;
//    float untilTurnMax = 3.5f;
//    float untilTurnMin = 1.5f;

    bool checkBulletsAction = false;
    float untilBulletsEvade = 1f;
    float untilBulletsEvadeMax = 3f;
    float untilBulletsEvadeMin = 1f;
    float accuracy;

    public DumbHitterController(SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData) : base(thisShip) {
        this.bullets = bullets;

        accuracy = accData.startingAccuracy;
        if (accData.isDynamic)
            thisShip.StartCoroutine(AccuracyChanger(accData));

        thisShip.StartCoroutine(Logic());
        thisShip.StartCoroutine(BehavioursRandomTiming());
        thisShip.StartCoroutine(ApproachArcGenerator());
    }

    private IEnumerator BehavioursRandomTiming() {
        while (true) {
            //TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
            TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);
            yield return null;
        }
    }

    Vector2 approachArc = Vector2.zero;
    private IEnumerator ApproachArcGenerator() {
        approachArc = Vector2.zero;
        while (true) {
            if (!Main.IsNull(target)) {
                float arcDegrees = Random.Range(100f, 270f);
                float dist = 2 * (target.polygon.R + thisShip.polygon.R) + 20f;
                float arcRadius = Random.Range(dist * 0.7f, dist * 1.2f);
				float duration = (2 * Mathf.PI * arcRadius) * (arcDegrees / 360f) / thisShip.originalMaxSpeed;
                //duration = Random.Range(duration * 0.8f, duration * 1.2f);
				float angleSpeed = arcDegrees / duration;
                float currentDegrees = arcDegrees;
				Debug.LogWarning ("arc " + duration + " " + arcDegrees);
				float arcRotationRad = Random.Range(1, 360) * Mathf.Deg2Rad;
                while (duration > 0) {
                    duration -= Time.deltaTime;
                    currentDegrees -= angleSpeed * Time.deltaTime;
                    var rad = currentDegrees * Mathf.Deg2Rad;
					approachArc = arcRadius * (new Vector2(1,0) - new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
                    approachArc = Math2d.RotateVertex(approachArc, arcRotationRad);
                    yield return null;
                }
            }
            approachArc = Vector2.zero;
			float noArcDuration = Random.Range(3f, 6f);
            yield return new WaitForSeconds(noArcDuration);
        }
    }

    private IEnumerator Logic() {
        shooting = false;
        accelerating = true;

        float checkBehTimeInterval = 0.1f;
        float checkBehTime = 0;
        bool behaviourChosen = false;

        float duration;
        Vector2 newDir;
        while (true) {
            if (!Main.IsNull(target)) {
				accelerating = true;
                behaviourChosen = false;
                checkBehTime -= Time.deltaTime;
                tickData.Refresh(thisShip, target);
                if (checkBehTime <= 0) {
                    checkBehTime = checkBehTimeInterval;

                    if (!behaviourChosen && checkBulletsAction && tickData.dir.sqrMagnitude > 400) {
                        if (AIHelper.EvadeBullets(thisShip, bullets, out duration, out newDir)) {
                            behaviourChosen = true;
                            yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
                        }
                        checkBulletsAction = false;
                        if (!behaviourChosen) {
                            untilBulletsEvade = untilBulletsEvadeMin;
                        }
                    }

                    if (!behaviourChosen && timeForTurnAction) {
                        behaviourChosen = true;
                        float angle = Math2d.RandomSign() * UnityEngine.Random.Range(30, 70);
                        newDir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
                        duration = UnityEngine.Random.Range(1f, 2.5f);
                        yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
                        timeForTurnAction = false;
                    }

                    if (!behaviourChosen) {
                        var aimVelocity = (target.velocity - thisShip.velocity) * accuracy;
                        AimSystem aim = new AimSystem(target.position + approachArc, aimVelocity, thisShip.position, thisShip.maxSpeed);
                        turnDirection = aim.directionDist;
                        yield return null;
                    }
                }
            } else {
                Brake();
                yield return new WaitForSeconds(0.5f);
                checkBehTime -= 0.5f;
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

