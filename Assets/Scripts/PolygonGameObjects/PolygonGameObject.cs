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
	public int layerNum{ get; set;}
	public int layer{ get; set;}
	public int collision{ get; set;}

	//physical
	public float density{ get; set;}
	public float healthModifier{ get; set;}
	public float collisionDefence{ get; set;}
	public float collisionAttackModifier{ get; set;}

	//calculated
	public float mass{ get; private set;}
	public float inertiaMoment{ get; private set;}

	public int reward{ get; set;}

	//momentum
	public Vector2 velocity{ get; set;}
	public float rotation{ get; set;}
	public Vector2 position
	{
		get{return cacheTransform.position;}
		set{cacheTransform.position = ((Vector3)value).SetZ(cacheTransform.position.z);}
	}

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

	public List<Gun> guns { get; private set;}
	protected int linkedGunTick = 0;
	protected List<int> linkedGuns = new List<int>();
	protected List<int> notLinkedGuns = new List<int>();

	public IPolygonGameObject target{ get; private set;}
	public ITickable targetSystem;

	protected Shield shield = null;
	[SerializeField] private PolygonGameObject shieldGO;

	public PolygonGameObject minimapIndicator { get; set;}

	public List<PolygonGameObject> turrets { get; set;}

	public DropID dropID{get; set;}

	public DeathAnimation deathAnimation{get; set;}

	protected virtual void Awake () 
	{
		cacheTransform = transform;
		guns = new List<Gun>();
		turrets = new List<PolygonGameObject> ();
	}

	public void SetPolygon(Polygon polygon)
	{
		if(cacheTransform == null)
			cacheTransform = transform;
		
		this.polygon = polygon;
	}

	public void InitPolygonGameObject(PhysicalData physics)
	{
		this.density = physics.density;
		this.healthModifier = physics.healthModifier;
		this.collisionDefence = physics.collisionDefence;
		this.collisionAttackModifier = physics.collisionAttackModifier;

		mass = polygon.area * density;
		float approximationR = polygon.R * 4f / 5f;
		inertiaMoment = mass * approximationR * approximationR / 2f;
		fullHealth = Mathf.Pow(polygon.area, 0.8f) * healthModifier / 2f;//  polygon.R * Mathf.Sqrt(polygon.R) / 3f;
//		Debug.LogWarning (mass + " " + fullHealth);
		currentHealth = fullHealth;
	}

	public void SetGuns(List<Gun> guns, List<int> linked = null)
	{
		this.guns = new List<Gun>(guns);
		this.notLinkedGuns = new List<int> ();
		this.linkedGuns = new List<int> ();
		if (linked == null)
			linked = new List<int> ();

		for (int i = 0; i < guns.Count; i++) {
			var addList = (linked.Contains(i)) ? linkedGuns : notLinkedGuns;
			addList.Add(i);
		}

		this.linkedGunTick = 0;
		for (int i = 1; i < linkedGuns.Count; i++) {
			guns[linkedGuns[i]].ResetTime();
		}
	}

	public virtual void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
		guns.ForEach(g => g.SetTarget(target));
	}

	public void AddTurret(Place place, PolygonGameObject turret)
	{
		Math2d.PositionOnParent(turret.cacheTransform, place, cacheTransform, true, -1);
		turret.SetCollisionLayerNum (layerNum);
		turrets.Add (turret);
	}

	public virtual void SetCollisionLayerNum (int layerNum)
	{
		this.layerNum = layerNum;
		layer = 1 << layerNum;
		collision = CollisionLayers.GetLayerCollisions (layerNum);
		turrets.ForEach (t => t.SetCollisionLayerNum (layerNum));
	}

//	protected virtual float healthModifier
//	{
//		get{return Singleton<GlobalConfig>.inst.GlobalHealthModifier;}
//	}

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
		Vector2[] v = Math2d.OffsetVerticesFromCenter (polygon.circulatedVertices, 0.3f);
		shieldGO = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(v, shCol);
		shieldGO.name = "shield";
		shieldGO.cacheTransform.parent = cacheTransform;
		shieldGO.cacheTransform.localPosition = new Vector3 (0, 0, 1);
		shield.SetShieldGO (shieldGO);
	}

	public virtual void Tick(float delta)
	{
		if(targetSystem != null)
			targetSystem.Tick (delta);

		position += velocity * delta;
		ApplyRotation (delta);

		if (deathAnimation != null)
			deathAnimation.Tick (delta);

		ShieldsTick (delta);

		foreach (var t in turrets)
		{
			t.Tick(delta);
		}
	}

	protected virtual void ApplyRotation(float dtime)
	{
		cacheTransform.Rotate(Vector3.back, rotation*dtime);
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
