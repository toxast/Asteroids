using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DumbHitterController : BaseSpaceshipController {


    public DumbHitterController(SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData) : base(thisShip, accData) {

		CommonBeh.Data behData = new CommonBeh.Data {
			accuracyChanger = accuracyChanger,
			comformDistanceMax = 50,
			comformDistanceMin = 30,
			getTickData = GetTickData,
			mainGun = null,
			thisShip = thisShip,
		};


		EvadeBulletsBeh evadeBullets = new EvadeBulletsBeh(behData, bullets, new DelayFlag(true, 2, 4));
		logics.Add (evadeBullets);

		TurnForEnemyBeh turn = new TurnForEnemyBeh (behData, new DelayFlag (true, 4, 6), new RandomFloat (30, 70), new RandomFloat (1f, 2.5f));
		logics.Add(turn);

		HitByArcBeh hit = new HitByArcBeh (behData, new NoDelayFlag ());
		logics.Add(hit);

		FlyAroundBeh flyAround = new FlyAroundBeh(behData);
		logics.Add(flyAround);


		AssignCurrentBeh (null);
    }

//    private IEnumerator BehavioursRandomTiming() {
//        while (true) {
//            //TickActionVariable(ref timeForTurnAction, ref untilTurn, untilTurnMin, untilTurnMax);
//            TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);
//            yield return null;
//        }
//    }
//
//    
//
//    private IEnumerator Logic() {
//        shooting = false;
//        accelerating = true;
//
//        float checkBehTimeInterval = 0.1f;
//        float checkBehTime = 0;
//        bool behaviourChosen = false;
//
//        float duration;
//        Vector2 newDir;
//        while (true) {
//            if (!Main.IsNull(target)) {
//				accelerating = true;
//                behaviourChosen = false;
//                checkBehTime -= Time.deltaTime;
//                tickData.Refresh(thisShip, target);
//                if (checkBehTime <= 0) {
//                    checkBehTime = checkBehTimeInterval;
//
//                    if (!behaviourChosen && checkBulletsAction && tickData.dir.sqrMagnitude > 400) {
//                        if (AIHelper.EvadeBullets(thisShip, bullets, out duration, out newDir)) {
//                            behaviourChosen = true;
//                            yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
//                        }
//                        checkBulletsAction = false;
//                        if (!behaviourChosen) {
//                            untilBulletsEvade = untilBulletsEvadeMin;
//                        }
//                    }
//
//                    if (!behaviourChosen && timeForTurnAction) {
//                        behaviourChosen = true;
//                        float angle = Math2d.RandomSign() * UnityEngine.Random.Range(30, 70);
//                        newDir = Math2d.RotateVertex(tickData.dirNorm, angle * Mathf.Deg2Rad);
//                        duration = UnityEngine.Random.Range(1f, 2.5f);
//                        yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
//                        timeForTurnAction = false;
//                    }
//
//                    if (!behaviourChosen) {
//                        var aimVelocity = (target.velocity - thisShip.velocity) * accuracy;
//                        AimSystem aim = new AimSystem(target.position + approachArc, aimVelocity, thisShip.position, thisShip.maxSpeed);
//                        turnDirection = aim.directionDist;
//                        yield return null;
//                    }
//                }
//            } else {
//                Brake();
//                yield return new WaitForSeconds(0.5f);
//                checkBehTime -= 0.5f;
//            }
//        }
//    }
}

public class TurnForEnemyBeh : DelayedActionBeh {
	RandomFloat angle;
	RandomFloat duration;
	public TurnForEnemyBeh(CommonBeh.Data data, IDelayFlag delay, RandomFloat angle, RandomFloat duration) : base(data,delay) {
		this.angle = angle;
		this.duration = duration;
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && TargetNotNull;
	}

	protected override IEnumerator Action ()
	{
		float rangle = Math2d.RandomSign () * angle.RandomValue; //UnityEngine.Random.Range(30, 70);
		var tickData = data.getTickData();
		if (tickData != null) {
			var newDir = Math2d.RotateVertex (tickData.dirNorm, rangle * Mathf.Deg2Rad);
			//var duration = //UnityEngine.Random.Range (1f, 2.5f);
			SetFlyDir (newDir);
			var wait = WaitForSeconds(duration.RandomValue);
			while (wait.MoveNext ()) yield return true;
		}
	}
}

public class HitByArcBeh : DelayedActionBeh {

	Vector2 approachArc = Vector2.zero;

	public HitByArcBeh(CommonBeh.Data data, IDelayFlag delay) : base(data,delay) {
		_passiveTickOthers = true;
	}

	public override bool IsReadyToAct () {
		return base.IsReadyToAct () && TargetNotNull;
	}

	protected override IEnumerator Action () {
		approachArc = Vector2.zero;
		_canBeInterrupted = true;
		if (TargetNotNull) {
			float arcDegrees = Random.Range(100f, 270f);
			float dist = 2 * (target.polygon.R + thisShip.polygon.R) + 20f;
			float arcRadius = Random.Range(dist * 0.7f, dist * 1.2f);
			float duration = (2 * Mathf.PI * arcRadius) * (arcDegrees / 360f) / thisShip.originalMaxSpeed;
			//duration = Random.Range(duration * 0.8f, duration * 1.2f);
			float angleSpeed = arcDegrees / duration;
			float currentDegrees = arcDegrees;
			//Debug.LogWarning ("arc " + duration + " " + arcDegrees);
			float arcRotationRad = Random.Range(1, 360) * Mathf.Deg2Rad;
			while (duration > 0) {
				duration -= DeltaTime();
				currentDegrees -= angleSpeed * DeltaTime();
				var rad = currentDegrees * Mathf.Deg2Rad;
				approachArc = arcRadius * (new Vector2(1,0) - new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
				approachArc = Math2d.RotateVertex(approachArc, arcRotationRad);
				FollowForHit ();
				yield return true;
			}
		}
		_canBeInterrupted = false;
		approachArc = Vector2.zero;
		float noArcDuration = Random.Range(3f, 6f);
		var noArcBeh = AIHelper.TimerR (noArcDuration, DeltaTime, FollowForHit, () => TargetIsNull);
		while (noArcBeh.MoveNext ()) yield return true;
	}


	void FollowForHit(){
		if (TargetIsNull) {
			return;
		}
		var aimVelocity = (target.velocity - thisShip.velocity) * data.accuracyChanger.accuracy;
		AimSystem aim = new AimSystem(target.position + approachArc, aimVelocity, thisShip.position, thisShip.maxSpeed);
		FireAccelerateChange (true);
		FireDirChange(aim.directionDist);
	}
}

