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
	private bool mixPiority;
	protected PolygonGameObject curTarget{get{return thisObj.target;}}

	public TargetSystemBase(T thisObj, float repeatTargetCheck, bool mixPiority = false) {
		this.thisObj = thisObj;
		this.repeatTargetCheck = repeatTargetCheck;
		this.mixPiority = mixPiority;
		leftUntilTargetCheck = 0;
	}

	protected bool SholuldDropTargetInstantly() {
		return Main.IsNull(curTarget) || !ValidTarget(curTarget);
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
		return curTarget;
	}

	protected virtual PolygonGameObject GetTheClosestTarget() {
		if (mixPiority) {
			var enemyLayers = CollisionLayers.GetEnemyLayers (thisObj.layerLogic);
			var t = GetClosestObjectFromLayers (enemyLayers, mixed);
			if (t != null) {
				return t;
			}
		} else {
			var enemyLayers = CollisionLayers.GetEnemyLayers (thisObj.layerLogic);
			var t = GetClosestObjectFromLayers (enemyLayers, normList);
			if (t != null) {
				return t;
			}
			t = GetClosestObjectFromLayers (enemyLayers, lowList);
			if (t != null) {
				return t;
			}
		}
		return null;
	}

	public class PriorityMultiplier {
		public PolygonGameObject.ePriorityLevel plevel;
		public float mul = 1;
		public PriorityMultiplier(PolygonGameObject.ePriorityLevel lvl, float mul = 1){
			plevel = lvl;
			this.mul = mul;
		}
	}

	static List<PriorityMultiplier> lowList = new List<PriorityMultiplier>{new PriorityMultiplier(PolygonGameObject.ePriorityLevel.LOW)} ;
	static List<PriorityMultiplier> normList = new List<PriorityMultiplier>{new PriorityMultiplier(PolygonGameObject.ePriorityLevel.NORMAL)}; 
	static List<PriorityMultiplier> mixed = new List<PriorityMultiplier>{
		new PriorityMultiplier(PolygonGameObject.ePriorityLevel.NORMAL, 1f),
		new PriorityMultiplier(PolygonGameObject.ePriorityLevel.LOW, 1.7f),
	} ;


	private PolygonGameObject GetClosestObjectFromLayers(int enemylayer, List<PriorityMultiplier> plevels) {
		float distValue = float.MaxValue;
		int indx = -1;
		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++) {
			var obj = gobjects [i];
			if ((enemylayer & obj.layerLogic) != 0 && ValidTarget(obj)) {
				for (int k= 0; k < plevels.Count; k++) {
					if (obj.priority == plevels[k].plevel) {
						float objDistValue = GetPrioritizedDistValue (obj) * plevels[k].mul;
						if (objDistValue < distValue) {
							indx = i;
							distValue = objDistValue;
						}
						break;
					}
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

	protected virtual float GetPrioritizedDistValue(PolygonGameObject obj)
	{
		return GetDistValue(obj) / obj.priorityMultiplier;
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
