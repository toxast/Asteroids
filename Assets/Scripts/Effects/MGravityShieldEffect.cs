using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGravityShieldEffect : MEffectData {
	public GravityShieldEffect.Data data;
	public override void Apply (PolygonGameObject picker) {
		data.Apply (picker);
	}
}
