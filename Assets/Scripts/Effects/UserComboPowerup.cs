using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserComboPowerup : IApplyable, IHasDuration
{
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

	public void Apply (PolygonGameObject picker) {
		foreach (var item in effects) {
			item.Apply (picker);
		}
	}
}