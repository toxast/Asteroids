using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhExplosion
{
    public PhExplosion(Vector2 pos, float radius, float maxDamage, float maxForce, List<PolygonGameObject> objs, int collision = -1)
	{
		//Debug.LogWarning (pos + " " + radius + " " + power);
		float rsqr = radius * radius;
		foreach(var obj in objs)
		{
			if(obj == null)
			{
				continue;
			}

			if((obj.layer & collision) == 0)
			{
				continue;
			}

			Vector2 distCenters = obj.position - pos;
			if(Math2d.ApproximatelySame(distCenters, Vector2.zero))
			{
				//TODO: pass self obj
				continue;
			}

			float distCentersSqr = distCenters.sqrMagnitude;
			if(distCentersSqr < rsqr + obj.polygon.Rsqr + 2 * radius * obj.polygon.R) 
			{
				var polygon = PolygonCollision.GetPolygonInGlobalCoordinates(obj);
				int closestVertexIndx = -1;
				float minDist = float.MaxValue;
				for (int i = 0; i < polygon.vertices.Length; i++) 
				{
					var lenSqrt = (polygon.vertices[i] - pos).sqrMagnitude;
					if(lenSqrt < minDist)
					{
						minDist = lenSqrt;
                        closestVertexIndx = i;
					}
				}
                var closesVertex = polygon.vertices [closestVertexIndx];
                float force = maxForce * (1f - Mathf.Sqrt(minDist)/radius);
                float dmg = maxDamage* (1f - Mathf.Sqrt(minDist)/radius);
                PolygonCollision.ApplyForce(obj, closesVertex, force * (closesVertex - pos).normalized);
                //Debug.LogWarning("explosion hit: " + dmg);
                obj.Hit(dmg); //TODO: unify impule hits or add passive damage?

//				float distCentersMagnitude = distCenters.magnitude;
//				float dist2edgeOfObject = Mathf.Max(0, distCentersMagnitude - obj.polygon.R);
//				float p = Mathf.Sqrt(Mathf.Max(0, 1f - (dist2edgeOfObject/radius)));
//				Vector2 normalizedDist = distCenters / distCentersMagnitude;
//
//				float impulse = p * power;
//				var dVelocity = normalizedDist * impulse / obj.mass;
//				obj.velocity += dVelocity;
//				Debug.LogWarning(impulse);
//				obj.Hit(impulse);
			}
		}
	}

}
