using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PolygonCreator 
{
	public static Vector2[] GetRectShape(float halfWidth, float halfHeight)
	{
		return new Vector2[]
		{
			new Vector2(halfWidth, halfHeight),
			new Vector2(halfWidth, -halfHeight),
			new Vector2(-halfWidth, -halfHeight),
			new Vector2(-halfWidth, halfHeight),
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
			vertices[i] = GetVertex(r, angle);
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
		for(int i = 0; i < spikesCount; i++)
		{
			vertices[3 * i] = GetVertex(minR, angle);
			angle += deltaAngle;

			vertices[3*i + 1] = GetVertex(maxR, angle);
			spikes[i] = 3*i + 1; 
			angle += deltaAngle;

			vertices[3*i + 2] = GetVertex(minR, angle);
			angle += deltaAngle;
		}
		
		return vertices;
	}

	public static Vector2[] CreateTowerPolygonVertices(float R, float canonsSize, int sides, out int[] cannons)
	{
		float Rcannon = R - canonsSize;
		int vcount = sides * 4;
		Vector2[] vertices = new Vector2[vcount];
		cannons = new int[sides];
		float sideAngle = - 2f * Mathf.PI / sides;
		float angle = 0;
		for(int i = 0; i < sides; i++)
		{
			vertices[4 * i] = GetVertex(R, angle);

			angle += sideAngle * (2f/5f);
			vertices[4 * i + 1] = GetVertex(R, angle);

			angle += sideAngle / 10f;
			vertices[4 * i + 2] = GetVertex(Rcannon, angle);
			cannons[i] = 4 * i + 2;

			angle += sideAngle / 10f;
			vertices[4 * i + 3] = GetVertex(R, angle);

			angle += sideAngle* (2f/5f);
		}
		
		return vertices;
	}

	private static Vector2 GetVertex(float r, float angle)
	{
		return new Vector2(r*Mathf.Cos(angle), r*Mathf.Sin(angle));
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
		float area;
		Vector2 pivot = Math2d.GetMassCenter(vertices, out area);
		Math2d.ShiftVertices(vertices, -pivot);

		Polygon polygon = new Polygon(vertices);
		polygon.SetArea(area);

		GameObject polygonGo = new GameObject();
		T gamePolygon = polygonGo.AddComponent<T>();
		gamePolygon.SetPolygon(polygon);
		gamePolygon.cacheTransform.Translate(new Vector3(pivot.x, pivot.y, 0)); 

		AddRenderComponents (gamePolygon, color);

		return gamePolygon;
	}

	public static bool CheckIfVerySmallOrSpiky(Polygon polygon)
	{
		return (polygon.area < 0.5 || (polygon.area < 1.5 && polygon.area/polygon.Rsqr < 0.5));
	}

}


