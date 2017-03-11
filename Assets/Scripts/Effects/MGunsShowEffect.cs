using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MGunsShowEffect: MEffectData, IHasDuration {
	public MGunsShow data;
	public float iduration{get {return data.duration;} set{data.duration = value;}}
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}