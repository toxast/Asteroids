using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MHealOnce: MEffectData {
	public HealOnce.Data data;
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
