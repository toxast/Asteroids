using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PolygonCreator 
{
	public static Vector2[] GetRectShape(float x, float y)
	{
		return new Vector2[]
		{
			new Vector2(x, y),
			new Vector2(x, -y),
			new Vector2(-x, -y),
			new Vector2(-x, y),
		};
	}

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

	public static void AddRenderComponents(PolygonGameObject gamePolygon, Color color)
	{
		Mesh msh = CreatePolygonMesh(gamePolygon.polygon.vertices, color);
		
		// Set up game object with mesh;
		MeshRenderer renderer = gamePolygon.gameObject.AddComponent<MeshRenderer>();
		MeshFilter filter = gamePolygon.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		Material mat = Resources.Load("Materials/asteroidMaterial", typeof(Material)) as Material;
		renderer.sharedMaterial = mat;
		renderer.castShadows = false;
		renderer.receiveShadows = false;

		gamePolygon.mesh = filter.mesh;
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

	public static Vector2[] CreateSpikyPolygonVertices(float maxR, float minR, int spikesCount, out int[] spikes)
	{
		spikes = new int[spikesCount];
		int vcount = spikesCount * 3;

		if(spikesCount < 2)
		{
			throw new UnityException("cant create polygon from " + spikesCount + " vertices");
		}
		
		if(maxR <= 0 || maxR < minR)
		{
			throw new UnityException("wrong size of polygon: " + minR + "-" + maxR);
		}


		Vector2[] vertices = new Vector2[vcount];
		
		float deltaAngle = -2f * Mathf.PI/vcount;
		float angle = 0;
		float x;
		float y;
		for(int i = 0; i < spikesCount; i++)
		{
			x = minR*Mathf.Cos(angle);
			y = minR*Mathf.Sin(angle);
			vertices[3 * i] = new Vector2(x,y);
			angle += deltaAngle;

			x = maxR*Mathf.Cos(angle);
			y = maxR*Mathf.Sin(angle);
			vertices[3*i + 1] = new Vector2(x,y);
			spikes[i] = 3*i + 1; 
			angle += deltaAngle;

			x = minR*Mathf.Cos(angle);
			y = minR*Mathf.Sin(angle);
			vertices[3*i + 2] = new Vector2(x,y);
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

	public static T CreatePolygonGOByMassCenter<T>(Vector2[] vertices, Color color)
		where T: PolygonGameObject
	{
		Vector2 pivot = Math2d.GetMassCenter(vertices);
		Math2d.ShiftVertices(vertices, -pivot);

		Polygon polygon = new Polygon(vertices);

		GameObject polygonGo = new GameObject();
		T gamePolygon = polygonGo.AddComponent<T>();
		gamePolygon.SetPolygon(polygon);
		gamePolygon.cacheTransform.Translate(new Vector3(pivot.x, pivot.y, 0)); 

		AddRenderComponents (gamePolygon, color);

		return gamePolygon;
	}

}


