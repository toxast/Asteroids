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
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }
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

	private IEnumerator Logic()
	{
		while(true)
		{
			if(!Main.IsNull(target))
			{
				Vector2 dir = target.position - thisShip.position;
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

}
