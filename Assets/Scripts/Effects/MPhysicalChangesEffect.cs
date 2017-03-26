using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MPhysicalChangesEffect: MEffectData, IHasDuration {
	public PhysicalChangesEffect.Data data;
	public float iduration{get {return data.iduration;} set{data.iduration = value;}}
	public override IHasProgress Apply (PolygonGameObject picker) {
		return data.Apply (picker);
	}
}
