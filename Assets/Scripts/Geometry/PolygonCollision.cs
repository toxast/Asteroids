using UnityEngine;
using System.Collections;

public static class PolygonCollision 
{

	static public bool IsCollides(PolygonGameObject a, PolygonGameObject b)
	{
		int indxa, indxb;
		return IsCollides(a, b, out indxa, out indxb);
	}

	static public bool IsCollides(PolygonGameObject a, PolygonGameObject b, out int indxa, out int indxb)
	{
		indxa = -1;
		indxb = -1;

		Vector2 distance2d = a.cacheTransform.position - b.cacheTransform.position;
		if (distance2d.magnitude > a.polygon.R + b.polygon.R)
		{
			return false;
		}

		Polygon aGlobal = GetPolygonInGlobalCoordinates(a);
		Polygon bGlobal = GetPolygonInGlobalCoordinates(b);

		return IsCollides(aGlobal, bGlobal, out indxa, out indxb);
	}

	static private Polygon GetPolygonInGlobalCoordinates(PolygonGameObject a)
	{
		float angle = a.cacheTransform.rotation.eulerAngles.z*Math2d.PIdiv180;
		Vector2[] verticesA = Math2d.RotateVerticesRAD(a.polygon.vertices, angle);
		Math2d.ShiftVertices(verticesA, a.cacheTransform.position);
		Polygon aRotated = new Polygon(verticesA);
		aRotated.SetMassCenter (a.cacheTransform.position);
		return aRotated;
	}

	/*todo* boundbox*/
	static private bool IsCollides(Polygon polygonA,  Polygon polygonB, out int vertexA, out int vertexB )
	{ 
		vertexA = -1;
		vertexB = -1;
		for (int i = 0; i < polygonA.vcount; i++) 
		{
			var v = polygonA.vertices[i];
			if(polygonB.IsPointInside(v))
			{
				vertexA = i;
				return true;
			}
		}

		for (int i = 0; i < polygonB.vcount; i++) 
		{
			var v = polygonB.vertices[i];
			if(polygonA.IsPointInside(v))
			{
				vertexB = i;
				return true;
			}
		}

		return false;
	}


	//returns impulse of collision
	static public float ApplyCollision(PolygonGameObject aobj, PolygonGameObject bobj, int aVertex, int bVertex)
	{
		Polygon a = GetPolygonInGlobalCoordinates(aobj);
		Polygon b = GetPolygonInGlobalCoordinates(bobj);
		//find edge
		if(aVertex >= 0)
		{ 
			return ApplyCollision(aobj, a, aVertex, bobj, b);
		}
		else if(bVertex >= 0)
		{
			return ApplyCollision(bobj, b, bVertex, aobj, a);
		}

		return 0;
	}

	static private float ApplyCollision(PolygonGameObject aobj, Polygon a, int aVertex,
	                                   PolygonGameObject bobj, Polygon b)
	{
		var v0 = a.vertices[a.Previous(aVertex)];
		var v1 = a.vertices[aVertex];
		var v2 = a.vertices[a.Next(aVertex)];
		var vInside = (v0 - v1).normalized + (v2 - v1).normalized;
		Edge e = new Edge(v1, v1 + vInside.normalized*200); //TODO ray
		var intersections = Intersection.GetIntersections(e, b.edges);
		if(intersections.Count == 0)
		{	
			Debug.LogError("intersections.Count == 0");
			return 0f;
		}
		
		int closest = -1;
		float sqrdisr = float.MaxValue;
		for (int i = 0; i < intersections.Count; i++)
		{
			if(!intersections[i].haveIntersection)
				continue;
			
			var sqrdisti = (v1 - intersections[i].intersection).sqrMagnitude;
			if(sqrdisti < sqrdisr)
			{
				sqrdisr = sqrdisti;
				closest = i;
			}
		}

		if(closest < 0)
		{
			//TODO: its happening, fix!
			Debug.LogError("closest < 0");
			return 0f;
		}
		
		//got closest edge
		var collisionEdge = b.edges[closest];
		var intersectionPointOnEdge = intersections[closest].intersection;
		
		var Ra = intersectionPointOnEdge - a.massCenter;
		var Rb = intersectionPointOnEdge - b.massCenter;
		Vector2 Va = (Vector2)aobj.velocity + cross(Ra,  aobj.rotation * Math2d.PIdiv180);
		Vector2 Vb = (Vector2)bobj.velocity + cross(Rb,  bobj.rotation * Math2d.PIdiv180);
		var Vab =  Va - Vb;
		var Nb = -makeRight(collisionEdge.p2-collisionEdge.p1).normalized;
		if(Math2d.DotProduct(ref Nb, ref Vab) >= 0)
		{
			Debug.LogWarning("not approaching");
			return 0f;
		}
		float ekff = 0.4f; //0 < k < 1
		float wa = Vector3.Cross(Ra, Nb).sqrMagnitude / aobj.inertiaMoment;
		float wb = Vector3.Cross(Rb, Nb).sqrMagnitude / bobj.inertiaMoment;
		float j =  -(1 + ekff) * Math2d.DotProduct(ref Vab, ref Nb) / (1f/aobj.mass + 1f/bobj.mass + wa + wb);
		
		aobj.velocity = (Vector2)aobj.velocity + (j * Nb / aobj.mass);
		bobj.velocity = (Vector2)bobj.velocity - (j * Nb / bobj.mass);

		aobj.rotation -= Vector3.Cross (Ra, j * Nb).z / (aobj.inertiaMoment * Math2d.PIdiv180);
		bobj.rotation += Vector3.Cross (Rb, j * Nb).z / (bobj.inertiaMoment * Math2d.PIdiv180);

		return j;
	}

	static private Vector2 makeRight(Vector2 v)
	{
		return new Vector2 (v.y, -v.x);
	}

	static private Vector2 cross(Vector2 v, float w)
	{
		return w * new Vector2 (v.y, -v.x);
	}

	/*By SAT theorem, so it's for convex polygons only 
	static private bool SAT_IsCollides(Polygon polygonA,  Polygon polygonB)
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
	*/

}
