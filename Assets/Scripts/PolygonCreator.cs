using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PolygonCreator 
{
	public static List<Vector2> GetCompleteVertexes(Vector2[] halfVertices, float sizeModifier)
	{
		List<Vector2> vertices = new List<Vector2>();
		vertices.AddRange(halfVertices);
		for (int i = halfVertices.Length - 1; i >= 0; i--) 
		{
			vertices[i] *= sizeModifier;
			if(vertices[i].y != 0)
			{
				vertices.Add(new Vector2(vertices[i].x, -vertices[i].y));
			}
		}

		return vertices; 
	}

	public static void AddRenderComponents(Polygon polygon, GameObject gameObj, Color color)
	{
		Mesh msh = CreatePolygonMesh(polygon.vertices, color);
		
		// Set up game object with mesh;
		MeshRenderer renderer = gameObj.AddComponent<MeshRenderer>();
		MeshFilter filter = gameObj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		Material mat = Resources.Load("Materials/asteroidMaterial", typeof(Material)) as Material;
		renderer.sharedMaterial = mat;
		renderer.castShadows = false;
		renderer.receiveShadows = false;
	}

	/// <summary>
	/// Creates the polygon inside of bagel (inside circle radius minR, outer - maxR)
	/// with given count of vertices to generate 
	/// </summary>
	public static Vector2[] CreatePolygonVertices(float maxR, float minR, int vertexCount)
	{
		if(vertexCount < 3)
		{
			throw new UnityException("cant create polygon from " + vertexCount + " vertices");
		}

		if(maxR <= 0 || maxR < minR)
		{
			throw new UnityException("wrong size of polygon: " + minR + "-" + maxR);
		}

		Vector2[] vertices = new Vector2[vertexCount];
		
		float deltaAngle = -2f * Mathf.PI/vertexCount;
		float angle = 0;
		for(int i = 0; i < vertexCount; i++)
		{
			float r = Random.Range(minR, maxR);
			float x = r*Mathf.Cos(angle);
			float y = r*Mathf.Sin(angle);
			vertices[i] = new Vector2(x,y);
			angle += deltaAngle;
		}

		return vertices;
	}

	private static Mesh CreatePolygonMesh(Vector2[] vertices2D, Color color)
	{
		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
		}
		
		Color[] colors = new Color[vertices.Length];
		int k = 0;
		while (k < vertices.Length) {
			colors[k] = color;
			k++;
		}
		
		// Create the mesh
		Mesh msh = new Mesh();

		msh.vertices = vertices;
		msh.triangles = indices;
		msh.colors = colors;
		msh.RecalculateNormals();
		msh.RecalculateBounds();

		return msh;
	}
}


