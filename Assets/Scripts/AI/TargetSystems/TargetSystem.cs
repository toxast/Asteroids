using UnityEngine;
using System.Collections;
public class TargetSystem : TargetSystemBase<PolygonGameObject>
{
	float enemyLostRSqr = 100 * 100; 
	public TargetSystem (PolygonGameObject thisObj) : base (thisObj, 1.5f)
	{
	}

	protected override PolygonGameObject IsShouldLooseTheTargetForTheOther () {
		if (IsSqrDistMore (curTarget, enemyLostRSqr)) {
			var t = GetTheClosestTarget ();
			if (t != null && t != curTarget) {
				if (t.priority > curTarget.priority || IsSqrDistLess (t, SqrDist (curTarget) / 4f)) {
					return t;
				}
			}
		}
		return curTarget;
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		return dir.sqrMagnitude;
	}
}
