using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserComboPowerup : IApplyable{
	public List<MEffectData> effects;
	public void Apply (PolygonGameObject picker){
		foreach (var item in effects) {
			item.Apply (picker);
		}
	}
}