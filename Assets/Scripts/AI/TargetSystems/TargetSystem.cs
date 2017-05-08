using UnityEngine;
using System.Collections;
public class TargetSystem : TargetSystemBase<PolygonGameObject>
{
	float enemyLostRSqr = 23 * 23; 
	public TargetSystem (PolygonGameObject thisObj) : base (thisObj, 1.5f)
	{
	}

	protected override PolygonGameObject IsShouldLooseTheTargetForTheOther () {
		var t = GetTheClosestTarget ();
		if (t != null && t != curTarget) {
			if (t.priority > curTarget.priority) {
				return t;
			}
			//the bigger dist value is - the less priority is the target
			if(GetDistValue(curTarget) > enemyLostRSqr && GetPrioritizedDistValue (curTarget) > GetPrioritizedDistValue (t) / 4f){
				return t;
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


public class UserTargetSystem : TargetSystem{

	public UserTargetSystem (PolygonGameObject thisObj) : base (thisObj) {
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		if (obj is Comet) {
			return 0.0001f;
		} else {
			return base.GetDistValue (obj);
		}
	}
}