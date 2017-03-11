using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsShowEffect : TickableEffect, IHasDuration {
	MGunsShow data;
	PolygonGameObject gunsShowObj;

	protected override eType etype { get { return eType.GunsShow; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	public float iduration{get { return data.duration;} set { data.duration = value;}}

	public GunsShowEffect(MGunsShow data) {
		this.data = data;
	}

	public override void SetHolder(PolygonGameObject holder) {
		base.SetHolder(holder);
		gunsShowObj = data.CreateObj(holder.layerNum);
		SetPosition ();
		if (data.makeChild) {
			gunsShowObj.cacheTransform.parent = holder.cacheTransform;
			gunsShowObj.cacheTransform.localRotation = Quaternion.identity; 
		}
	}

	void SetPosition(){
		gunsShowObj.position = holder.position;
	}

	public override void HandleHolderDestroying ()
	{
		base.HandleHolderDestroying ();
		DestroyGunsShowObject ();
	}

	public override void Tick(float delta) {
		base.Tick(delta);
		if (!IsFinished()) {
			gunsShowObj.Tick(delta);
			if (!data.makeChild) {
				SetPosition ();
			}
			if (IsFinished ()) {
				DestroyGunsShowObject ();
			}
		} 
	}

	private void DestroyGunsShowObject(){
		if (!Main.IsNull(gunsShowObj)) {
			GameObject.Destroy(gunsShowObj.gameObj);
			gunsShowObj = null;
		}
	}

	public override bool IsFinished() {
		return gunsShowObj == null || gunsShowObj.Expired();
	}

	public void ForceFinish() {
		DestroyGunsShowObject ();
	}
}

