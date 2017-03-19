using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageExplosion {
	public DamageExplosion(List<ExplosionData> objectsAroundData, Vector2 pos, float maxDamage, bool distanceMatters = true) 
	{
		foreach(var data in objectsAroundData) {
			var obj = data.obj;
			float dmg; 
			dmg = maxDamage;
			if (distanceMatters) {
				dmg *= data.distance01;
			}
			if (dmg > 0) {
				obj.Hit (dmg);
			}
		}
	}
}
