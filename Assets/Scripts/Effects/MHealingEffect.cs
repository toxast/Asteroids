using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MHealingEffect: MEffectData, IHasDuration {
	public HealingEffect.Data data;
	public float iduration{get {return data.iduration;} set{data.iduration = value;}}
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
