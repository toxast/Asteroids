using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : MonoBehaviour
{
	public Transform cacheTransform;
	public Polygon polygon;

	public Vector2 velocity;
	float rotation;


	public void Init(Polygon polygon)
	{
		this.polygon = polygon;
		cacheTransform = transform;

		float speed = 3f;
		float a = (Random.Range(0f,359f) * Mathf.PI) / 180f;
		velocity = new Vector2(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed);

		rotation = 1f;

		cacheTransform.position = new Vector3(cacheTransform.position.x, cacheTransform.position.y, Random.Range(-1f, 0f));
	}

	public void Tick(float delta)
	{
		Vector2 move = velocity*delta;
		cacheTransform.position += new Vector3(move.x, move.y, 0);
		cacheTransform.Rotate(Vector3.back, rotation);
	}

	public List<Vector2[]> Split()
	{
		bool byMassCenter;
		List<Vector2[]> parts = SplitByInteriorVertexOrMassCenter (polygon, out byMassCenter);
		if (!byMassCenter)
		{
			List<Vector2[]> deepestParts = new List<Vector2[]>();
			foreach(var part in parts)
			{
				Polygon p = new Polygon(part);
				Vector2 massCenter = Math2d.GetMassCenter(p.edges);
				p.SetMassCenter(massCenter);
				deepestParts.AddRange(p.SplitByInteriorVertex ());
			}
			return deepestParts;
		}
		else
		{
			return parts;
		}
	}

	private List<Vector2[]> SplitByInteriorVertexOrMassCenter(Polygon p, out bool byMassCenter)
	{
		List<Vector2[]> parts = p.SplitByInteriorVertex ();
		if(parts.Count < 2)
		{
			byMassCenter = true;
			return p.SplitByMassCenter();
		}
		else
		{
			byMassCenter = false;
			return parts;
		}
	}

}
