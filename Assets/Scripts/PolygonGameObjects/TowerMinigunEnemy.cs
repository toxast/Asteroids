using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//spins fast, counter to the target movement, shoots from every gun. 
//Has recharge duration, taken from gun recharge time
public class TowerMinigunEnemy : PolygonGameObject
{
//	float detectionDistanceSqr;
//	float rotationSpeed;
//	Rotaitor cannonsRotaitor;
//
//	//changable
//	Gun closestGun = null;
//	float currentAimAngle;
//
//	public void Init(MStationTowerData data) {
//		rotationSpeed = data.rotationSpeed.RandomValue;
//		InitPolygonGameObject(data.physical); 
//		cannonsRotaitor = new Rotaitor (cacheTransform, rotationSpeed);
//		StartCoroutine(Aim());
//	}
//
//	public override void Tick(float delta)
//	{
//		base.Tick (delta);
//
//		TickGunsNew (delta);
//	}
//
//	private IEnumerator Aim()
//	{
//		float aimInterval = 0.5f;
//
//		while(true)
//		{
//			if(!Main.IsNull(target) && shooting)
//			{
//				AimSystem aim = new AimSystem(target.position, target.velocity, position, guns[0].BulletSpeedForAim);
//				if(aim.canShoot)
//				{
//					currentAimAngle = aim.directionAngleRAD * Mathf.Rad2Deg;
//					float minAngle = 360;
//
//					foreach (var gun in guns) 
//					{
//						float shooterAngle = GunAngle(gun) + transform.eulerAngles.z;
//						float dangle = Math2d.DeltaAngleDeg(currentAimAngle, shooterAngle);
//
//						float absAngle = Mathf.Abs(dangle);
//						if(absAngle < minAngle)
//						{
//							minAngle = absAngle;
//							closestGun = gun;
//						}
//					}
//				}
//			}
//			yield return new WaitForSeconds(aimInterval);
//		}
//	}
//
//	private float GunAngle(Gun p)
//	{
//		return Math2d.AngleRad (new Vector2 (1, 0), p.place.pos) * Mathf.Rad2Deg;
//	}
//
//
//
//	private void TickGunsNew(float delta)
//	{
//		for (int i = 0; i < guns.Count; i++) 
//		{
//			guns[i].Tick(delta);
//		}
//
//		if (shooting) {
//			if (target != null && closestGun != null) {
//				closestGun.ShootIfReady ();
//			}
//		}
//	}
//
//	float chargeRotation = 300f;
//	float rotationChargingRate = 40f;
//	float shootDuration = 3f;
//	bool shooting = false;
//
//	IEnumerator CheckDistanceAndCharge()
//	{
//		float deltaTime = 0.3f;
//
//		while(true)
//		{
//			yield return new WaitForSeconds(deltaTime); 
//
//			if(!Main.IsNull(target))
//			{
//				if(Mathf.Abs(rotation) > chargeRotation)
//				{
//					shooting = true;
//					yield return WaitForSeconds(shootDuration); 
//					shooting = false;
//					yield return StartCoroutine( Slow() ); 
//				}
//				else
//				{
//					rotation += Mathf.Sign(rotation) * rotationChargingRate * deltaTime;
//				}
//			}
//			else
//			{
//				Slow(deltaTime);
//			}
//		}
//	}
//
//	IEnumerator Shoot()
//	{
////		if(!Main.IsNull(target))
////		{
////			if(aim.canShoot)
////			{
////				velocity = chargeSpeed * aim.direction.normalized;
////			}
////			else
////			{
////				Vector2 direction = target.position - position;
////				velocity = chargeSpeed * direction.normalized;
////			}
////		}
////		yield return new WaitForSeconds(chargeDuration); 
//	}
//
//	private bool Slow(float deltaTime)
//	{
////		float diff = Mathf.Abs (rotation) - Mathf.Abs (initialRotation);
////		float delta = rotationSlowingRate * deltaTime;
////		bool slowingRotation = diff > 0;
////		if(slowingRotation)
////		{
////			if (delta < diff) {
////				rotation -= Mathf.Sign (rotation) * delta;
////			} else {
////				rotation = Mathf.Sign (rotation) * initialRotation;
////				slowingRotation = false;
////			}
////		}
////
////		var vmag = velocity.magnitude;
////		diff = vmag - initialVelocity;
////		delta = velocityslowingRate * deltaTime;
////		bool slowingVelocity = diff > 0;
////		if(slowingVelocity)
////		{
////			if (delta < diff) {
////				velocity = (velocity / vmag) * (vmag - delta);
////			} else {
////				velocity = (velocity / vmag) * initialVelocity;
////				slowingVelocity = false;
////			}
////		}
////
////		return slowingVelocity || slowingRotation;
//	}
//
//	IEnumerator Slow()
//	{
////		float deltaTime = 0.15f;
////
////		bool slowing = true;
////		while(slowing)
////		{
////			slowing = Slow(deltaTime);
////
////			yield return new WaitForSeconds(deltaTime); 
////		}
//	}
//
}
