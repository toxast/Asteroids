using UnityEngine;
using System;
using System.Collections;

public class SimpleTower : PolygonGameObject
{
	public static Vector2[] vertices = PolygonCreator.CreateTowerVertices2(1, 6);
	
	private float rangeAngle = 20f; //if angle to target bigger than this - dont even try to shoot
	private float cannonsRotatingSpeed = 50f;
	
	private float currentAimAngle = 0;
	
	Rotaitor cannonsRotaitor;
	private bool smartAim = false;

	private bool restrictAngle = false;
	private Func<Vector3> anglesRestriction;

	public void Init(bool smartAim)
	{
		this.smartAim = smartAim;
		
		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);
		
		StartCoroutine(Aim());
	}

	public void SetAngleRestrictions(Func<Vector3> angelsRestriction)
	{
		restrictAngle = true;
		this.anglesRestriction = angelsRestriction;
	}
	
	public override void Tick(float delta)
	{
		base.Tick (delta);
		
		if(Main.IsNull(target))
			return;
		
		RotateCannonWithRestrictions(delta);

		TickGuns (delta);
	}
	
	private void RotateCannonWithRestrictions(float deltaTime)
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
		float aimInterval = 0;//(smartAim) ? 0.2f : 0f;
		
		while(true)
		{
			if(!Main.IsNull(target))
			{
				if(smartAim)
				{
					AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, guns[0].bulletSpeed);
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
	
	private void TickGuns(float delta)
	{
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].Tick(delta);
		}
		
		if(!Main.IsNull(target))
		{
			if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
			{
				for (int i = 0; i < guns.Count; i++) 
				{
					guns[i].ShootIfReady();
				}
			}
		}
	}
}
