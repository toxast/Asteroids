using UnityEngine;
using System.Collections;

//TODO: no targets on asteroid!!!!!!!!!!!!!!!!

public class TargetSystem : ITickable
{
	IPolygonGameObject thisObj;
	private float repeatTargetCheck = 2f;
	private float leftUntilTargetCheck;

	float enemyDetectionRSqr = 100 * 100; 
	float enemyLostRSqr = 200 * 200;


	public TargetSystem(IPolygonGameObject thisObj)
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
			var target = thisObj.target;

			leftUntilTargetCheck = repeatTargetCheck;

			if(Main.IsNull(target))
			{
				NoTargetBeh();
				//TODO: is under attack;
			}
			else
			{
				//потеря, если слишком далеко
				if(IsSqrDistMore(target, enemyLostRSqr))
				{
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

	private bool IsSqrDistLess(IPolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) < Rsqr;
	}

	private bool IsSqrDistMore(IPolygonGameObject t, float Rsqr)
	{
		return SqrDist(t) >= Rsqr;
	}

	private float SqrDist(IPolygonGameObject t)
	{
		return (thisObj.position - t.position).sqrMagnitude;
	}

	private IPolygonGameObject GetClosestTarget()
	{
		int enemyLayer = Main.GetEnemyLayer (thisObj.layer);
		
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
