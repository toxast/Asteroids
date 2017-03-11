using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MExtraGunsEffect : MEffectData, IHasDuration {
	public ExtraGunsEffect.Data data;
	public float iduration{get {return data.iduration;} set{data.iduration = value;}}
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
