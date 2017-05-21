using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommonController : BaseSpaceshipController, IGotTarget {

    public CommonController(SpaceShip thisShip, List<PolygonGameObject> bullets, Gun gun, AccuracyData accData) : base(thisShip, accData) {
        float evadeDuration = (90f / thisShip.originalTurnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.originalMaxSpeed * 0.8f);
        var evadeBullets = evadeDuration < 1.2f;
        var turnBehEnabled = evadeDuration < 3f;
           
        Debug.Log(thisShip.name + " evadeDuration " + evadeDuration + " turnBehEnabled: " + turnBehEnabled + " evadeBullets: " + evadeBullets);

		var comformDistanceMax = gun.Range;
		var comformDistanceMin = comformDistanceMax * 0.5f;
        CommonBeh.Data behData = new CommonBeh.Data {
            accuracyChanger = accuracyChanger,
            comformDistanceMax = comformDistanceMax,
            comformDistanceMin = comformDistanceMin,
            getTickData = GetTickData,
            mainGun = gun,
            thisShip = thisShip,
        };

		EvadeBeh evadeBeh = new EvadeBeh(behData);
		logics.Add(evadeBeh);

		if (evadeBullets) {
			DelayFlag cowardDelay = new DelayFlag(true, 12, 20);
			CowardBeh cowardBeh = new CowardBeh(behData, cowardDelay);
			logics.Add(cowardBeh);
		}

		if (evadeBullets) {
			DelayFlag checkBulletsDelay = new DelayFlag(false, 1 , 4);
			EvadeBulletsBeh evadeBulletsBeh = new EvadeBulletsBeh(behData, bullets, checkBulletsDelay);
			logics.Add(evadeBulletsBeh);
		}

		var untilCheckAccelerationMin = evadeDuration / 6f;
		var untilCheckAccelerationMax = untilCheckAccelerationMin * 2f;
		DelayFlag accDelay = new DelayFlag(true, untilCheckAccelerationMin, untilCheckAccelerationMax);
		var attackMin = Mathf.Max(2f, evadeDuration * 2f);
		var attackMax = attackMin * 1.8f;
		ShootBeh shootBeh = new ShootBeh(behData, accDelay, new RandomFloat(attackMin, attackMax));
		if (turnBehEnabled) {
			TurnBeh turnBeh = new TurnBeh (behData, new NoDelayFlag ());
			WeightedBehs shootAndTurn = new WeightedBehs (new List<IBehaviour> { turnBeh, shootBeh }, new List<float> { 8, 1 }, 1, 7);
			logics.Add (shootAndTurn);
		} else {
			logics.Add (shootBeh);
		}
      
		//starting from least prioritized behs, so they would end up in the end of the list
		FlyAroundBeh flyAround = new FlyAroundBeh(behData);
		logics.Add(flyAround);

        AssignCurrentBeh(null);
    }
}
