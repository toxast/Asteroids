using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : MonoBehaviour
{
	public Transform cacheTransform;
	public Polygon polygon;

	public Vector3 velocity;
	float rotation;
	private float health;

	public void Init(Polygon polygon)
	{
		this.polygon = polygon;
		cacheTransform = transform;

		float speed = Random.Range(1f, 4f);
		float a = (Random.Range(0f, 359f) * Mathf.PI) / 180f;
		velocity = new Vector3(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed, 0f);
		rotation = Random.Range(30f, 90f);

		health = polygon.R * Mathf.Sqrt(polygon.R) / 3f;

		//z-fighting
		cacheTransform.position = new Vector3(cacheTransform.position.x, cacheTransform.position.y, Random.Range(-1f, 0f));
	}

	public void Hit(float dmg)
	{
		health -= dmg;
	}

	public bool IsKilled()
	{
		return health <= 0;
	}

	public void Tick(float delta)
	{
		cacheTransform.position += velocity * delta;
		cacheTransform.Rotate(Vector3.back, rotation*delta);
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

}
