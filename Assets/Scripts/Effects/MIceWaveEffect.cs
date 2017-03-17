using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MIceWaveEffect: MEffectData {
	public IceWaveEffect.Data data;
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
