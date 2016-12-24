using UnityEngine;
using System.Collections;
using System;

//TODO: no targets on asteroid!!!!!!!!!!!!!!!!
//pass enemy layers?

public class TurretTargetSystem : TargetSystemBase<PolygonGameObject>
{
	float gunsRangeSqr;
	float rotationSpeed;
	Func<Vector3> angelsRestriction;

	public TurretTargetSystem(PolygonGameObject thisObj, float rotationSpeed, Func<Vector3> angelsRestriction, float repeatTargetCheck) 
		: base(thisObj, repeatTargetCheck)
	{
		this.rotationSpeed = rotationSpeed;
		this.angelsRestriction = angelsRestriction;

		this.gunsRangeSqr = 1.2f * thisObj.guns [0].Range;
		gunsRangeSqr *= gunsRangeSqr;
	}
		
	protected override PolygonGameObject IsShouldLooseTheTargetForTheOther () {
		Vector3 restrict = angelsRestriction ();
		Vector2 restrictDir = restrict;
		float allowed = restrict.z;
		Vector2 dir = curTarget.position - thisObj.position;
		if (!InHitZone (dir, restrictDir, allowed)) {
			//потеря, если вне зоны 
			var t = GetTheClosestTarget ();
			return t;
		} else {
			var t = GetTheClosestTarget ();
			//переключиться если кто-то рядом а старая цель далеко. 
			if (t != null && t != curTarget) {
				Vector2 dirt = t.position - thisObj.position;
				var curDelta = Vector3.Angle (thisObj.cacheTransform.right, dir);
				if (curDelta > 5 && 2 * Vector3.Angle (thisObj.cacheTransform.right, dirt) < curDelta) {
					return t;
				}
			}
			return curTarget;
		}
	}

	private bool InHitZone(Vector2 dir, Vector2 restrictDir, float allowed)
	{
		return (dir.sqrMagnitude < gunsRangeSqr) && (Vector3.Angle (restrictDir, dir) < allowed);
	}

	Vector3 restrict = Vector3.zero;
	Vector2 restrictDir = Vector2.one;
	float allowed = 0;

	protected override PolygonGameObject GetTheClosestTarget ()
	{
		restrict = angelsRestriction ();
		restrictDir = restrict;
		allowed = restrict.z;
		return base.GetTheClosestTarget ();
	}

	protected override bool ValidTarget (PolygonGameObject obj)
	{
		if (!base.ValidTarget (obj)) {
			return false;
		}

		var dir = obj.position - thisObj.position;
		if (!InHitZone (dir, restrictDir, allowed)) {
			return false;
		}

		return true;
	}

	protected override float GetDistValue (PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		var angle = Math2d.DeltaAngleDeg( Math2d.GetRotationDg(dir), Math2d.GetRotationDg(thisObj.cacheTransform.right));
		float time2rotate = Mathf.Abs(angle) / rotationSpeed;
		return (Mathf.Pow(dir.sqrMagnitude, 0.25f) * time2rotate);
	}
}

