using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhExplosion
{
	public PhExplosion(Vector2 pos, float power, List<PolygonGameObject> objs)
	{
		float radius = power / 10;
		float rsqr = radius * radius;
		foreach(var obj in objs)
		{
			Vector2 dist = (Vector2)obj.cacheTransform.position - pos;
			float sqrMagnitude = dist.sqrMagnitude;
			if(sqrMagnitude < rsqr) 
			{
//				var polygon = PolygonCollision.GetPolygonInGlobalCoordinates(obj);
//				int closestVertex = -1;
//				float minDist = float.MaxValue;
//				for (int i = 0; i < polygon.vertices.Length; i++) 
//				{
//					var lenSqrt = (polygon.vertices[i] - pos).sqrMagnitude;
//					if(lenSqrt < minDist)
//					{
//						minDist = lenSqrt;
//						closestVertex = i;
//					}
//				}
//				float impulse = power * (1f - Mathf.Sqrt(minDist)/radius);
//				PolygonCollision.ApplyImpulse(obj, pos, impulse * (polygon.vertices[closestVertex] - pos));
//				obj.Hit(impulse / (obj.mass * 15f)); //TODO: unify impule hits or add passive damage?

				float magnitude = Mathf.Sqrt(sqrMagnitude);
				float p = 1 - (magnitude/radius);
				Vector2 normalizedDist = dist/magnitude;
				float hit = p * power / obj.mass;
				obj.velocity += (Vector3)(normalizedDist * hit );
				obj.Hit(hit / 15f);
			}
		}
	}

}
