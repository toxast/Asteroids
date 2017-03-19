using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PhExplosion
{
    public PhExplosion(Vector2 pos, float radius, float maxDamage, float maxForce, List<PolygonGameObject> objs, int collision = -1) {
		var objectsAroundData = ExplosionData.CollectData (pos, radius, objs, collision);
		new ForceExplosion (objectsAroundData, pos, maxForce);
		new DamageExplosion(objectsAroundData, pos, maxDamage);
	}
}


