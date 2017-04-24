using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PolygonGameObject : MonoBehaviour, IFreezble
{
	[SerializeField] bool destroy = false;

	//basic
	public GameObject gameObj{get{return gameObject;}}
	public Polygon polygon{ get; private set;}

	private Polygon _globalPolygon;
	public Polygon globalPolygon {
		get { 
			if (_globalPolygon == null) {
				_globalPolygon = PolygonCollision.GetPolygonInGlobalCoordinates(this);
			}
			return _globalPolygon;
		}
	}
	public void NullifyGlobalPolygon() {
		_globalPolygon = null;
	}

	public Transform cacheTransform{ get; private set;}
	public Mesh mesh;
    public int[] triangulationIndices;
    public PolygonCreator.MeshDataUV meshUV{ get; set;}
	public Material mat;

	//collision
	public int logicNum;//{get; private set;}
	public int layerLogic;//{get; private set;}

	public int collisionNum;//{get; private set;}
	public int layerCollision;//{get; private set;}
	public int collisions;

	//physical
	public float density{ get; private set; }
	public float healthModifier;
	public float collisionDefenceOriginal{ get; private set; }
	public float collisionDefence{ get; private set; }
	public float collisionAttackModifier{ get; private set; }

    //calculated
	public float inertiaMoment;
    float _mass;
    public float mass { get { return _mass; } set { 
			_mass = value; 
			massSqrtCalculated = false; 
			massSqrt85Calculated = false;
			UpdateMassRelatedValues ();
	}}
    bool massSqrtCalculated = false;
    float _massSqrt;
    public float massSqrt {
        get {
			if (!massSqrtCalculated) { _massSqrt = Mathf.Sqrt(mass);  massSqrtCalculated = true;}
            return _massSqrt;
        }
    }

	bool massSqrt85Calculated = false;
	float _massSqrt85;
	public float massSqrt85 {
		get {
			if (!massSqrt85Calculated) { 
				_massSqrt85 = Mathf.Pow (mass, 0.85f); 
				massSqrt85Calculated = true; 
				//Debug.LogError (mass +  " -> sqrt07 " + _massSqrt85); 
			}
			return _massSqrt85;
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
	public float pLeftlifeTime{get{ return leftlifeTime;}}

	public bool showOffScreen = true;

	public enum DestructionType
	{
		eNormal,
		eComplete,
		eDisappear,
		eSplitlOnlyOnHit,//if hit - destroy, if not - just disappear
	}
	public DestructionType destructionType;
	public bool destroyOnBoundsTeleport;

	public enum ePriorityLevel{
		NORMAL = 5,
		LOW = 3,
	}
	public float priorityMultiplier = 1f; //the bigger the value the more like it will be a targrt, DONT set to 0 
	public ePriorityLevel priority = ePriorityLevel.NORMAL;

    public bool ignoreBounds = false; //when object is controlled by other positioning logic
    public List<PolygonGameObject> teleportWithMe;

	public float fullHealth{ protected set; get;}
	[SerializeField] protected float currentHealth;
	public float GetLeftHealthPersentage(){
		return currentHealth / fullHealth;
	}
//	public event Action<float> healthChanged;

	public List<Gun> guns { get; private set;}
	protected List<List<int>> linkedGuns = new List<List<int>>(); //indexes of linked gun, can be a few of them
	protected List<int> linkedGunTick = new List<int>(); //each list of linked guns has a current index. 
	protected List<int> notLinkedGuns = new List<int>();
	protected List<int> spawnerGuns = new List<int>();

	protected List<TickableEffect> effects = new List<TickableEffect> ();
	public PolygonGameObject target{ get; private set;}
	public ITickable targetSystem;

	protected Shield shield = null;
	[SerializeField] private PolygonGameObject shieldGO;
	public Shield GetShield() { return shield; }

	public PolygonGameObject minimapIndicator;

	public List<PolygonGameObject> turrets;

	public DropID dropID;

	public DeathAnimation deathAnimation;
	public float overrideExplosionDamage;
	public float overrideExplosionRange;

	public bool capturedByEarthSpaceship = false;

	[NonSerialized] public IceEffect.Data iceEffectData;
	[NonSerialized] public BurningEffect.Data burnDotData;

    public event Action<PolygonGameObject, float> OnCollision;
    public virtual void OnHit(PolygonGameObject other, float collisonDmgToBeDealt) {
		if (burnDotData != null) {
			other.AddEffect (new BurningEffect (burnDotData));
		}
		if (iceEffectData != null) {
			other.AddEffect (new IceEffect (iceEffectData));
		}

        if (OnCollision != null) {
            OnCollision(other, collisonDmgToBeDealt);
        }
	}

	[NonSerialized] public HeavyBulletEffect.Data heavyBulletData;

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

	public void MultiplyMass(float multiplyBy) {
		mass *= multiplyBy;
	}

	public void MultiplyCollisionAttack(float multiplyBy) {
		collisionAttackModifier *= multiplyBy;
	}

	float _freezeMod = 1f;
	public float freezeMod { 
		get { return _freezeMod; }
		private set {_freezeMod = value; }
	}
	public virtual void Freeze(float mod){
		freezeMod *= mod;
	}

	[NonSerialized] int defChanges = 0;
	public void ChangeCollisionDefence(float def) {
		collisionDefence = def;
		defChanges++;
	}

	public void RestoreCollisionDefence() {
		defChanges--;
		if (defChanges == 0) {
			collisionDefence = collisionDefenceOriginal;
		}
	}

	public void InitPolygonGameObject(PhysicalData physics)
	{
		this.density = physics.density;
		this.healthModifier = physics.healthModifier;
		this.collisionDefence = physics.collisionDefence;
		collisionDefenceOriginal = collisionDefence;
		this.collisionAttackModifier = physics.collisionAttackModifier;

		mass = polygon.area * density;

		if (physics.health >= 0) {
			fullHealth = physics.health;
		} else {
			fullHealth = Mathf.Pow(polygon.area, 0.8f) * healthModifier / 2f;
		}
		currentHealth = fullHealth;
	}

	private void UpdateMassRelatedValues(){
		float approximationR = polygon.R * 4f / 5f;
		inertiaMoment = mass * approximationR * approximationR / 2f;
	}

	public void SetInfiniteLifeTime(){
		startinglifeTime = 0;
	}

	public void InitLifetime(float lifeTime)
	{
		this.startinglifeTime = lifeTime;
		this.leftlifeTime = lifeTime;
	}

	public void SetGuns(List<Gun> guns, List<List<int>> linked = null)
	{
		this.spawnerGuns = new List<int> ();
		this.guns = new List<Gun>(guns);
		this.notLinkedGuns = new List<int> ();
		this.linkedGuns = linked ?? new List<List<int>>();
		for (int i = 0; i < guns.Count; i++) {
			if (!linkedGuns.Exists (list => list.Contains (i))) {
				notLinkedGuns.Add (i);
			}
			if(guns[i] is SpawnerGun) {
				spawnerGuns.Add(i);
			}
		}

		this.linkedGunTick = linkedGuns.ConvertAll (g => 0);
		for (int i = 0; i < linkedGuns.Count; i++) {
			for (int k = 1; k < linkedGuns [i].Count; k++) {
				guns [linkedGuns [i] [k]].SetTimeForNextShot ();
			}
		}
	}

	//no support for linked guns
	public void AddExtraGuns(List<Gun> newGuns)
	{
		for (int i = 0; i < newGuns.Count; i++) {
			int index = guns.Count + i;
			notLinkedGuns.Add(index);
			if(newGuns[i] is SpawnerGun) {
				spawnerGuns.Add(index);
			}
		}
		this.guns.AddRange(newGuns);
	}

	//no support for linked guns
	public void RemoveGuns(List<Gun> gunsToRemove)
	{
		for (int i = 0; i < gunsToRemove.Count; i++) {
			var rgun = gunsToRemove [i];
			var index = guns.FindIndex (g => g == rgun);
			if (index >= 0) {
				notLinkedGuns.Remove (index);
				spawnerGuns.Remove (index);
				guns.RemoveAt (index);
				for (int k = 0; k < notLinkedGuns.Count; k++) {
					if (notLinkedGuns [k] > index) {
						notLinkedGuns [k] = notLinkedGuns [k] - 1;
					}
				}
				for (int k = 0; k < spawnerGuns.Count; k++) {
					if (spawnerGuns [k] > index) {
						spawnerGuns [k] = spawnerGuns [k] - 1;
					}
				}
				rgun.OnGunRemoving ();
			} else {
				Debug.LogError ("gun to remove not found");
			}
		}
	}

	public bool Expired()
	{
		return HasLifetime && leftlifeTime < 0;
	}

	public virtual void SetTarget(PolygonGameObject target)
	{
		//Debug.LogError (this.name + " SetTarget " + (target == null ? "null" : target.name));
		this.target = target;
		guns.ForEach(g => g.SetTarget(target));
	}

	public void AddTurret(Place place, PolygonGameObject turret)
	{
		Math2d.PositionOnParent(turret.cacheTransform, place, cacheTransform, true, -1);
		turret.SetLayerNum (logicNum, collisionNum);
		turrets.Add (turret);
	}

	public virtual void SetLayerNum (int layerNum)
	{
		SetLayerNum (layerNum, layerNum);
	}

	public virtual void SetLayerNum (int layerNum, int collisionNum) {
		this.logicNum = layerNum;
		layerLogic = 1 << layerNum;

		this.collisionNum = collisionNum;
		layerCollision = 1 << collisionNum; //there are exceptions
		collisions = CollisionLayers.GetLayerCollisions (collisionNum);

		turrets.ForEach (t => t.SetLayerNum (layerNum, collisionNum));
	}


	public void Accelerate(float delta, float thrust, float stability, float maxSpeed, float maxSpeedSqr, Vector2 dirNormalized)
	{
		if (dirNormalized == Vector2.zero) {
#if UNITY_EDITOR
			Debug.LogError("null dir");
#endif
			return;
		}
#if UNITY_EDITOR
		if(Mathf.Abs(dirNormalized.magnitude - 1f) > 0.1f) {
			Debug.LogError("normalize the value! " + dirNormalized + " " + dirNormalized.magnitude);
		}
#endif

		float deltaV = delta * thrust;
		velocity -= stability * velocity.normalized * deltaV;
		velocity += (1 + stability)* dirNormalized * deltaV;

		if(velocity.sqrMagnitude > maxSpeedSqr)
		{
			velocity = velocity.normalized*(Mathf.Max(velocity.magnitude - deltaV, maxSpeed));
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

	public E AddEffect<E>(E effect) 
		where E: TickableEffect
	{
		bool updated = false;
		if (effect.CanBeUpdatedWithSameEffect) {
			var same = effects.Find (e =>  e.IsTheSameEffect(effect));
			if (same != null) {
				same.UpdateBy (effect);
				updated = true;
				return same as E;
			}
		} 

		if (!updated) {
			effect.SetHolder (this);
			effects.Add (effect);
		}
		return effect;
	}

    //threshold >= 0
    private List<Vector2[]> SplitUntilInteriorThreshold(Vector2[] verts, int threshold) {
        List<Vector2[]> parts = new List<Vector2[]>();
        Polygon p = new Polygon(verts);
        if (p.GetInteriorVerticesCount() > threshold) {
            var toTest = p.SplitByConcaveVertex();
			if (toTest.Count > 1) {
				foreach (var item in toTest) {
					parts.AddRange (SplitUntilInteriorThreshold (item, threshold));
				}
			} else {
				Debug.LogError ("could not split");
				parts.AddRange(toTest);
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

	public virtual void Heal(float amount){
		currentHealth = Mathf.Min (currentHealth + amount, fullHealth);
	}

	public virtual void Hit(float dmg)
	{
		#if UNITY_EDITOR
		var wtext = Singleton<worldTextUI>.inst;
		if(wtext.showDmgInEditor){
			Vector3 offset = new Vector3 (new RandomFloat(-5,5).RandomValue, new RandomFloat(-5,5).RandomValue, -1);
			Singleton<worldTextUI>.inst.ShowText (cacheTransform.position + offset, this.GetColor(), dmg.ToString("##.##"), 12); 
		}
		#endif

//      Debug.LogError ("hit " + dmg);
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
		Color shCol = Color.green;
		shCol.a = 0.3f;
		Vector2[] v = Math2d.OffsetVerticesFromCenter (polygon.circulatedVertices, 0.45f);
		shieldGO = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(v, shCol);
		shieldGO.name = "shield";
		shieldGO.cacheTransform.parent = cacheTransform;
		shieldGO.cacheTransform.localPosition = new Vector3 (0, 0, 1);
		shield = new Shield (shieldData, shieldGO);
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

	public void TickGuns(float delta)
	{
		if (deathAnimation != null && deathAnimation.started)
			return;
		
		var d = delta;
		for (int i = 0; i < notLinkedGuns.Count; i++) {
			guns[notLinkedGuns[i]].Tick(d);
		}
		
		if (linkedGuns.Any ()) {
			for (int i = 0; i < linkedGunTick.Count; i++) {
				var linkedGunsIndexList = linkedGuns [i];
				int indexToTick = linkedGunTick [i];
				guns [linkedGunsIndexList[indexToTick]].Tick (d);
			}
		}

		if (!Main.IsNull (target)) {
			if (spawnerGuns.Any ()) {
				for (int i = 0; i < spawnerGuns.Count; i++) {
					guns [spawnerGuns [i]].ShootIfReady ();
				}
			}
		}
	}

    public void Shoot()
	{
		for (int i = 0; i < notLinkedGuns.Count; i++) {
			guns[notLinkedGuns[i]].ShootIfReady();
		}
		
		for (int i = 0; i < linkedGuns.Count; i++) {
			ShootLinkedGunList (i);
		}
	}

	protected void ShootLinkedGunList(int i) {
		var indexList = linkedGuns [i];
		var index = linkedGunTick [i]; 
		var gun = guns [indexList [index]];
		if(indexList.Any() && gun.ReadyToShoot()) {
			gun.ShootIfReady();
			linkedGunTick[i] = linkedGunTick[i] + 1;
			if(linkedGunTick[i] >= indexList.Count)
				linkedGunTick[i] = 0;
		}
	}

	protected void ChangeVertex(int indx, Vector2 v)
	{
		Vector3[] vertx3d = mesh.vertices;
		vertx3d[indx] =  new Vector3(v.x, v.y, 0f);
		mesh.vertices = vertx3d;
		
		polygon.ChangeVertex(indx, v);
	}

	[System.NonSerialized] List<ParticlesInst> particles = new  List<ParticlesInst>();
	//TODO: refactor this somehow, i need a lot of parametes and control over the particles
	public List<ParticleSystem> AddParticles(List<ParticleSystemsData> datas)
	{
		List<ParticlesInst> result = new List<ParticlesInst> ();
		foreach (var setup in datas) {
			if (setup.prefab == null) {
				Debug.LogError ("null particle system");
			} else {
				var inst = Instantiate (setup.prefab) as ParticleSystem;
				ApplyOverrides (inst, setup);
				inst.Play ();

				Math2d.PositionOnParent (inst.transform, setup.place, cacheTransform, true, setup.zOffset);
				result.Add (new ParticlesInst{ system = inst, data = setup });
			}
		}
		particles.AddRange (result); 
		return result.ConvertAll(s => s.system);
	}

	private void ApplyOverrides(ParticleSystem inst, ParticleSystemsData setup){
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
		if (setup.overrideStartColor) {
			inst.SetStartColor(setup.startColor);
		}
	}

	public class ParticlesInst
	{
		public ParticleSystem system;
		public ParticleSystemsData data;
	}

	public void OnBoundsTeleporting(Vector2 newPos) {
		if(teleportWithMe != null) {
			Vector2 delta = newPos - position;
			foreach (var controllable in teleportWithMe) {
				if (!Main.IsNull(controllable)) {
					controllable.position += delta;
				}
			}
		}
		if (destroyOnBoundsTeleport) {
			Kill();
			destructionType = PolygonGameObject.DestructionType.eDisappear;
		}
		ToggleAllDistanceEmitParticles (false);
	}

	public void OnBoundsTeleported() {
		ToggleAllDistanceEmitParticles (true);
	}

	public void ToggleAllDistanceEmitParticles(bool play) {
		for (int i = particles.Count - 1; i >= 0; i--) {
			var item = particles [i];
			if (item == null || item.system == null || item.system.transform == null) {
				particles.RemoveAt(i);
			}
		}

		if (play) {
			foreach (var item in particles) {
				//turns out not only rateover distance
				//if (item != null && item.system != null) { //&& item.system.emission.rateOverDistanceMultiplier > 0) {
					item.system.Play ();
				//}
			}
		} else {
			foreach (var item in particles) {
				//if (item != null && item.system != null) {// && item.system.emission.rateOverDistanceMultiplier > 0) {
					item.system.Pause ();
				//}
			}
		}
	}

	List<ParticleSystemsData> destroyEffects;
	public void AddDestroyAnimationParticles(List<ParticleSystemsData> datas) {
		if (destroyEffects == null) {
			destroyEffects = new List<ParticleSystemsData> ();
		}
		destroyEffects.AddRange(datas);
	}

    public void AddObjectAsFollower(PolygonGameObject obj) {
        obj.ignoreBounds = true;
        if (teleportWithMe == null) {
            teleportWithMe = new List<PolygonGameObject>();
        }
        Main.PutOnFirstNullOrDestroyedPlace(teleportWithMe, obj);
    }

	//called before HandleDestroying
	public List<ParticlesInst> GetEffectsForSplitParts() {
		return particles.FindAll (p => p.data.afterlife && p.data.parentToSplitParts);
	}

	//at the very beginning of destruction proccess
	public virtual void HandleStartDestroying() 
	{
		
	}

    public Action OnDestroying;
	public virtual void HandleDestroy() 
	{
		for (int i = 0; i < particles.Count; i++) {
			var ps = particles [i];
			if (ps.data.afterlife && ps.system != null && ps.system.transform != null ) {
				if (ps.data.parentToSplitParts == false) {
					ps.system.transform.parent = null;
				}
				var main = ps.system.main;
				if (ps.data.stopEmission) {
					ps.system.Stop ();
				}
				float blifetime = main.startLifetimeMultiplier + 2f;
				if (ps.data.inheritVelocity && !ps.data.parentToSplitParts) {
					var verts = PolygonCreator.CreatePerfectPolygonVertices (1, 3);
					Color col = new Color (0, 0, 0, 0);
					var holder = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(verts, col);
					holder.InitPolygonGameObject (new PhysicalData ());
					holder.velocity = velocity;
					holder.SetLayerNum (CollisionLayers.ilayerMisc);
					holder.gameObject.name = "animation holder " + ps.system.name;
					ps.system.transform.parent = holder.cacheTransform;
					Singleton<Main>.inst.AddToDestructor(holder, blifetime + 1f);
				}
				Destroy (ps.system.gameObject, blifetime);
			}
		}

		if (destroyEffects != null) {
			foreach (var item in destroyEffects) {
				var inst  = Instantiate(item.prefab) as ParticleSystem;
				Math2d.PositionOnParent (inst.transform, item.place, cacheTransform, false, -1);
				ApplyOverrides (inst, item);
				inst.Play ();
				Destroy (inst.gameObject, inst.main.duration + inst.main.startLifetimeMultiplier);
			}
		}

		foreach (var item in effects) {
			item.HandleHolderDestroying ();
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

	public void RemoveFollower (PolygonGameObject obj) {
		if (Main.IsNull (obj)) {
			return;
		}
		obj.ignoreBounds = false;
		teleportWithMe.Remove (obj);
	}

    public virtual void SetSpawnParent(PolygonGameObject prnt) { }
}
