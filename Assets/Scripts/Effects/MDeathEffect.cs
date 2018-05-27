using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDeathEffect : MEffectData {
	[SerializeField] MEffectData effectToApply;
	[SerializeField] List<PolygonGameObject.KillReason> deathReasons;
	PolygonGameObject picker;

	public override IHasProgress Apply (PolygonGameObject picker)
	{
		this.picker = picker;
		picker.OnDestroying += HandleDestruction;
		return null;
	}

	void HandleDestruction(){
		if (deathReasons.Count == 0 || deathReasons.Contains(picker.GetDeathReason ())) {
			effectToApply.Apply (picker);
		}
	}
}
