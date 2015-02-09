﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerEnemy : PolygonGameObject
{
	private float detectionDistanceSqr;

	private float rotationSpeed = 30f;

	Gun closestGun = null;
	float currentAimAngle;
	Rotaitor cannonsRotaitor;

	protected override float healthModifier {
		get {
			return base.healthModifier * Singleton<GlobalConfig>.inst.TowerEnemyHealthModifier;
		}
	}

	public void Init()
	{
		cannonsRotaitor = new Rotaitor (cacheTransform, rotationSpeed);

		StartCoroutine(Aim());
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);

		RotateCannon(delta);

		TickGuns (delta);
	}

	private IEnumerator Aim()
	{
		float aimInterval = 0.5f;

		while(true)
		{
			if(target != null)
			{
				AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, guns[0].bulletSpeed);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
					float minAngle = 360;

					foreach (var gun in guns) 
					{
						float shooterAngle = GunAngle(gun) + transform.eulerAngles.z;
						float dangle = Math2d.DeltaAngleGRAD(currentAimAngle, shooterAngle);

						float absAngle = Mathf.Abs(dangle);
						if(absAngle < minAngle)
						{
							minAngle = absAngle;
							closestGun = gun;
						}
					}
					currentAimAngle -= GunAngle(closestGun);
				}
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private float GunAngle(Gun p)
	{
		return Math2d.AngleRAD2 (new Vector2 (1, 0), p.place.pos) / Math2d.PIdiv180;
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
	}

	private void TickGuns(float delta)
	{
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].Tick(delta);
		}
		
		if(target != null && closestGun != null)
		{
			closestGun.ShootIfReady();
		}
	}

}
