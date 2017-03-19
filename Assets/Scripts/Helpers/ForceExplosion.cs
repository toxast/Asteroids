using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceExplosion {
	public ForceExplosion(List<ExplosionData> objectsAroundData, Vector2 pos, float force, bool distanceMatters = true, bool areaMatters = true) 
	{
		foreach(var data in objectsAroundData) {
			var obj = data.obj;
			var closesVertex = data.closesVertex;
			if (distanceMatters) {
				force *= data.distance01;
			}
			if (areaMatters) {
				force *= obj.massSqrt85;
			}
			var dir = ExplosionData.HackTransformForceDirection (pos, closesVertex, data.obj);
			PolygonCollision.ApplyForce(obj, closesVertex, force * dir);
		}
	}
}
