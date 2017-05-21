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

		CommonBeh.Data behData = new CommonBeh.Data {
			accuracyChanger = accuracyChanger,
			comformDistanceMax = 50,
			comformDistanceMin = 30,
			getTickData = GetTickData,
			mainGun = null,
			thisShip = thisShip,
		};

		TakeChargePositionBeh chargePosBeh = new TakeChargePositionBeh (behData, new NoDelayFlag (), chargingDist);
		RotateOnTargetBeh rotateOnTarget = new RotateOnTargetBeh (behData, new NoDelayFlag (), getAimDirection);
		ChargeAttckBeh chargeAttack = new ChargeAttckBeh (behData, new NoDelayFlag (), getAimDirection, chargeDuration, chData.shootWhenCharge, chData.chargeEffect);
		SequentialBehs mainLogic = new SequentialBehs (new List<IBehaviour>{ chargePosBeh, rotateOnTarget, chargeAttack });
		logics.Add(mainLogic);

		FlyAroundBeh flyAround = new FlyAroundBeh(behData);
		logics.Add(flyAround);

		AssignCurrentBeh (null);
    }

    private Vector2 getAimDirection() {
		if (Main.IsNull(target)) {
			return turnDirection;
		}
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
