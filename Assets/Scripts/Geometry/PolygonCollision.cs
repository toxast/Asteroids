using UnityEngine;
using System.Collections;

public static class PolygonCollision 
{
	static public bool IsCollides(PolygonGameObject a, PolygonGameObject b)
	{
		Vector2 distance2d = a.cacheTransform.position - b.cacheTransform.position;
		if (distance2d.magnitude > a.polygon.R + b.polygon.R)
		{
			return false;
		}

		Polygon aGlobal = GetPolygonInGlobalCoordinates(a);
		Polygon bGlobal = GetPolygonInGlobalCoordinates(b);
		
		return IsCollides(aGlobal, bGlobal);
	}

	static private Polygon GetPolygonInGlobalCoordinates(PolygonGameObject a)
	{
		float angle = a.cacheTransform.rotation.eulerAngles.z*Math2d.PIdiv180;
		Vector2[] verticesA = Math2d.RotateVertices(a.polygon.vertices, angle);
		Math2d.ShiftVertices(verticesA, a.cacheTransform.position);
		Polygon aRotated = new Polygon(verticesA);
		return aRotated;
	}
	
	static private bool IsCollides(Polygon polygonA,  Polygon polygonB)
	{ 
		if(!Intersect(polygonA, polygonB) || !Intersect(polygonA, polygonB))
		{
			return false;
		}
		
		return true;
	}
	
	static private bool Intersect(Polygon polygonA,  Polygon polygonB)
	{
		for (int i = 0; i < polygonA.vcount; i++) 
		{
			Edge e = polygonA.edges[i];
			float a = (e.p2.x - e.p1.x) / (e.p1.y  - e.p2.y); //a perpendecular p1-p2
			Vector2 lineNormalized = new Vector2(1f, a).normalized;
			
			//projectA
			float maxA, minA;
			Project(polygonA, lineNormalized, out minA, out maxA);
			
			//projectB
			float maxB, minB;
			Project(polygonB, lineNormalized, out minB, out maxB);
			
			if(minB > maxA || maxB < minA)
			{
				return false;
			}
		}
		
		return true;
	}
	
	static private void Project(Polygon p, Vector2 normalizedLine, out float min, out float max)
	{
		max = float.MinValue;
		min = float.MaxValue;
		for (int n = 0; n < p.vcount; n++) 
		{
			float projection = Math2d.DotProduct(ref normalizedLine, ref p.vertices[n]);
			if(projection > max) max = projection;
			if(projection < min) min = projection;
		}
	}

}
