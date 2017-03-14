using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityForceExplosion {
    public GravityForceExplosion(Vector2 pos, float radius, float maxDamage, float maxForce, List<PolygonGameObject> objs, int collision = -1) {
		var objectsAroundData = ExplosionData.CollectData (pos, radius, objs, collision);
		foreach(var data in objectsAroundData) {
			var obj = data.obj;
			var closesVertex = data.closesVertex;
			float force = maxForce * data.distance01 * obj.massSqrt;
			float dmg = maxDamage * data.distance01;
			var dir = ExplosionData.HackTransformForceDirection (pos, closesVertex, data.obj);
			PolygonCollision.ApplyForce(obj, closesVertex, force * dir);
			obj.Hit(dmg);
		}
    }
}
