using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSuperSaw : SawEnemy {
    MSawBossData bossData;
    RotatingObjectsShield generatorsEffect;
    bool swapnedAnyGenerator = false;
    List<SpawnAxis> spawnAxises = new List<SpawnAxis>();
    public override void InitSawEnemy(MSawData data) {
        base.InitSawEnemy(data);
        bossData = data as MSawBossData;
        generatorsEffect = bossData.generatorsShieldData.Apply(this) as RotatingObjectsShield;
        this.SetShield(new ShieldData(99999, 9999, 0, 0));
		this.SetChargeRestriction (DoChargeChecker);
		this.AddDestroyAnimationParticles (new List<ParticleSystemsData>{ bossData.deathPS });
		this.priority = ePriorityLevel.LOW;
		Debug.LogError ("TODO: rage animation");
    }

	static float rageHealth = 0.4f;

	List<LazerBetweenTwoOjects> lazers = new List<LazerBetweenTwoOjects>();

	bool spawn = false;
    public override void Tick(float delta) {
        base.Tick(delta);
        generatorsEffect.angleOffsetDeg = cacheTransform.eulerAngles.z;
		if (spawn) {
			foreach (var s in spawnAxises) {
				s.Tick (delta);
			}
		}
		foreach (var item in lazers) {
			item.Reposition ();
		}

		if (firstDelay > 0) {
			firstDelay -= delta;
		}
    }

	protected override float ThrustMultiplier () {
		if (GetLeftHealthPersentage () > rageHealth) {
			return base.ThrustMultiplier ();
		} else {
			return base.ThrustMultiplier () * 1.2f;
		}
	}

	float firstDelay = -1;
    bool DoChargeChecker() {
		var aliveGenerators = generatorsEffect.AliveShieldObjects ();
		if (!swapnedAnyGenerator && aliveGenerators.Count > 0) {
            swapnedAnyGenerator = true;
			foreach (var item in aliveGenerators) {
				if (!item.isBossObject) { //if forgot to make it boss object. or didnt want to
					Singleton<Main>.inst.DisplayBossHealth (item);
				}
				lazers.Add (new LazerBetweenTwoOjects (item, this));
			}
        }
        if (!swapnedAnyGenerator) {
            return false;
        }
		bool anyAlive = aliveGenerators.Count > 0;

        if (!anyAlive && shield != null) {
            shield.DestroyShield();
            shield = null;
            StartChargeBeh();
        }

		return !anyAlive && firstDelay <= 0;
    }

    void StartChargeBeh() {
		this.priority = ePriorityLevel.NORMAL;

		this.AddParticles (new List<ParticleSystemsData>{ bossData.eyePS, bossData.eyeFlamePS });
		firstDelay = 4f;

		spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 60, bossData.spawnInterval));
		spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 180, bossData.spawnInterval));
		spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 300, bossData.spawnInterval));
		foreach (var item in spawnAxises) {
			item.OnSpawned += HandleSpawnedObj;
		}
    }

	List<PolygonGameObject> spawned = new List<PolygonGameObject>();
	void HandleSpawnedObj(PolygonGameObject obj){
		Main.PutOnFirstNullPlace (spawned, obj);
	}

	public override void HandleStartDestroying ()	{
		base.HandleStartDestroying ();
		foreach (var item in spawned) {
			if (!Main.IsNull (item)) {
				item.Kill ();
			}
		}
	}

	protected override IEnumerator Slow ()
	{
		float duration = bossData.slowingDuration;
		bool slowing = true;
		while (duration > 0 && slowing) {
			duration -= Time.deltaTime;
			slowing = SlowVelocity (Time.deltaTime, velocityslowingRate);
			yield return null;
		}

		spawn = true;
		duration = bossData.spawnDuration;
		while (duration > 0) {
			duration -= Time.deltaTime;
			yield return null;
		}
		spawn = false;

		yield return base.Slow ();
	}



	private class LazerMesh{
		//float lazerLen = 100;
		GameObject lazer;
		Renderer lazerRenderer;
		Mesh lazerMesh;
		protected Transform lTransform;
		public LazerMesh(){
			lazer = PolygonCreator.CreateLazerGO(Color.magenta);
			lazerRenderer = lazer.GetComponent<Renderer> ();
			lTransform = lazer.transform;
			lazerMesh = lazer.GetComponent<MeshFilter>().mesh;
			PolygonCreator.ChangeLazerMesh (lazerMesh, 1, 4);
			lazerRenderer.material.SetFloat("_Alpha", 1);
			lazerRenderer.material.SetFloat ("_HitCutout", 0);
			lazerRenderer.material.SetFloat ("_Speed", 0.5f);
		}

		public void Position(Vector2 begin, Vector2 endpoint){
			if (lazer != null && begin != endpoint) {
				float angle = Math2d.GetRotationDg(endpoint - begin);
				lTransform.position = new Vector3(begin.x, begin.y, lTransform.position.z);
				lTransform.rotation = Quaternion.Euler(0, 0, angle);
				var dist = (endpoint - begin).magnitude;
				PolygonCreator.ChangeLazerMesh (lazerMesh, dist, 3);
				lazerRenderer.material.SetFloat ("_HitCutout",  1);
			}
		}

		public void Destroy(){
			if (lazer != null) {
				GameObject.Destroy (lazer);
				lazer = null;
			}
		}
	}

	private class LazerBetweenTwoOjects : LazerMesh{
		PolygonGameObject obj1;
		PolygonGameObject obj2;
		public LazerBetweenTwoOjects(PolygonGameObject obj1, PolygonGameObject obj2):base(){
			this.obj1 = obj1;
			this.obj2 = obj2;
			lTransform.position = lTransform.position.SetZ(Mathf.Max(obj1.cacheTransform.position.z, obj2.cacheTransform.position.z) + 1);
			Reposition();
			obj1.OnDestroying += () => Destroy();
			obj2.OnDestroying += () => Destroy();
		}

		public void Reposition(){
			if (Main.IsNull (obj1) || Main.IsNull (obj2)) {
				return;
			}
			Position (obj1.position, obj2.position);
		}
	}


    public class SpawnAxis {
        public PolygonGameObject holder;
        public MSpawnDataBase spawn;
        float angleOffset = 0; //deg angle
        public float interval = 3;
		float rotationThreshold = 40f;
		public event Action<PolygonGameObject> OnSpawned;
		//float velocityMultiplier = 1.2f;

		float length;
        float timeUntilSpawn = 0;

		public SpawnAxis(PolygonGameObject holder, MSpawnDataBase spawn, float angleOffset, float spawnInterval) {
            this.holder = holder;
            this.spawn = spawn;
            this.angleOffset = angleOffset;
			this.interval = spawnInterval;
            length = holder.polygon.R;
        }

		float prevDot = -1;
        public void Tick(float delta) {
            timeUntilSpawn -= delta;
			if (timeUntilSpawn < 0 && !Main.IsNull(holder.target) && Mathf.Abs(holder.rotation) > rotationThreshold) { // 
               var basepos = holder.position;
				var edgePos = basepos + Math2d.RotateVertexDeg(new Vector2(length, 0), angleOffset + holder.cacheTransform.rotation.eulerAngles.z); //fix
				var offset =  0.6f * (edgePos - basepos);
				var pivot = basepos + offset;
				var dirRotation = Math2d.MakeRight(offset) * Mathf.Sign(holder.rotation);
				dirRotation = dirRotation.normalized;
				Debug.DrawLine (pivot, pivot + dirRotation * 20f, Color.white);
				Debug.DrawLine (basepos, edgePos, Color.red);
				var dot = Vector2.Dot (dirRotation, (holder.target.position - pivot).normalized);
				if (prevDot > 0.9f && dot > 0.9f && dot < prevDot) {
					prevDot = -1;
                    timeUntilSpawn = interval;
                    Spawn(0.7f, basepos, edgePos, dirRotation);
					Spawn(0.5f, basepos, edgePos, dirRotation);
					if (holder.GetLeftHealthPersentage () < rageHealth) {
						Spawn(0.6f, basepos, edgePos, dirRotation);
					}
                }
				prevDot = dot;
            }
        }

        void Spawn(float a, Vector2 basepos, Vector2 edgePos, Vector2 dirNorm) { //a c [0, 1]
            var pos = basepos + a * (edgePos - basepos);
            float velocity = (a * length) * Mathf.Abs(holder.rotation) * (Mathf.PI / 180f);
			var shieldObj =  CollisionLayers.SpawnObjectFriendlyToParent (holder, spawn);
            shieldObj.position = pos;
            shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(holder.cacheTransform.position.z + 1);
			shieldObj.velocity = dirNorm * velocity + holder.velocity;
			Singleton<Main>.inst.Add2Objects (shieldObj);
			OnSpawned (shieldObj);
        }
    }
}
