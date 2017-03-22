using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ForceExplosion {

	public static float maxForce = 0;

	public ForceExplosion(List<ExplosionData> objectsAroundData, Vector2 pos, float force, bool distanceMatters = true, bool areaMatters = true) 
	{
		foreach(var data in objectsAroundData) {
			float pforce = force;
			var obj = data.obj;
			var closesVertex = data.closesVertex;
			if (distanceMatters) {
				pforce *= data.distance01;
			}
			if (areaMatters) {
				pforce *= obj.massSqrt;
			}
			var dir = ExplosionData.HackTransformForceDirection (pos, closesVertex, data.obj);
//			if (dir.magnitude > 2) {
//				Debug.LogError ("WTF");
//			}
//			if (Mathf.Abs(force) > maxForce) {
//				maxForce = Mathf.Abs(force);
//				Debug.LogWarning(force + " " + obj.mass + " " + data.distance01 + "" );
//			}
			PolygonCollision.ApplyForce(obj, closesVertex, pforce * dir);
		}
	}
}
