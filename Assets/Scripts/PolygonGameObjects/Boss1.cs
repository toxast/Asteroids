using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI that follows and shooting at the target while it is in "fireRange"
/// If gets too close ("closeRange") - stops acceleration
/// </summary>
public class Boss1 : InputController 
{
	PolygonGameObject thisShip;
	PolygonGameObject target;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;
	float fireRange = 200f;
	float fireRangeSqr;
	float closeRange = 30f;
	float closeRangeSqr;
	public Boss1(PolygonGameObject thisShip)
	{
		this.thisShip = thisShip;
		closeRangeSqr = closeRange * closeRange;
		fireRangeSqr = fireRange * fireRange;
		thisShip.StartCoroutine (Logic ());
	}
	
	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}
	
	public void Tick(PolygonGameObject p){}
	
	private IEnumerator Logic()
	{
		while(true)
		{
			if(target != null)
			{
				Vector2 dir = target.cacheTransform.position - thisShip.cacheTransform.position;
				turnDirection = dir;
				if(dir.SqrMagnitude < fireRangeSqr)
				{
					bool acc = (dir.SqrMagnitude > closeRangeSqr);
					Attack(acc, 0);
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
	
	
	private IEnumerator Attack(bool acceleration, float duration)
	{
		accelerating = acceleration;
		shooting = true;
		
		while(duration >= 0)
		{
			if(target == null)
				yield break;
			
			yield return new WaitForSeconds(0);
			duration -= Time.deltaTime;
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
