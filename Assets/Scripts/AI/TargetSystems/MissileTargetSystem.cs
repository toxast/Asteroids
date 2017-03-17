using UnityEngine;
using System.Collections;

public class MissileTargetSystem : TargetSystemBase<SpaceShip>
{
	public MissileTargetSystem (SpaceShip thisObj) : base (thisObj, 0.5f)
	{
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		var angle = Math2d.DeltaAngleDeg( Math2d.GetRotationDg(dir), Math2d.GetRotationDg(thisObj.cacheTransform.right));
		float time2rotate = 0.2f + Mathf.Abs(angle) / thisObj.originalTurnSpeed;
		return (dir.magnitude * time2rotate * time2rotate);
	}
}

