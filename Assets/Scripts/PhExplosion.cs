using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhExplosion
{
	public PhExplosion(Vector2 pos, float power, List<PolygonGameObject> objs)
	{
		float radius = power / 4;
		float rsqr = radius * radius;
		foreach(var obj in objs)
		{
			Vector2 dist = (Vector2)obj.cacheTransform.position - pos;
			float sqrMagnitude = dist.sqrMagnitude;
			if(sqrMagnitude < rsqr) 
			{
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
