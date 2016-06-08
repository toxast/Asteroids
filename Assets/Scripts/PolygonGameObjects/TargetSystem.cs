using UnityEngine;
using System.Collections;

//TODO: no targets on asteroid!!!!!!!!!!!!!!!!
//pass enemy layers?

public class TargetSystem : ITickable
{
	PolygonGameObject thisObj;
	private float repeatTargetCheck = 2f;
	private float leftUntilTargetCheck;

	float enemyDetectionRSqr = 100 * 100;  //todo: pass as parameter? based on guns?
	float enemyLostRSqr = 150 * 150; //todo: pass as parameter? based on guns?


	public TargetSystem(PolygonGameObject thisObj)
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
				//TODO: is under attack;
			}
			else
			{
				//потеря, если слишком далеко
				if(IsSqrDistMore(target, enemyLostRSqr))
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
						if(IsSqrDistLess(t, SqrDist(target)/4f))
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

	private bool IsSqrDistLess(PolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) < Rsqr;
	}

	private bool IsSqrDistMore(PolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) >= Rsqr;
	}

	private float SqrDist(PolygonGameObject t)
	{
		return (thisObj.position - t.position).sqrMagnitude;
	}

	private PolygonGameObject GetClosestTarget()
	{
		//TODO: pass desired layer
		int enemyLayer = CollisionLayers.GetEnemyLayer (thisObj.layer);
		
		var pos = thisObj.position;
		int indx = -1;
		float closeValue = 0;

		var gobjects = Singleton<Main>.inst.gObjects;

		for (int i = 0; i < gobjects.Count; i++)
		{
			var obj = gobjects[i];
			if((enemyLayer & obj.layer) != 0)
			{
				var dir = obj.position - pos;
				float objCloseValue = 100f / dir.sqrMagnitude; //TODO
				
				if(closeValue < objCloseValue)
				{
					indx = i;
					closeValue = objCloseValue;
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
}
