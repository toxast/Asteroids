using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PolygonCreator 
{

	public class MeshDataUV
	{
		public Vector2 offset;
		public Vector2 centerOffsetUnits;
	}

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

	private static Material _defaultMaterial;
	public static Material defaultMaterial
	{
		get{
			if(_defaultMaterial == null)
			{
				_defaultMaterial = Resources.Load("Materials/asteroidMaterial", typeof(Material)) as Material;
			}
			return _defaultMaterial;
		}
	}


	private static Material _texturedMaterial;
	public static Material texturedMaterial
	{
		get{
			if(_texturedMaterial == null)
			{
				_texturedMaterial = Resources.Load("Materials/textured", typeof(Material)) as Material;
			}
			return _texturedMaterial;
		}
	}

	public static T CreatePolygonGOByMassCenter<T>(Vector2[] vertices, Color color, Material mat = null, MeshDataUV meshUV = null)
		where T: PolygonGameObject
	{
		float area;
		Vector2 pivot = Math2d.GetMassCenter(vertices, out area);
		Math2d.ShiftVertices(vertices, -pivot);
		
		Polygon polygon = new Polygon(vertices);
		polygon.SetArea(area);
		
		GameObject polygonGo = new GameObject();
		T gamePolygon = polygonGo.AddComponent<T>();
		if(meshUV != null)
		{
			gamePolygon.meshUV = new MeshDataUV ();
			gamePolygon.meshUV.offset = meshUV.offset;
			gamePolygon.meshUV.centerOffsetUnits = meshUV.centerOffsetUnits + pivot;
		}
		gamePolygon.SetPolygon(polygon);
		gamePolygon.cacheTransform.Translate(new Vector3(pivot.x, pivot.y, 0)); 

		if(mat == null)
			mat = defaultMaterial;

		AddRenderComponents (gamePolygon, color, mat);
		
		return gamePolygon;
	}

	public static void AddRenderComponents(PolygonGameObject gamePolygon, Color color, Material mat)
	{
		MeshDataUV newMeshUV;
		Mesh msh = CreatePolygonMesh(gamePolygon.polygon.vertices, color, gamePolygon.meshUV, out newMeshUV);
		gamePolygon.meshUV = newMeshUV;
		// Set up game object with mesh;
		MeshRenderer renderer = gamePolygon.gameObject.AddComponent<MeshRenderer>();
		MeshFilter filter = gamePolygon.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		gamePolygon.mat = mat;
		renderer.sharedMaterial = mat;
		renderer.castShadows = false;
		renderer.receiveShadows = false;
		gamePolygon.mesh = filter.mesh;
	}

	public static GameObject CreateLazerGO()
	{
		var mat = defaultMaterial;
		GameObject g = new GameObject("lazer");
		var vrts = new Vector2[]
		{
			new Vector2(10, 1),
			new Vector2(10, -1),
			new Vector2(0, -1),
			new Vector2(0, 1),
		};
		MeshDataUV newMeshUV;
		Mesh msh = CreatePolygonMesh(vrts, Color.red, null, out newMeshUV);
		msh.uv = new Vector2[]
		{
			new Vector2 (0, 1),
			new Vector2 (1, 1),
			new Vector2 (1, 0),
			new Vector2 (0, 0),
		};
		MeshRenderer renderer = g.AddComponent<MeshRenderer>();
		MeshFilter filter = g.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
//		gamePgolygon.mat = mat;
		renderer.sharedMaterial = mat;
		renderer.castShadows = false;
		renderer.receiveShadows = false;
//		gamePolygon.mesh = filter.mesh;

		return g;
	}

	public static void ChangeLazerMesh(Mesh m, float dist, float width)
	{
		var vrts = new Vector3[]
		{
			new Vector3(dist, width/2f, 0),
			new Vector3(dist, -width/2f, 0),
			new Vector3(0, -width/2f, 0),
			new Vector3(0, width/2f, 0),
		};
		m.vertices = vrts;
		m.RecalculateBounds ();
	}

	public static Mesh CreatePolygonMesh(Vector2[] vertices2D, Color color, MeshDataUV meshD, out MeshDataUV newMeshUV)
	{
		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();
		
		
		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i=0; i<vertices.Length; i++) 
		{
			vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
		}
		
		Color[] colors = new Color[vertices.Length];
		int k = 0;
		while (k < vertices.Length) 
		{
			colors[k] = color;
			k++;
		}
		
		// Create the mesh
		Mesh msh = new Mesh();
		
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.uv = GetUV(vertices, meshD, out newMeshUV);
		msh.colors = colors;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		return msh;
	}

	private static Vector2[] GetUV(Vector3[] vertices, MeshDataUV meshUV, out MeshDataUV newMeshUV)
	{
		float imgSize = 256;
		float pixelsPerUnit = 7;

		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		for (int i = 0; i < vertices.Length; i++) {
			var v = new Vector3(vertices[i].x, vertices[i].y, 0); 
			
			if(v.x < minX)
				minX = v.x;
			
			if(v.y < minY)
				minY = v.y;
			
			if(v.x > maxX)
				maxX = v.x;
			
			if(v.y > maxY)
				maxY = v.y;
		}
		
		float maxSide = Mathf.Max (maxX - minX, maxY - minY);
		float maxSidePx = maxSide * pixelsPerUnit;
		float max = maxSidePx / imgSize;
		
		Vector2 offset = Vector2.zero;
		if(meshUV == null)
		{
			offset = new Vector2(Random.Range(0, 1f - max), Random.Range(0, 1f - max));
		}
		else
		{
			offset = meshUV.offset + (meshUV.centerOffsetUnits + new Vector2(minX, minY)) * pixelsPerUnit / imgSize;
		}

		newMeshUV = new MeshDataUV
		{
			offset = offset,
			centerOffsetUnits = new Vector2(-minX, -minY),
		};

		Vector2[] uvs = new Vector2[vertices.Length];
		for (var i = 0 ; i < uvs.Length; i++)
		{
			uvs[i] = offset + (new Vector2 (vertices[i].x - minX, vertices[i].y - minY) * max) / maxSide;
		}
		
		return uvs;
	}


	/// <summary>
	/// Creates the polygon inside of bagel (inside circle radius minR, outer - maxR)
	/// with given count of vertices to generate 
	/// </summary>
	public static Vector2[] CreateAsteroidVertices(float maxR, float minR, int vertexCount)
	{
		//return GetRectShape (5, 5);
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

		Polygon p = new Polygon (vertices);
		List<Vector2[]> spikes;
		Vector2[] spikeless;
		p.CutOffAllSpikes (out spikes, out spikeless);
		return spikeless;
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

	public static Vector2[] CreatePrefectPolygonVertices(float R, int sidesNum)
	{
		Vector2[] vertices = new Vector2[sidesNum];
		float dAngle = - 2f * Mathf.PI / sidesNum;
		float angle = 0;
		for(int i = 0; i < sidesNum; i++)
		{
			vertices[i] = GetVertex(R, angle);
			angle += dAngle;
		}
		
		return vertices;
	}

	public static Vector2[] CreateTowerVertices2(float R, int sidesNum)
	{
		Vector2[] vertices = new Vector2[sidesNum + 2];
		float dAngle = -2f * Mathf.PI / sidesNum;

		float cannonLength = R*1.3f;
		float angle = -dAngle/5f;
		vertices[0] = GetVertex(cannonLength, angle);

		angle = -angle;
		vertices[1] = GetVertex(cannonLength, angle);

		angle = dAngle/2f;
		for(int i = 2; i < sidesNum + 2; i++)
		{
			vertices[i] = GetVertex(R, angle);
			angle += dAngle;
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







	public static bool CheckIfVerySmallOrSpiky(Polygon polygon)
	{
		return (polygon.area < 0.5 || (polygon.area < 1.5 && polygon.area/polygon.Rsqr < 0.5));
	}

}


