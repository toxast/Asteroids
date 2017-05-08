using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MultiEffects : MEffectData {
	public List<MEffectData> effects;
	public float iduration {
		get {
			var ed = effects.Find( e => (e is IHasDuration));
			if (ed != null) {
				return (ed as IHasDuration).iduration;
			} else {
				return 0;
			}
		} 
		set {
			foreach (var item in effects) {
				if (item is IHasDuration) {
					(item as IHasDuration).iduration = value;
				}
			}
		}
	}

	public override IHasProgress Apply (PolygonGameObject picker) {
		IHasProgress progressEffect = null;
		foreach (var item in effects) {
			var effect = item.Apply (picker);
			if (effect != null && progressEffect == null) {
				progressEffect = effect;
			}
		}

		return progressEffect;
	}
}
