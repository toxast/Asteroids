using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSystemBase<T> : ITickable
	where T: PolygonGameObject
{
	protected T thisObj;
	protected float repeatTargetCheck = 2f;

	protected float leftUntilTargetCheck;

	protected bool hasTarget = false;

	protected PolygonGameObject curTarget{get{return thisObj.target;}}

	public TargetSystemBase(T thisObj, float repeatTargetCheck) {
		this.thisObj = thisObj;
		this.repeatTargetCheck = repeatTargetCheck;
		leftUntilTargetCheck = 0;
	}

	protected bool SholuldDropTargetInstantly() {
		return Main.IsNull(curTarget) || curTarget.IsInvisible();
	}

	public void Tick(float delta) {
		if (hasTarget && SholuldDropTargetInstantly()) {
			thisObj.SetTarget (null);
			leftUntilTargetCheck = 0;
		}
		hasTarget = curTarget != null;

		if (leftUntilTargetCheck >= 0) {
			leftUntilTargetCheck -= delta;
		}

		if (leftUntilTargetCheck < 0) {
			leftUntilTargetCheck = repeatTargetCheck;
			PolygonGameObject newTarget = null;
			if (curTarget == null) {
				newTarget = GetTheClosestTarget ();
				if (newTarget != null) {
                    //string newstr = newTarget != null ? newTarget.name : "null";
                    //Debug.LogError("target gained: " + newstr);
                    thisObj.SetTarget (newTarget);
				}
			} else {
				newTarget = IsShouldLooseTheTargetForTheOther ();
				if (newTarget != curTarget) {

                    //string curstr = curTarget != null ? curTarget.name : "null";
                    //string newstr = newTarget != null ? newTarget.name : "null";
                    //Debug.LogError("target change: " + curstr + " " + newstr);

                    thisObj.SetTarget (newTarget);
				}
			}
		}
	}

	//can return null if had to loose the target
	//can assume that current targed passed SholuldDropTargetInstantly check
	protected virtual PolygonGameObject IsShouldLooseTheTargetForTheOther() {
		return thisObj.target;
	}

	protected virtual PolygonGameObject GetTheClosestTarget()
	{
		var enemyLayers = CollisionLayers.GetEnemyLayersInPriority (thisObj.layer);
		foreach (var enemylayer in enemyLayers) {
			var t = GetClosestObjectFromLayer (enemylayer);
			if (t != null) {
				return t;
			}
		}

		return null;
	}

	private PolygonGameObject GetClosestObjectFromLayer(int enemylayer) {
		float distValue = float.MaxValue;
		int indx = -1;
		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++) {
			var obj = gobjects [i];
			if ((enemylayer & obj.layer) != 0 && ValidTarget(obj)) {
				float objDistValue = GetDistValue (obj);
				if (objDistValue < distValue) {
					indx = i;
					distValue = objDistValue;
				}
			}
		}

		if (indx >= 0) {
			return gobjects [indx];
		} else {
			return null;
		}
	}

	protected virtual bool ValidTarget(PolygonGameObject obj) {
		return !obj.IsInvisible ();
	}

	protected virtual float GetDistValue(PolygonGameObject obj)
	{
		var dir = obj.position - thisObj.position;
		return dir.sqrMagnitude;
	}

	protected bool IsSqrDistLess(PolygonGameObject t, float Rsqr) {
		return SqrDist(t) < Rsqr;
	}

	protected bool IsSqrDistMore(PolygonGameObject t, float Rsqr) {
		return SqrDist(t) >= Rsqr;
	}

	protected float SqrDist(PolygonGameObject t) {
		return (thisObj.position - t.position).sqrMagnitude;
	}
}
