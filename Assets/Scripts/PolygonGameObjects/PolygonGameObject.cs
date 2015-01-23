﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour
{
	public Transform cacheTransform;
	public Polygon polygon;
	public Mesh mesh;


	protected float fullHealth;
	[SerializeField] protected float health;
	public float density = 1;
	public float mass;
	public float inertiaMoment;
	public Vector3 velocity;
	public float rotation;

	public bool markedForDeath = false;
	public event Action<float> healthChanged;


	void Awake () 
	{
		cacheTransform = transform;
	}

	public void SetPolygon(Polygon polygon)
	{
		this.polygon = polygon;
		mass = polygon.area * density;
		float approximationR = polygon.R * 4f / 5f;
		inertiaMoment = mass * approximationR * approximationR / 2f;
		fullHealth = Mathf.Sqrt(mass) * healthModifier;//  polygon.R * Mathf.Sqrt(polygon.R) / 3f;
		health = fullHealth;
	}

	protected virtual float healthModifier
	{
		get{return Singleton<GlobalConfig>.inst.GlobalHealthModifier;}
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

	public void SetAlpha(float a)
	{
		Color [] colors = mesh.colors;
		for (int i = 0; i < colors.Length; i++) 
		{
			colors[i].a = a;
		}
		mesh.colors = colors;
	}

	public List<Vector2[]> Split()
	{
		List<Vector2[]> parts = polygon.SplitByInteriorVertex ();
		bool success = parts.Count >= 2;
		if(!success)
		{
			if(polygon.vcount == 3 || Chance(0.5f))
			{
				parts = polygon.SplitByMassCenterAndEdgesPoints();
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

		List<Vector2[]> deepestParts2 = new List<Vector2[]> ();
		foreach(var part in deepestParts)
		{
			Polygon p = new Polygon(part);
			deepestParts2.AddRange(p.SplitSpike());
		}

		return deepestParts2;
	}
	
	private bool Chance(float chance)
	{
		return chance > UnityEngine.Random.Range(0f, 1f);
	}

	public virtual void Hit(float dmg)
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

	protected void ChangeVertex(int indx, Vector2 v)
	{
		Vector3[] vertx3d = mesh.vertices;
		vertx3d[indx] =  new Vector3(v.x, v.y, 0f);
		mesh.vertices = vertx3d;
		
		polygon.ChangeVertex(indx, v);
	}

}
