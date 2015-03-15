using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour, IPolygonGameObject
{
	//basic
	public GameObject gameObj{get{return gameObject;}}
	public Polygon polygon{ get; private set;}

	public Polygon globalPolygon{get; set;}
	public Transform cacheTransform{ get; private set;}
	public Mesh mesh;
	public PolygonCreator.MeshDataUV meshUV{ get; set;}
	public Material mat{get; set;}

	//collision
	public int layer{ get; set;}
	public int collision{ get; set;}

	//physical
	public float density{ get; set;}
	public float mass{ get; private set;}
	public float inertiaMoment{ get; private set;}
	public Vector3 velocity{ get; set;}
	public float rotation{ get; set;}


	public enum DestructionType
	{
		eNormal,
		eComplete,
		eJustDestroy,
	}
	public DestructionType destructionType{get; set;}
	public bool destroyOnBoundsTeleport{get; set;}

	protected float fullHealth;
	[SerializeField] protected float currentHealth;
	public event Action<float> healthChanged;

	public List<Gun> guns { get; set;}


	public IPolygonGameObject target{ get; private set;}
	public ITickable targetSystem;

	protected Shield shield = null;
	[SerializeField] private PolygonGameObject shieldGO;

	public DropID dropID{get; set;}

	public DeathAnimation deathAnimation{get; set;}

	protected virtual void Awake () 
	{
		cacheTransform = transform;
		guns = new List<Gun>();
	}

	public void SetPolygon(Polygon polygon)
	{
		if(cacheTransform == null)
			cacheTransform = transform;

		this.polygon = polygon;
		density = 1; //TODO
		mass = polygon.area * density;
		float approximationR = polygon.R * 4f / 5f;
		inertiaMoment = mass * approximationR * approximationR / 2f;
		fullHealth = Mathf.Sqrt(mass) * healthModifier;//  polygon.R * Mathf.Sqrt(polygon.R) / 3f;
		currentHealth = fullHealth;
	}

	public virtual void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
		guns.ForEach(g => g.SetTarget(target));
	}

	public Vector2 position
	{
		get{return cacheTransform.position;}
		set{cacheTransform.position = ((Vector3)value).SetZ(cacheTransform.position.z);}
	}

	protected virtual float healthModifier
	{
		get{return Singleton<GlobalConfig>.inst.GlobalHealthModifier;}
	}

	public Color GetColor()
	{
		return mesh.colors [0];
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

	public void SetShield(ShieldData shieldData)
	{
		shield = new Shield (shieldData);
		
		Color shCol = Color.green;
		shCol.a = 0.3f;
		Vector2[] v = Math2d.OffsetVerticesFromCenter (polygon.vertices, 0.6f);
		shieldGO = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(v, shCol);
		shieldGO.cacheTransform.parent = cacheTransform;
		shieldGO.cacheTransform.localPosition = new Vector3 (0, 0, 1);
		shield.SetShieldGO (shieldGO);
	}

	public virtual void Tick(float delta)
	{
		if(targetSystem != null)
			targetSystem.Tick (delta);

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
