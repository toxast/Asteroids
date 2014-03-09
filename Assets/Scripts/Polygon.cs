using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Polygon
{
	public Vector2[] vertices;
	public Vector2[] circulatedVertices; //vertices.Length + 2 (last, 0, 1,...., last, 0) 
	public Edge[] edges; 
	private float xMin;
	public int vcount{private set; get;}

	public float R; //radius of outer sphere 
	public float Rsqr;
	Vector2 massCenter = new Vector2(0,0);

	//private static double CosCutTreshold = Math.Cos((130f * Math.PI) / 180f);

	public Polygon(Vector2[] vertices)
	{
		this.vertices = vertices;
		this.vcount = vertices.Length;

		circulatedVertices = new Vector2[vcount + 2];
		circulatedVertices[0] = vertices[vcount-1]; //last
		for (int i = 0; i < vcount; i++) 
		{
			circulatedVertices[i+1] = vertices[i];
		}
		circulatedVertices[vcount+1] = vertices[0];

		edges = new Edge[vcount];
		for(int i=1; i < circulatedVertices.Length - 1; i++)
		{
			edges[i-1] = new Edge(circulatedVertices[i], circulatedVertices[i+1]);
		}

		this.xMin = GetMinX ();

		this.R = GetRadiusOfOuterSphere();
		this.Rsqr = R*R;
		//massCenter = GetMassCenter();

		/*GameObject m = new GameObject();
		m.name = "m";
		m.transform.position = new Vector3(massCenter.x, massCenter.y, 0);
		for (int i = 0; i < vcount; i++) 
		{
			//Debug.Log(IsEdgeLiesInsideOfPolygon(masscenter, i));
		}*/
	}

	public void SetMassCenter(Vector2 center)
	{
		massCenter = center;
	}

	private float GetRadiusOfOuterSphere()
	{
		float maxRsqr = -1;
		float r = -1;
		for (int i = 0; i < vcount; i++) 
		{
			r = (vertices[i]-massCenter).sqrMagnitude;
			if(r > maxRsqr)
			{
				maxRsqr = r;
			}
		}
		return (float)Math.Sqrt(maxRsqr);
	}

	private float GetMinX()
	{
		float min = float.MaxValue;
		for (int i = 0; i < vcount; i++) 
		{
			if(min > vertices[i].x)
			{
				min = vertices[i].x;
			}
		}
		return min;
	}

	public int Distance(int i1, int i2)
	{
		int abs = Math.Abs(i1-i2);
		return Math.Min(abs, vcount - abs);
	}

	public bool IsVerticesAdjacent(int i1, int i2)
	{
		return Distance(i1, i2) <= 1;
	}

	public int Next(int i)
	{
		if(i == vcount - 1)
		{
			return 0;
		}
		else
		{
			return i+1;
		}
	}

	public int Previous(int i)
	{
		if(i == 0)
		{
			return vcount - 1;
		}
		else
		{
			return i-1;
		}
	}



	public bool IsCanCutByVertices(int i1, int i2)
	{
		if(Distance(i1, i2) == 2)
		{
			return false;
		}
		/*//Debug.LogWarning("IsCanCutByVertices " + i1 + "-" + i2);
		if(Distance(i1, i2) == 2)
		{
			//Debug.LogWarning("distance 2");
			Vector2 center;
			if(i1 < i2)
			{
				center = vertices[Next(i1)];
			}
			else
			{
				center = vertices[Next(i2)];
			}

			Vector2 v1 = vertices[i1] - center;
			Vector2 v2 = vertices[i2] - center;

			float cos = Math2d.Cos(ref v1, ref v2);

			//Debug.LogWarning(v1 + "-" + v2 + " cos: " + cos + "  th: " +  CosCutTreshold);

			if(cos < CosCutTreshold)
			{
				return false;
			}
		}*/

		return !IsVerticesAdjacent(i1, i2) && IsEdgeLiesInsideOfPolygon(i1, i2);
	}

	/// <summary>
	/// Determines whether edge (vertices[a] -> vertices[b]) lies inside of polygon specified by vertices2D.
	/// </summary>
	public bool IsEdgeLiesInsideOfPolygon(int a, int b)
	{
		if (vcount <= 3)
		{
			return true;
		}
		
		//Debug.LogWarning ("IsEdgeLiesInsideOfPolygon: " + a + "->" + b);
		if (IsVerticesAdjacent(a, b))
		{
			//Debug.Log("adjacent edges " + a + " " + b);
			return true;
		}
		
		bool inside = false;
		
		//TODO: refactor edges without a and b vertexes
		List<Edge> edgesWithoutAB = new List<Edge>(vcount);
		for(int i=0; i < vcount - 1; i++)
		{
			if(i != a && i != b && i+1 != a && i+1 != b)
				edgesWithoutAB.Add(new Edge(vertices[i], vertices[i+1]));
		}
		if((vcount - 1 != a) && (vcount - 1 != b) && (0 != a) && (0 != b))
			edgesWithoutAB.Add(new Edge(vertices[vcount - 1], vertices[0]));
		
		
		Vector2 p1 = vertices[a];
		Vector2 p2 = vertices[b];
		
		//check if there are no intersections with no adjacent edges
		foreach (var edge in edgesWithoutAB)
		{
			Intersection isc = new Intersection(p1, p2, edge.p1, edge.p2);
			//Debug.Log(p1 + " " + p2 + " with " + edge.p1 + " " +  edge.p2 + " : " + isc.haveIntersection);
			if(isc.haveIntersection)
			{
				return false;
			}
		}
		
		//check that at least one point of segment lies inside of polygon
		//i picked the center one
		Vector2 center = (p2 + p1)/2f;
		
		inside = IsPointInside(center);
		
		return inside;
	}

	//check that v is inside
	public bool IsEdgeLiesInsideOfPolygon(Vector2 v, int b)
	{
		bool inside = false;
		
		//TODO: refactor edges without a vertexes
		List<Edge> edgesWithoutAB = new List<Edge>(vcount);
		for(int i=0; i < vcount - 1; i++)
		{
			if(i != b && i+1 != b)
				edgesWithoutAB.Add(new Edge(vertices[i], vertices[i+1]));
		}
		if((vcount - 1 != b) && (0 != b))
			edgesWithoutAB.Add(new Edge(vertices[vcount - 1], vertices[0]));
		
		
		Vector2 p1 = v;
		Vector2 p2 = vertices[b];
		
		//check if there are no intersections with no adjacent edges
		foreach (var edge in edgesWithoutAB)
		{
			Intersection isc = new Intersection(p1, p2, edge.p1, edge.p2);
			//Debug.Log(p1 + " " + p2 + " with " + edge.p1 + " " +  edge.p2 + " : " + isc.haveIntersection);
			if(isc.haveIntersection)
			{
				return false;
			}
		}
		
		//check that at least one point of segment lies inside of polygon
		//i picked the center one
		Vector2 center = (p2 + p1)/2f;
		
		inside = IsPointInside(center);
		
		return inside;
	}

	//TODO: count vertexes
	public bool IsPointInside(Vector2 point)
	{
		//Debug.LogWarning ("IsPointInsideOfPolygon " + point);
		//cast a ray from polygon bounding box to point: (Xmin - e, point.y) to (point.x, point.y)
		//if it gives even numbers of intersections - the point is inside of a polygon
		bool inside = false;

		Vector2 bound = new Vector2(xMin-1, point.y); //let e = 1
		int intersectionsCount = Intersection.GetIntersections (new Edge(bound, point), edges).Count();
		//Debug.LogWarning("intersections: " + intersectionsCount);
		inside = intersectionsCount%2 == 1;
		//Debug.LogWarning ("inside: " + inside);
		return inside;
	}
	
	public List<Vector2[]> SplitByInteriorVertex()
	{
		//Debug.Log("SplitByInteriorVertex, vcount:" + vcount);
		List<Vector2[]> parts = new List<Vector2[]> ();
		int interiorVertex = GetInteriorVertex();
		if(interiorVertex >= 0)
		{
			//Debug.Log("SplitByInteriorVertex: got vertex");
			parts = SplitByInteriorVertex(interiorVertex);
		}
		else
		{
			//Debug.Log("SplitByInteriorVertex: no vertex");
			parts.Add(vertices);
		}
		//Debug.Log("SplitByInteriorVertex parts: " + parts.Count);
		return parts;
	}

	private int GetInteriorVertex()
	{
		int interiorVertex = -1;

		if(vcount <= 3)
		{
			//Debug.Log("no iterior vertex, it's triangle");
			return -1;
		}

		//Debug.LogWarning("circulatedVertices: " + circulatedVertices.Length);

		Vector2 a;
		Vector2 b;
		//find vertex
		for (int i = 1; i < circulatedVertices.Length-1; i++) 
		{
			a = circulatedVertices[i] - circulatedVertices[i-1];
			b = circulatedVertices[i+1] - circulatedVertices[i];
			float rotate = Math2d.Rotate(ref a, ref b);
			//Debug.LogWarning("rotate: " + rotate);
			if(rotate > 0)
			{
				interiorVertex = i-1;
				//Debug.LogWarning("interiorVertex: " + interiorVertex);
				break;
			}
		}
		return interiorVertex;
	}
	 
	private List<Vector2[]> SplitByInteriorVertex(int interiorIndx)
	{
		List<Vector2[]> parts = new List<Vector2[]> ();

		Vector2 interiorVertex = vertices[interiorIndx];

		int cindex  = interiorIndx + 1;
		Vector2 a = (circulatedVertices[cindex] - circulatedVertices[cindex-1]).normalized;
		Vector2 b = -(circulatedVertices[cindex+1] - circulatedVertices[cindex]).normalized;

		Vector2 bisector = interiorVertex + (a + b).normalized * 3 * R;
		//Debug.LogWarning("vertex: " + vIndx + " bisector: "+ bisector);

		int edgeIndex = -1;
		float lastDistance = float.MaxValue;
		Intersection intersection = null;
		for (int i = 0; i < vcount; i++)
		{
			Intersection insc = new Intersection(edges[i].p1, edges[i].p2, interiorVertex, bisector);
			if(insc.haveIntersection && insc.intersection != interiorVertex)
			{
				float distSqr = (interiorVertex - insc.intersection).sqrMagnitude;
				if(intersection == null || lastDistance < distSqr)
				{
					edgeIndex = i;
					lastDistance = distSqr;
					intersection = insc;
				}
			}
		}

		if(intersection == null)
		{
			parts.Add(vertices);
			return parts;
		}

		if(intersection.intersection == edges[edgeIndex].p1)
		{
			return SplitBy2Vertices(edgeIndex, interiorIndx);
		}
		else if(intersection.intersection == edges[edgeIndex].p2)
		{
			return SplitBy2Vertices(Next(edgeIndex), interiorIndx);
		}

		List<Vector2> part1 = GetVertices(interiorIndx, edgeIndex);
		part1.Add(intersection.intersection);

		List<Vector2> part2 = GetVertices(Next(edgeIndex), interiorIndx);
		part2.Add(intersection.intersection);

		parts.Add(part1.ToArray());
		parts.Add(part2.ToArray());
		return parts;
	}


	//TODO: test
	public  List<Vector2[]> SplitBy2Vertices(int index1, int index2)
	{
		int min = Mathf.Min (index1, index2);
		int max = Mathf.Max (index1, index2);
		
		List<Vector2> vertices1 = new List<Vector2> ();
		List<Vector2> vertices2 = new List<Vector2> ();
		
		for (int i = 0; i < vcount; i++) 
		{
			if(i <= min || i >= max)
			{
				vertices1.Add(vertices[i]);
			}
			
			if(i >= min && i <= max)
			{
				vertices2.Add(vertices[i]);
			}
		}

		List<Vector2[]> parts = new List<Vector2[]>();
		parts.Add(vertices1.ToArray());
		parts.Add(vertices2.ToArray());
		return parts;
	}


	//splits by center mass to center of the edge
	//no check for edge inside
	public List<Vector2[]> SplitByMassCenterAndEdgesCenters()
	{
		List<Vector2[]> parts = new List<Vector2[]>();

		List<int> edgeIndexes = GetLagestEdges(3);
		edgeIndexes.Sort();
		edgeIndexes.Add(edgeIndexes[0]);//circulated
		for (int i = 0; i < edgeIndexes.Count-1; i++) 
		{
			int edge1 = edgeIndexes[i];
			int edge2 = edgeIndexes[i+1];

			List<Vector2> part = GetVertices(Next(edge1), edge2);
			part.Add(edges[edge2].GetMiddle());
			part.Add(massCenter);
			part.Add(edges[edge1].GetMiddle());
			parts.Add(part.ToArray());
		}

		return parts;
	}

	private List<int> GetLagestEdges(int count)
	{
		List<Inx2Len> sortedEdges = new List<Inx2Len>();
		for (int i = 0; i < vcount; i++) 
		{
			sortedEdges.Add(new Inx2Len(edges[i].getSqrLength(), i));
		}
		sortedEdges.Sort();//acsending order
		
		//take last longest edges
		List<int> edgeIndexes = new List<int>();
		for (int i = sortedEdges.Count - 1; i >= sortedEdges.Count - count; i--)
		{
			edgeIndexes.Add(sortedEdges[i].indx);
		}
		return edgeIndexes;
	}

	//splits by center mass to center of the edge
	//no check for edge inside
	public List<Vector2[]> SplitByMassCenterVertexAndEdgeCenter()
	{
		//Debug.LogWarning("SplitByMassCenterVertexAndEdgeCenter");
		int edge = GetLagestEdges(1)[0];
		int vertex = UnityEngine.Random.Range(0, vcount);
		
		List<Vector2[]> parts = new List<Vector2[]>();

		{
			List<Vector2> part = GetVertices(vertex, edge);
			part.Add(edges[edge].GetMiddle());
			part.Add(massCenter);
			parts.Add(part.ToArray());
		}

		{
			List<Vector2> part = GetVertices(Next(edge), vertex);
			part.Add(massCenter);
			part.Add(edges[edge].GetMiddle());
			parts.Add(part.ToArray());
		}

		//Debug.LogWarning("SplitByMassCenterVertexAndEdgeCenter parts: " + parts.Count);
		return parts;
	}

	private List<Vector2> GetVertices(int from, int to)
	{
		List<Vector2> part = new List<Vector2>();
		int vtx = from;
		while(true)
		{
			part.Add(vertices[vtx]);
			if(vtx == to) break;
			vtx = Next(vtx);
		}
		return part;
	}
	
	private class Inx2Len : IComparable<Inx2Len>
	{
		float sqrLen;
		public int indx;
		
		public Inx2Len(float sqrLen, int indx)
		{
			this.sqrLen = sqrLen;
			this.indx = indx;
		}
		
		public int CompareTo(Inx2Len other)
		{
			if(other == null)
				return 1;
			
			return sqrLen.CompareTo(other.sqrLen);
		}
	}
}

