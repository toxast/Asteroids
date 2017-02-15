using UnityEngine;
using System.Collections;
public class TargetSystem : TargetSystemBase<PolygonGameObject>
{
	float enemyLostRSqr = 100 * 100; 
	public TargetSystem (PolygonGameObject thisObj) : base (thisObj, 1.5f)
	{
	}

	protected override PolygonGameObject IsShouldLooseTheTargetForTheOther () {
		if (IsSqrDistMore (thisObj.target, enemyLostRSqr)) {
			var t = GetTheClosestTarget ();
			if (t != null && t != thisObj.target) {
				if (IsSqrDistLess (t, SqrDist (thisObj.target) / 4f)) {
					return t;
				}
			}
		}
		return thisObj.target;
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		return dir.sqrMagnitude;
	}
}
