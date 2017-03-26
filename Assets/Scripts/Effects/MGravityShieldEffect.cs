using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGravityShieldEffect : MEffectData {
	public GravityShieldEffect.Data data;
	public override IHasProgress Apply (PolygonGameObject picker) {
		return data.Apply (picker);
	}
}
