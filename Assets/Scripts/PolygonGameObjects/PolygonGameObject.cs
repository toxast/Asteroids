using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour , IGotTarget
{
	public Transform cacheTransform;
	public Polygon polygon;
	public Mesh mesh;

	public int layer;
	public int collision;


	public PolygonGameObject shieldGO;

	public DropID dropID;

	protected float fullHealth;
	[SerializeField] protected float currentHealth;

	protected Shield shield = null;

	public float density = 1;
	public float mass;
	public float inertiaMoment;
	public Vector3 velocity;
	public float rotation;

	public DeathAnimation deathAnimation;

	protected PolygonGameObject target;
	public List<Gun> guns = new List<Gun>();

	public event Action<float> healthChanged;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public virtual void SetTarget(PolygonGameObject target)
	{
		this.target = target;
		guns.ForEach(g => g.SetTarget(target));
	}

	public Vector2 position
	{
		get{return cacheTransform.position;}
		set{cacheTransform.position = ((Vector3)value).SetZ(cacheTransform.position.z);}
	}

	public void SetShield(ShieldData shieldData)
	{
		shield = new Shield (shieldData);

		Color shCol = Color.green;
		shCol.a = 0.3f;
		Vector2[] v = Math2d.OffsetVerticesFromCenter (polygon.vertices, 0.4f);
		shieldGO = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(v, shCol);
		shieldGO.cacheTransform.parent = cacheTransform;
		shieldGO.cacheTransform.localPosition = new Vector3 (0, 0, 1);
		shield.SetShieldGO (shieldGO);
	}

	public void SetPolygon(Polygon polygon)
	{
		this.polygon = polygon;
		mass = polygon.area * density;
		float approximationR = polygon.R * 4f / 5f;
		inertiaMoment = mass * approximationR * approximationR / 2f;
		fullHealth = Mathf.Sqrt(mass) * healthModifier;//  polygon.R * Mathf.Sqrt(polygon.R) / 3f;
		currentHealth = fullHealth;
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

		{
			List<Vector2[]> parts2 = new List<Vector2[]>();
			foreach (var part in parts) 
			{
				Polygon p = new Polygon(part);
				if(p.GetInteriorVerticesCount() > 3)
				{
					parts2.AddRange(p.SplitByInteriorVertex ());
				}
				else
				{
					parts2.Add(p.vertices);
				}
			}
			parts = parts2;
		}

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
				
				{
					List<Vector2[]> parts2 = new List<Vector2[]>();
					foreach (var part in parts) 
					{
						Polygon p = new Polygon(part);
						parts2.AddRange(p.SplitByInteriorVertex ());
					}
					parts = parts2;
				}

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
		if(shield != null)
		{
			dmg = shield.Deflect(dmg);
		}

		currentHealth -= dmg;
	}

	public void Kill()
	{
		currentHealth = 0;
	}
	
	public bool IsKilled()
	{
		return currentHealth <= 0;
	}

	public virtual void Tick(float delta)
	{
		cacheTransform.position += velocity * delta;
		cacheTransform.Rotate(Vector3.back, rotation*delta);

		if (deathAnimation != null)
			deathAnimation.Tick (delta);

		ShieldsTick (delta);
	}

	private void ShieldsTick(float delta)
	{
		if(shield != null)
		{
			shield.Tick(delta);
		}
	}

	protected void ChangeVertex(int indx, Vector2 v)
	{
		Vector3[] vertx3d = mesh.vertices;
		vertx3d[indx] =  new Vector3(v.x, v.y, 0f);
		mesh.vertices = vertx3d;
		
		polygon.ChangeVertex(indx, v);
	}

}
