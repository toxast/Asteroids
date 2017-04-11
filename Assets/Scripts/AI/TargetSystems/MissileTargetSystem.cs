using UnityEngine;
using System.Collections;

public class MissileTargetSystem : TargetSystemBase<SpaceShip>
{
	public MissileTargetSystem (SpaceShip thisObj) : base (thisObj, 0.5f, true)
	{
	}

	protected override PolygonGameObject IsShouldLooseTheTargetForTheOther ()
	{
		if (!Main.IsNull(curTarget)) {
			if (Vector2.Dot (thisObj.velocity, curTarget.position - thisObj.position) > 0) {
				return curTarget;
			}
		}

		var t = GetTheClosestTarget ();
		if (t != null && t != curTarget) {
			//the bigger dist value is - the less priority is the target
			if(GetPrioritizedDistValue (curTarget) > GetPrioritizedDistValue (t) / 4f){
				return t;
			}
		}
		return curTarget;
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		var angle = Math2d.DeltaAngleDeg( Math2d.GetRotationDg(dir), Math2d.GetRotationDg(thisObj.cacheTransform.right));
		angle = Mathf.Abs(angle);
		float time2rotate = 0.5f + Mathf.Abs(angle) / thisObj.originalTurnSpeed;
		return (dir.magnitude * time2rotate);
	}
}

