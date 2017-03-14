using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionData{
	public PolygonGameObject obj;
	public Vector2 distCenters;
	public int closestVertexIndx;
	public Vector2 closesVertex;
	public float distance01;

	//change the force direction so it wouldn't be all into rotation 
	//force from origin to applyPos of the obj
	public static Vector2 HackTransformForceDirection(Vector2 origin, Vector2 applyPos, PolygonGameObject obj) {
		var originalForceDir = (applyPos - origin).normalized; //todo: pass dist
		var toCenter = (obj.position - applyPos).normalized;
		var dot = Vector2.Dot (toCenter, originalForceDir);
		if (Mathf.Abs (dot) < 0.4f) {
			float signRight = Mathf.Sign (Vector2.Dot (Math2d.MakeRight (toCenter), originalForceDir));
			float signDot = Mathf.Sign (dot);
			if (signRight == 0) { signRight = 1f; }
			if (signDot == 0) { signDot = 1f; }
			var newForceDir = Math2d.RotateVertexDeg (signDot * toCenter, -45f * signRight);
			return newForceDir;
		} else {
			return originalForceDir;
		}
	}

	public static List<ExplosionData> CollectData(Vector2 pos, float radius, List<PolygonGameObject> objs, int collision = -1){
		List<ExplosionData> result = new List<ExplosionData> ();
		float rsqr = radius * radius;
		foreach(var obj in objs) {
			if(obj == null) {
				continue;
			}
			if((obj.layerCollision & collision) == 0) {
				continue;
			}
			Vector2 distCenters = obj.position - pos;
			if(Math2d.ApproximatelySame(distCenters, Vector2.zero)) {
				continue;
			}
			float distCentersSqr = distCenters.sqrMagnitude;
			if(distCentersSqr < rsqr + obj.polygon.Rsqr + 2 * radius * obj.polygon.R) {
				ExplosionData exp = new ExplosionData ();
				exp.obj = obj;
				exp.distCenters = distCenters;
				var gpolygon = obj.globalPolygon;
				int closestVertexIndx = -1;
				float minDistSqr = float.MaxValue;
				for (int i = 0; i < gpolygon.vertices.Length; i++) {
					var lenSqrt = (gpolygon.vertices [i] - pos).sqrMagnitude;
					if (lenSqrt < minDistSqr) {
						minDistSqr = lenSqrt;
						closestVertexIndx = i;
					}
				}
				if (minDistSqr < rsqr) {
					var closesVertex = gpolygon.vertices [closestVertexIndx];
					exp.closesVertex = closesVertex;
					exp.distance01 = (1f - Mathf.Sqrt(minDistSqr) / radius);
					exp.closestVertexIndx = closestVertexIndx;
					result.Add (exp);
				}
			}
		}
		return result;
	}
}
