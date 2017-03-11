using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MHeavyBulletEffect : MEffectData, IHasDuration {
	public HeavyBulletEffect.Data data;
	public float iduration{get {return data.iduration;} set{data.iduration = value;}}
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}



