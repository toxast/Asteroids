using UnityEngine;
using System.Collections;
using System.Diagnostics;

public static class Math2d
{
	public static float PIdiv180 = Mathf.PI/180f;

	static Vector2 right = new Vector2(1,0);

	static public float DotProduct(ref Vector2 v1, ref Vector2 v2)
	{
		return v1.x*v2.x + v1.y*v2.y;
	}
	
	static public float Cos(ref Vector2 v1, ref Vector2 v2)
	{
		return DotProduct(ref v1, ref v2) / (v1.magnitude * v2.magnitude);
	}

	static public float Cos(Vector2 v1, Vector2 v2)
	{
		return DotProduct(ref v1, ref v2) / (v1.magnitude * v2.magnitude);
	}

	/// <summary>
	/// returns angle [0, 2*Pi] to rotate from v1 to v2 counterclockwise;
	/// </summary>
	static public float Angle(ref Vector2 v1, ref Vector2 v2)
	{
		float sign = Mathf.Sign(Cross(ref v1, ref v2));
		float angle = Mathf.Acos(Cos(ref v1, ref v2));
		if(sign > 0)
		{
			return angle;
		}
		else
		{
			return 2*Mathf.PI - angle;
		}
	}

	/// <summary>
	/// returns angle [0, 2*Pi] to rotate from vector (1, 0) to v counterclockwise;
	/// </summary>
	static public float GetRotation(ref Vector2 v)
	{
		return Angle(ref right, ref v);  
	}

	static public float GetRotation(Vector2 v)
	{
		return Angle(ref right, ref v);  
	}

	static public Vector2 GetMassCenter(Vector2[] vertices, out float area)
	{
		Edge[] egdes = GetEdges(vertices);
		return GetMassCenter(egdes, out area);
	}

	static public float Cross(ref Vector2 a, ref Vector2 b)
	{
		return a.x*b.y - a.y*b.x;
	}

 	static public Vector2 GetMassCenter(Edge[] edges, out float area)
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

		area = Mathf.Abs(A)/2f;
		
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

	public static void ScaleVertices(Vector2[] vertices, float scale)
	{
		for (int i = 0; i < vertices.Length; i++) 
		{
			vertices[i] *= scale;
		}
	}
	
	static public Vector2[] RotateVertices(Vector2[] vertices, float angle)
	{
		float cosA = Mathf.Cos(angle);
		float sinA = Mathf.Sin(angle);
		Vector2[] verticesRotated = new Vector2[vertices.Length];

		for (int i = 0; i < verticesRotated.Length; i++) 
		{
			verticesRotated[i] = RotateVertex(vertices[i], cosA, sinA); 
		}

		return verticesRotated;
	}

	static public Vector2 RotateVertex(Vector2 v, float cosA, float sinA)
	{
		return new Vector2(v.x*cosA - v.y*sinA, v.x*sinA + v.y*cosA);
	}

	static public Edge RotateEdge(Edge e, float cosA, float sinA)
	{
		return new Edge (RotateVertex (e.p1, cosA, sinA), RotateVertex (e.p2, cosA, sinA));
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

	public static Vector3 SetZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
}

