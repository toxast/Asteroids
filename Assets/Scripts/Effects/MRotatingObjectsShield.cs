using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MRotatingObjectsShield: MEffectData, IHasDuration {
	public RotatingObjectsShield.Data data;
	public float iduration{get {return data.iduration;} set{data.iduration = value;}}
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
