using UnityEngine;
using System.Collections;
using System.Diagnostics;

public static class Math2d
{

	static public float DotProduct(ref Vector2 v1, ref Vector2 v2)
	{
		return v1.x*v2.x + v1.y*v2.y;
	}
	
	static public float Cos(ref Vector2 v1, ref Vector2 v2)
	{
		return DotProduct(ref v1, ref v2) / (v1.magnitude * v2.magnitude);
	}

	static public Vector2 GetMassCenter(Vector2[] vertices)
	{
		Edge[] egdes = GetEdges(vertices);
		return GetMassCenter(egdes);
	}

	static public float Rotate(ref Vector2 a, ref Vector2 b)
	{
		return a.x*b.y - a.y*b.x;
	}

 	static public Vector2 GetMassCenter(Edge[] edges)
	{
		float Cx = 0f;
		float Cy = 0f;
		float A  = 0f;
		float xy = 0;
		Vector2 vi;
		Vector2 vi_1;
		
		for(int i=0; i < edges.Length; i++)
		{
			vi = edges[i].p1; 
			vi_1 = edges[i].p2;
			xy = vi.x * vi_1.y - vi_1.x * vi.y;
			A += xy;
			Cx += (vi.x + vi_1.x) * xy;
			Cy += (vi.y + vi_1.y) * xy;
		}
		
		A *= 3f;
		Cx /= A;
		Cy /= A;
		
		Vector2 center = new Vector2(Cx, Cy);
		return center;
	}

	/// <summary>
	/// Returns edges formed by given vertices, closed. 
	/// Order: 0-1, 1-2, ..., last-0
	/// </summary>
	static public Edge[] GetEdges(Vector2[] vertices)
	{
		Edge[] edges = new Edge[vertices.Length];
		for(int i=0; i < vertices.Length - 1; i++)
		{
			edges[i] = new Edge(vertices[i], vertices[i+1]);
		}
		edges[vertices.Length - 1] = new Edge(vertices[vertices.Length - 1], vertices[0]);
		return edges;
	}

	public static void ShiftVertices(Vector2[] vertices, Vector2 offset)
	{
		for (int i = 0; i < vertices.Length; i++) 
		{
			vertices[i] += offset;
		}
	}

	static public bool IsCollides(Transform transformA, Polygon polygonA, Transform transformB, Polygon polygonB)
	{
		float angle = transformA.rotation.eulerAngles.z*Mathf.PI/180f;
		Vector2[] verticesA = RotateVertices(polygonA.vertices, angle);
		ShiftVertices(verticesA, transformA.position);
		Polygon a = new Polygon(verticesA);

		angle = transformB.rotation.eulerAngles.z*Mathf.PI/180f;
		Vector2[] verticesB = RotateVertices(polygonB.vertices, angle);
		ShiftVertices(verticesB, transformB.position);
		Polygon b = new Polygon(verticesB);

		return IsCollides(a, b);
	}

	static public bool IsCollides(Polygon polygonA,  Polygon polygonB)
	{ 
		if(!Intersect(polygonA, polygonB) || !Intersect(polygonA, polygonB))
		{
			return false;
		}

		return true;
	}

	private static bool Intersect(Polygon polygonA,  Polygon polygonB)
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

	private static void Project(Polygon p, Vector2 normalizedLine, out float min, out float max)
	{
		max = float.MinValue;
		min = float.MaxValue;
		for (int n = 0; n < p.vcount; n++) 
		{
			float projection = DotProduct(ref normalizedLine, ref p.vertices[n]);
			if(projection > max) max = projection;
			if(projection < min) min = projection;
		}
	}
	
	static private Vector2[] RotateVertices(Vector2[] vertices, float angle)
	{
		float cosA = Mathf.Cos(angle);
		float sinA = Mathf.Sin(angle);
		Vector2[] verticesRotated = new Vector2[vertices.Length];

		Vector2 vi;
		for (int i = 0; i < verticesRotated.Length; i++) 
		{
			vi = vertices[i];
			verticesRotated[i] = new Vector2(vi.x*cosA - vi.y*sinA, vi.x*sinA + vi.y*cosA);
		}

		return verticesRotated;
	}

	/*
	static public void TestRefDotProduct()
	{
		Vector2 a = new Vector2(2f,30f);
		Vector2 b = new Vector2(7f,12f);

		Stopwatch sw = Stopwatch.StartNew();

		for(int i = 0; i < 10000000; i++)
		{
			DotProduct(ref a, ref b);
		}

		sw.Stop();
		UnityEngine.Debug.LogError("TestRefDotProduct Time taken: " + sw.Elapsed.TotalMilliseconds);
	}

	static public void TestDotProduct()
	{
		Vector2 a = new Vector2(2f,30f);
		Vector2 b = new Vector2(7f,12f);

		Stopwatch sw = Stopwatch.StartNew();

		for(int i = 0; i < 10000000; i++)
		{
			DotProduct(a, b);
		}

		sw.Stop();
		UnityEngine.Debug.LogError("TestDotProduct Time taken: " + sw.Elapsed.TotalMilliseconds);
	}
	*/

	public static Vector3 SetX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 SetY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}
}


