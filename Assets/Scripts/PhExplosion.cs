using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhExplosion
{
	public PhExplosion(Vector2 pos, float radius, float power, List<IPolygonGameObject> objs)
	{
		//Debug.LogWarning (pos + " " + radius + " " + power);
		float rsqr = radius * radius;
		foreach(var obj in objs)
		{
			if(obj == null)
			{
				Debug.LogError("null");
				continue;
			}

			Vector2 dist = obj.position - pos;
			if(Math2d.ApproximatelySame(dist, Vector2.zero))
			{
				//TODO: self obj
				continue;
			}

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
				float p = Mathf.Pow(Mathf.Abs(1f - (magnitude/radius)), 1.5f);
				Vector2 normalizedDist = dist/magnitude;

				float impulse = p * power;
				var dVelocity = normalizedDist * impulse / obj.mass;
				obj.velocity += dVelocity;
				obj.Hit(Singleton<GlobalConfig>.inst.ExplosionDamageKff * impulse * Singleton<GlobalConfig>.inst.DamageFromCollisionsModifier);
			}
		}
	}

}
