using UnityEngine;
using System.Collections;
using System;

//TODO: no targets on asteroid!!!!!!!!!!!!!!!!
//pass enemy layers?

public class TurretTargetSystem : ITickable
{
	PolygonGameObject thisObj;
	private float repeatTargetCheck = 1f;
	private float leftUntilTargetCheck;
	
	float enemyDetectionRSqr;
//	float enemiesImportancy = 10f; //1 means - as important as asteroids

	float rotationSpeed;
	Func<Vector3> angelsRestriction;

	public TurretTargetSystem(PolygonGameObject thisObj, float rotationSpeed, Func<Vector3> angelsRestriction, float repeatTargetCheck)
	{
		this.thisObj = thisObj;
		this.rotationSpeed = rotationSpeed;
		this.angelsRestriction = angelsRestriction;
		this.repeatTargetCheck = repeatTargetCheck;
		enemyDetectionRSqr = 1.2f * thisObj.guns [0].Range;
		enemyDetectionRSqr *= enemyDetectionRSqr;

		leftUntilTargetCheck = 0;
	}

	bool hasTarget = false;
	public void Tick(float delta)
	{
		var target = thisObj.target;
		
		bool haveTargetNow = !Main.IsNull(target);
		if(hasTarget && !haveTargetNow)
		{
			leftUntilTargetCheck = 0;
		}
		hasTarget = !Main.IsNull(target);
		
		
		if (leftUntilTargetCheck >= 0)
			leftUntilTargetCheck -= delta;
		
		
		if(leftUntilTargetCheck < 0)
		{
			leftUntilTargetCheck = repeatTargetCheck;
			
			if(!haveTargetNow)
			{
				NoTargetBeh();
			}
			else
			{
				Vector3 restrict = angelsRestriction ();
				Vector2 restrictDir = restrict;
				float allowed = restrict.z;
				Vector2 dir = target.position - thisObj.position;
				//потеря, если вне зоны 
				if(!InHitZone(dir, restrictDir, allowed))
				{
					thisObj.SetTarget(null);
					NoTargetBeh();
				}
				else
				{
					var t = GetClosestTarget();
					//переключиться если кто-то рядом а старая цель далеко. 
					if(t != null && t != target)
					{
						Vector2 dirt = t.position - thisObj.position;
						var curDelta = Vector3.Angle(thisObj.cacheTransform.right, dir);
						if(curDelta > 5 && 2*Vector3.Angle(thisObj.cacheTransform.right, dirt) < curDelta)
						{
							thisObj.SetTarget(t);
						}
					}
				}
			}
		}
	}
	
	private void NoTargetBeh()
	{
		var t = GetClosestTarget();
		if(t != null && IsSqrDistLess(t, enemyDetectionRSqr))
		{
			thisObj.SetTarget(t);
		}
	}
	
	//TODO: common
	private bool IsSqrDistLess(PolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) < Rsqr;
	}
	
	//TODO: common
	private bool IsSqrDistMore(PolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) >= Rsqr;
	}
	
	//TODO: common
	private float SqrDist(PolygonGameObject t)
	{
		return (thisObj.position - t.position).sqrMagnitude;
	}

	private bool InHitZone(Vector2 dir, Vector2 restrictDir, float allowed)
	{
		return (dir.sqrMagnitude < enemyDetectionRSqr) && (Vector3.Angle (restrictDir, dir) < allowed);
	}
	
	public PolygonGameObject GetClosestTarget()
	{
		var g = thisObj;
		
		int enemyLayer = CollisionLayers.GetEnemyLayer(g.layer);
		
		var pos = g.position;
		int indx = -1;
		float closeValue = 0;

		Vector3 restrict = angelsRestriction ();
		Vector2 restrictDir = restrict;
		float allowed = restrict.z;
		
		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++)
		{
			var obj = gobjects[i];
			if((enemyLayer & obj.layer) != 0)
			{
				var dir = obj.position - pos;
				if(InHitZone(dir, restrictDir, allowed))
				{
					float objCloseValue = GetCloseValue(dir) ;
					
					//inc importancy for enemies
//					if((obj.layer & enemyLayer) != 0)
//						objCloseValue *= enemiesImportancy;
					
					if(closeValue < objCloseValue)
					{
						indx = i;
						closeValue = objCloseValue;
					}
				}
			}
		}
		
		if(indx >= 0)
		{
			return gobjects[indx];
		}
		else
		{
			return null;
		}
	}
	
	//the more the value the better target is
	private float GetCloseValue(Vector2 dir)
	{
		var angle = Math2d.DeltaAngleDeg( Math2d.GetRotationDg(dir), Math2d.GetRotationDg(thisObj.cacheTransform.right));
		float time2rotate = Mathf.Abs(angle) / rotationSpeed;
		return 1000f / (Mathf.Pow(dir.sqrMagnitude, 0.25f) * time2rotate);
	}
}

