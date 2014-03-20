using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour 
{
	public Transform cacheTransform;
	public Polygon polygon;
	public Mesh mesh;


	[SerializeField] private float health;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void SetPolygon(Polygon polygon)
	{
		this.polygon = polygon;
		health = Mathf.Sqrt(polygon.area);//  polygon.R * Mathf.Sqrt(polygon.R) / 3f;
	}

	public void SetColor(Color col)
	{
		int len = mesh.colors.Length;
		Color [] colors = new Color[len];
		for (int i = 0; i < len; i++) 
		{
			colors[i] = col;
		}
		mesh.colors = colors;
	}

	public List<Vector2[]> Split()
	{
		//Debug.LogWarning("Split");
		List<Vector2[]> parts = polygon.SplitByInteriorVertex ();
		if(parts.Count < 2)
		{
			if(polygon.vcount == 3 || Chance(0.5f))
			{
				parts = polygon.SplitByMassCenterAndEdgesCenters();
			}
			else
			{
				parts = polygon.SplitByMassCenterVertexAndEdgeCenter();
			}
		}
		
		List<Vector2[]> deepestParts = new List<Vector2[]>();
		foreach(var part in parts)
		{
			if(Chance(0.4f))
			{
				deepestParts.Add(part);
			}
			else
			{
				Polygon p = new Polygon(part);
				deepestParts.AddRange(p.SplitByInteriorVertex ());
			}
		}
		return deepestParts;
	}
	
	private bool Chance(float chance)
	{
		return chance > UnityEngine.Random.Range(0f, 1f);
	}

	public void Hit(float dmg)
	{
		health -= dmg;
	}
	
	public bool IsKilled()
	{
		return health <= 0;
	}

	public virtual void Tick(float delta)
	{

	}

}
