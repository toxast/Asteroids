﻿using UnityEngine;
using System;
using System.Collections;

public class SimpleTower : PolygonGameObject, IGotTarget
{

	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	public event System.Action<ShootPlace, Transform> FireEvent;
	
	private float rangeAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	private float cannonsRotatingSpeed = 50f;
	
	private float currentAimAngle = 0;
	
	private PolygonGameObject target;
	Rotaitor cannonsRotaitor;
	ShootPlace shooter;
	private bool smartAim = false;

	private bool restrictAngle = false;
	private Func<Vector3> anglesRestriction;

	public void Init(ShootPlace shooter, bool smartAim)
	{
		this.smartAim = smartAim;
		this.shooter = shooter;
		
		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		
		StartCoroutine(Aim());
		
		StartCoroutine(FireCoroutine());
	}

	public void SetAngleRestrictions(Func<Vector3> angelsRestriction)
	{
		restrictAngle = true;
		this.anglesRestriction = angelsRestriction;
	}
	
	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}
	
	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		if (target == null)
			return;
		
		RotateCannon(delta);
	}
	
	private void RotateCannon(float deltaTime)
	{
		float angle = currentAimAngle;
		if(restrictAngle)
		{
			Vector3 restrict = anglesRestriction();
			Vector2 dir = restrict;
			float allowed = restrict.z;
			float dangle = Math2d.DeltaAngleGRAD(Math2d.GetRotation(dir)/Math2d.PIdiv180, currentAimAngle);
			if(Mathf.Abs(dangle) < allowed)
			{
				cannonsRotaitor.Rotate(deltaTime, angle);
			}
		}
		else
		{
			cannonsRotaitor.Rotate(deltaTime, angle);
		}
	}
	
	
	private IEnumerator Aim()
	{
		float aimInterval = (smartAim) ? 0.5f : 0f;
		
		while(true)
		{
			if(target != null)
			{
				if(smartAim)
				{
					AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, shooter.speed);
					if(aim.canShoot)
					{
						currentAimAngle = aim.directionAngleRAD;
						currentAimAngle /= Math2d.PIdiv180;
					}
				}
				else
				{
					currentAimAngle = Math2d.AngleRAD2(new Vector2(1, 0), target.cacheTransform.position - cacheTransform.position);
					currentAimAngle /= Math2d.PIdiv180;
				}

			}
			yield return new WaitForSeconds(aimInterval);
		}
	}
	
	private IEnumerator FireCoroutine()
	{
		float defaultInterval = shooter.fireInterval;
		float shortInterval = defaultInterval/2f;
		
		float deltaTime = defaultInterval;
		while(true)
		{
			yield return new WaitForSeconds(deltaTime);
			
			if(target != null)
			{
				if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
				{
					Fire();
					deltaTime = defaultInterval;
				}
				else
				{
					deltaTime = shortInterval;
				}
			}
		}
	}
	
	private void Fire()
	{
		if(FireEvent != null)
		{
			FireEvent(shooter, cacheTransform);
		}
	}
}