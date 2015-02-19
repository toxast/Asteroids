using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI that follows and shooting at the target while it is in "fireRange"
/// If gets too close ("closeRange") - stops acceleration
/// </summary>
public class SimpleAI1 : InputController, IGotTarget
{
	PolygonGameObject thisShip;
	IPolygonGameObject target;
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;
	float fireRange = 60f;
	float fireRangeSqr;
	float closeRange = 30f;
	float closeRangeSqr;
	public SimpleAI1(PolygonGameObject thisShip)
	{
		this.thisShip = thisShip;
		closeRangeSqr = closeRange * closeRange;
		fireRangeSqr = fireRange * fireRange;
		thisShip.StartCoroutine (Logic ());
	}
	
	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}
	
	public void Tick(PolygonGameObject p){}

	public bool IsBraking()
	{
		return braking;
	}

	private IEnumerator Logic()
	{
		while(true)
		{
			if(!Main.IsNull(target))
			{
				Vector2 dir = target.cacheTransform.position - thisShip.cacheTransform.position;
				turnDirection = dir;
				if(dir.sqrMagnitude < fireRangeSqr)
				{
					accelerating = (dir.sqrMagnitude > closeRangeSqr);
					shooting = true;
				}
				else
				{
					accelerating = false;
					shooting = false;
				}
			}
			else
			{
				accelerating = false;
				shooting = false;
				//TODO: break
			}
			yield return new WaitForSeconds(0f);
		}
	}
	
	public Vector2 TurnDirection ()
	{
		return turnDirection;
	}
	
	public bool IsShooting()
	{
		return shooting;
	}
	
	public bool IsAccelerating()
	{
		return accelerating;
	}
}
