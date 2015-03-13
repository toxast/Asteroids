using UnityEngine;
using System.Collections;

//TODO: no targets on asteroid!!!!!!!!!!!!!!!!
//pass enemy layers?

public class MissileTargetSystem : ITickable
{
	SpaceShip thisObj;
	private float repeatTargetCheck = 2f;
	private float leftUntilTargetCheck;
	
	float enemyDetectionRSqr = 150 * 150;  //todo: pass as parameter? based on guns?
	float enemiesImportancy = 10f; //1 means - as important as asteroids
	
	public MissileTargetSystem(SpaceShip thisObj)
	{
		this.thisObj = thisObj;
		
		leftUntilTargetCheck = 0;
	}
	
	
	/*
	 * 1) нет цели.
	 * a) если есть кто-то близко
	 * б) если атакует
	 * 
	 * 2) есть цель
	 * а) потеря, если слишком далеко
	 * б) если атакует кто-то другой - переключиться или кто-то рядом а старая цель далеко. 
	*/	
	
	public void Tick(float delta)
	{
		if (leftUntilTargetCheck >= 0)
			leftUntilTargetCheck -= delta;
		
		
		if(leftUntilTargetCheck < 0)
		{
			leftUntilTargetCheck = repeatTargetCheck;
			var target = thisObj.target;

			if(Main.IsNull(target))
			{
				NoTargetBeh();
				//TODO: is under attack;
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
	private bool IsSqrDistLess(IPolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) < Rsqr;
	}

	//TODO: common
	private bool IsSqrDistMore(IPolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) >= Rsqr;
	}

	//TODO: common
	private float SqrDist(IPolygonGameObject t)
	{
		return (thisObj.position - t.position).sqrMagnitude;
	}
	
	public IPolygonGameObject GetClosestTarget()
	{
		var g = thisObj;

		int enemyLayer = Main.GetEnemyLayer(g.layer);
		
		var pos = g.position;
		int indx = -1;
		float closeValue = 0;

		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++)
		{
			var obj = gobjects[i];
			if((g.collision & obj.layer) != 0)
			{
				var dir = obj.position - pos;
				if(dir.sqrMagnitude < enemyDetectionRSqr)
				{
					float objCloseValue = GetCloseValue(g, dir) ;
					
					//inc importancy for enemies
					if((obj.layer & enemyLayer) != 0)
						objCloseValue *= enemiesImportancy;
					
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
	private float GetCloseValue(SpaceShip s, Vector2 dir)
	{
		var angle = Math2d.DeltaAngleGRAD( Math2d.GetRotationG(dir), Math2d.GetRotationG(s.cacheTransform.right));
		float time2rotate = Mathf.Abs(angle) / s.turnSpeed;
		return 1000f / (dir.magnitude * time2rotate);
	}
}

