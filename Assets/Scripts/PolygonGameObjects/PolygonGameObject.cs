using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour
{
	[SerializeField] bool destroy = false;

	//basic
	public GameObject gameObj{get{return gameObject;}}
	public Polygon polygon{ get; private set;}

	public Polygon globalPolygon{get; set;}
	public Transform cacheTransform{ get; private set;}
	public Mesh mesh;
    public int[] triangulationIndices;
    public PolygonCreator.MeshDataUV meshUV{ get; set;}
	public Material mat;

	//collision
	public int layerNum;
	public int layer;
	public int collision;

	//physical
	public float density;
	public float healthModifier;
	public float collisionDefence;
	public float collisionAttackModifier;

    //calculated
	public float inertiaMoment;
    float _mass;
    public float mass { get { return _mass; } set { _mass = value; massSqrtCalculated = false; } }
    bool massSqrtCalculated = false;
    float _massSqrt;
    public float massSqrt {
        get {
            if (!massSqrtCalculated) { _massSqrt = Mathf.Sqrt(mass); }
            return _massSqrt;
        }
    }


	public int reward;

	//momentum
	public Vector2 velocity;
	public float rotation;
	public Vector2 position
	{
		get{return cacheTransform.position;}
		set{cacheTransform.position = ((Vector3)value).SetZ(cacheTransform.position.z);}
	}

	public float damageOnCollision;
	public float startinglifeTime;
	protected float leftlifeTime;

	public enum DestructionType
	{
		eNormal,
		eComplete,
		eDisappear,
		eSptilOnlyOnHit,//if hit - destroy, if not - just disappear
	}
	public DestructionType destructionType;
	public bool destroyOnBoundsTeleport;

    public bool ignoreBounds = false; //when object is controlled by other positioning logic
    public List<PolygonGameObject> teleportWithMe;

	public float fullHealth{ protected set; get;}
	[SerializeField] protected float currentHealth;
//	public event Action<float> healthChanged;

	public List<Gun> guns { get; private set;}
	protected int linkedGunTick = 0;
	protected List<int> linkedGuns = new List<int>();
	protected List<int> notLinkedGuns = new List<int>();
	protected List<int> spawnerGuns = new List<int>();

	protected List<TickableEffect> effects = new List<TickableEffect> ();
	public PolygonGameObject target{ get; private set;}
	public ITickable targetSystem;

	protected Shield shield = null;
	[SerializeField] private PolygonGameObject shieldGO;

	public PolygonGameObject minimapIndicator;

	public List<PolygonGameObject> turrets;

	public DropID dropID;

	public DeathAnimation deathAnimation;
	public float overrideExplosionDamage;
	public float overrideExplosionRange;

	public bool capturedByEarthSpaceship = false;

    public virtual void OnHit(PolygonGameObject other) { }

	protected virtual void Awake () 
	{
		cacheTransform = transform;
		guns = new List<Gun>();
		turrets = new List<PolygonGameObject> ();
		overrideExplosionDamage = -1;
		overrideExplosionRange = -1;
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

		if (physics.health >= 0) {
			fullHealth = physics.health;
		} else {
			fullHealth = Mathf.Pow(polygon.area, 0.8f) * healthModifier / 2f;
		}
		currentHealth = fullHealth;
	}

	public void InitLifetime(float lifeTime)
	{
		this.startinglifeTime = lifeTime;
		this.leftlifeTime = lifeTime;
	}

	public void SetGuns(List<Gun> guns, List<int> linked = null)
	{
		this.spawnerGuns = new List<int> ();
		this.guns = new List<Gun>(guns);
		this.notLinkedGuns = new List<int> ();
		this.linkedGuns = new List<int> ();
		if (linked == null)
			linked = new List<int> ();

		for (int i = 0; i < guns.Count; i++) {
			var addList = (linked.Contains(i)) ? linkedGuns : notLinkedGuns;
			addList.Add(i);

			if(guns[i] is SpawnerGun)
			{
				spawnerGuns.Add(i);
			}
		}

		this.linkedGunTick = 0;
		for (int i = 1; i < linkedGuns.Count; i++) {
			guns[linkedGuns[i]].ResetTime();
		}
	}

	public bool Expired()
	{
		return HasLifetime && leftlifeTime < 0;
	}

	public virtual void SetTarget(PolygonGameObject target)
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

	public void Accelerate(float delta, float thrust, float stability, float maxSpeed, float maxSpeedSqr, Vector2 dirNormalized)
	{
		#if UNITY_EDITOR
		if(Mathf.Abs(dirNormalized.magnitude - 1f) > 0.1f)
		{
			Debug.LogError("normalize the value! " + dirNormalized + " " + dirNormalized.magnitude);
		}
		#endif

		float deltaV = delta * thrust;
		velocity -= stability * velocity.normalized * deltaV;
		velocity += (1 + stability)* dirNormalized * deltaV;

		if(velocity.sqrMagnitude > maxSpeedSqr)
		{
			velocity = velocity.normalized*(Mathf.Min(velocity.magnitude - deltaV, maxSpeed));
		}
	} 

	public void Brake(float delta, float pBrake)
	{
		if (velocity == Vector2.zero)
			return;

		var newMagnitude = velocity.magnitude - delta * pBrake; 
		if (newMagnitude < 0)
		{
			newMagnitude = 0;
			velocity = Vector2.zero;
		} 
		else
		{
			velocity = velocity.normalized * newMagnitude;
		}
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

	public virtual void SetAlpha(float a)
	{
		Color [] colors = mesh.colors;
		for (int i = 0; i < colors.Length; i++) 
		{
			colors[i].a = a;
		}
		mesh.colors = colors;
	}

    public float GetAlpha() {
        return mesh.colors [0].a;
    }

    public bool increaceAlphaOnHitAndDropInvisibility = false;
    bool _isInvisible = false;
	public bool IsInvisible() {
		return _isInvisible;
	}

    public void SetInvisible(bool invis) {
        _isInvisible = invis;
    }

	public void AddEffect(TickableEffect effect) 
	{
		bool updated = false;
		if (effect.CanBeUpdatedWithSameEffect) {
			var same = effects.Find (e =>  e.IsTheSameEffect(effect));
			if (same != null) {
				same.UpdateBy (effect);
				updated = true;
			}
		} 

		if (!updated) {
			effect.SetHolder (this);
			effects.Add (effect);
		}
	}

    //threshold >= 0
    private List<Vector2[]> SplitUntilInteriorThreshold(Vector2[] verts, int threshold) {
        List<Vector2[]> parts = new List<Vector2[]>();
        Polygon p = new Polygon(verts);
        if (p.GetInteriorVerticesCount() > threshold) {
            var toTest = p.SplitByConcaveVertex();
            foreach (var item in toTest) {
                parts.AddRange(SplitUntilInteriorThreshold(item, threshold));
            }
        } else {
            parts.Add(p.vertices);
        }
        return parts;
    }


	public List<Vector2[]> Split()
	{
		List<Vector2[]> parts = polygon.SplitByConcaveVertex ();

		{
			List<Vector2[]> parts2 = new List<Vector2[]>();
			foreach (var part in parts) 
			{
				Polygon p = new Polygon(part);
				if(p.GetInteriorVerticesCount() >= 3)
				{
                    var toTest = p.SplitByConcaveVertex();
                    foreach (var item in toTest) {
                        parts2.AddRange(SplitUntilInteriorThreshold(item, 2));
                    }
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
						parts2.AddRange(p.SplitByConcaveVertex ());
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
				deepestParts.AddRange(p.SplitByConcaveVertex ());
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
//        Debug.LogError ("hit " + dmg);
		if (increaceAlphaOnHitAndDropInvisibility) {
		    float alpha = GetAlpha ();
			SetAlpha(Mathf.Min(1, alpha + 0.65f));
            SetInvisible(false);
		}

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

	public bool HasLifetime {get { return startinglifeTime > 0;}}

	public virtual void Tick(float delta)
	{
		if (destroy) {
			destroy = false;
			Kill ();
		}
		if (HasLifetime) {
			leftlifeTime -= delta; 
		}
		if (targetSystem != null) {
			targetSystem.Tick (delta);
		}
		position += velocity * delta;
		ApplyRotation (delta);

		if (deathAnimation != null) {
			deathAnimation.Tick (delta);
		}
		ShieldsTick (delta);
		foreach (var t in turrets) {
			t.Tick (delta);
		}

		for (int i = effects.Count - 1; i >= 0; i--) {
			effects [i].Tick (delta);
			if (effects [i].IsFinished ()) {
				effects.RemoveAt (i);
			}
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

	protected void TickGuns(float delta)
	{
		if (deathAnimation != null && deathAnimation.started)
			return;
		
		var d = delta;
		for (int i = 0; i < notLinkedGuns.Count; i++) 
		{
			guns[notLinkedGuns[i]].Tick(d);
		}
		
		if(linkedGuns.Any())
			guns [linkedGuns [linkedGunTick]].Tick (d);
	}

	protected void Shoot()
	{
		for (int i = 0; i < notLinkedGuns.Count; i++) 
		{
			guns[notLinkedGuns[i]].ShootIfReady();
		}
		
		if(linkedGuns.Any() && guns[linkedGuns[linkedGunTick]].ReadyToShoot())
		{
			guns[linkedGuns[linkedGunTick]].ShootIfReady();
			linkedGunTick ++;
			if(linkedGunTick >= linkedGuns.Count)
				linkedGunTick = 0;
		}
	}

	protected void ChangeVertex(int indx, Vector2 v)
	{
		Vector3[] vertx3d = mesh.vertices;
		vertx3d[indx] =  new Vector3(v.x, v.y, 0f);
		mesh.vertices = vertx3d;
		
		polygon.ChangeVertex(indx, v);
	}

	//TODO: refactor this somehow, i need a lot of parametes and control over the particles
	public List<ParticleSystem> SetParticles(List<ParticleSystemsData> datas)
	{
		List<ParticleSystem> result = new List<ParticleSystem> ();
		foreach (var setup in datas) 
		{
			if(setup.prefab == null)
			{
				Debug.LogError("null particle system");
			}
			else
			{
				var inst = Instantiate(setup.prefab) as ParticleSystem;
                var pmain = inst.main;
                if (setup.overrideSize > 0) {
                    pmain.startSizeMultiplier = setup.overrideSize;
                }
                if (setup.overrideDuration > 0) {
                    pmain.duration = setup.overrideDuration;
                }
				if (setup.overrideDelay > 0) {
					pmain.startDelayMultiplier = setup.overrideDelay;
				}
                inst.Play();

                Math2d.PositionOnParent (inst.transform, setup.place, cacheTransform, true, setup.zOffset);
				result.Add (inst);
			}
		}
		return result;
	}

	List<ParticleSystemsData> destroyEffects;
	public void SetDestroyAnimationParticles(List<ParticleSystemsData> datas)
	{
		destroyEffects = new List<ParticleSystemsData>(datas);
	}

    public void AddObjectAsFollower(PolygonGameObject obj) {
        obj.ignoreBounds = true;
        if (teleportWithMe == null) {
            teleportWithMe = new List<PolygonGameObject>();
        }
        Main.PutOnFirstNullOrDestroyedPlace(teleportWithMe, obj);
    }

    public Action OnDestroying;
	public virtual void HandleDestroying() {
		if (destroyEffects != null) {
			foreach (var item in destroyEffects) {
				var inst  = Instantiate(item.prefab) as ParticleSystem;
				Math2d.PositionOnParent (inst.transform, item.place, cacheTransform, false, -1);
				Destroy (inst.gameObject, 5f);
			}
		}

        if (teleportWithMe != null) {
            for (int i = teleportWithMe.Count - 1; i >= 0; i--) {
                var item = teleportWithMe[i];
                if (!Main.IsNull(item)) {
                    item.ignoreBounds = false;
                }
                teleportWithMe[i] = null;
            }
            teleportWithMe.Clear();
        }
        
        if (OnDestroying != null) {
			OnDestroying ();
		}
	}

    public virtual void SetSpawnParent(PolygonGameObject prnt) { }
}
