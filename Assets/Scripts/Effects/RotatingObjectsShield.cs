using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjectsShield : DurationEffect {
	protected override eType etype { get { return eType.RotatingObjectsShield; } }
    public float angleOffsetDeg = 0;
	Data data;
	float currentAngle = 0;
	IEnumerator routineRotate;
	IEnumerator routineSpawn;
	List<PolygonGameObject> shields;
	float rotationSpeed;
	float partMaxSpeed;
	float partMaxSpeedSqr;
	float force;

    public RotatingObjectsShield(Data data) : base(data) {
        this.data = data;
    }

	public List<PolygonGameObject> AliveShieldObjects() {
		List<PolygonGameObject> alive = new List<PolygonGameObject> ();
        if (shields != null) {
            for (int i = 0; i < shields.Count; i++) {
                if (!Main.IsNull(shields[i])) {
					alive.Add(shields[i]);
                }
            }
        }
		return alive;
    }

    public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		routineSpawn = SpawnShieldObjects ();
		routineRotate = RotateShields ();
		shields = new List<PolygonGameObject> ();
		rotationSpeed = 2f * Mathf.PI * data.asteroidShieldRadius / (360f / Mathf.Abs(data.shieldRotationSpeed));
		SpaceShip holderAsSpaceship = holder as SpaceShip;
		if (data.overrideForce < 0) {
			force = rotationSpeed / 0.5f;
			if (holderAsSpaceship != null) {
				force += holderAsSpaceship.thrust;
			}
		} else {
			force = data.overrideForce;
		}
		float maxSpeed = (holderAsSpaceship != null) ? holderAsSpaceship.maxSpeed : holder.velocity.magnitude;
		partMaxSpeed = data.overrideMaxPartSpeed > 0 ? data.overrideMaxPartSpeed : rotationSpeed + maxSpeed;
		partMaxSpeedSqr = partMaxSpeed * partMaxSpeed;
	}

	public override void Tick (float delta) {
        base.Tick (delta);
        if (!IsFinished()) {
            deltaTime = delta;
            routineRotate.MoveNext();
            routineSpawn.MoveNext();
        }
	}

    public override void OnExpired() {
        DestroyShields();
    }

    public override void HandleHolderDestroying() {
        base.HandleHolderDestroying();
        if (data.killShieldsObjectsOnDeath) {
            DestroyShields();
        }
    }

    private void DestroyShields() {
        if (shields != null) {
            for (int i = 0; i < shields.Count; i++) {
                if (!Main.IsNull(shields[i])) {
                    shields[i].Kill();
					if (data.disableExplosionOnKill) {
						shields [i].deathAnimation = null;
					}
                }
            }
        }
    }

    private PolygonGameObject CreateShieldObj() {
		var shieldObj =  CollisionLayers.SpawnObjectFriendlyToParent(holder, data.spawn);
		if (data.collideWithAsteroids) {
			shieldObj.collisions |= (int)CollisionLayers.eLayer.ASTEROIDS;
		}
		shieldObj.controlledBySomeone = true;
		shieldObj.position = holder.position;
		shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(holder.cacheTransform.position.z + data.zOffset);
		if (data.randomizeRotation) {
			shieldObj.cacheTransform.Rotate (new Vector3 (0, 0, new RandomFloat (0, 360).RandomValue));
		}

		holder.AddObjectAsFollower(shieldObj);
		shieldObj.priorityMultiplier = 0.5f;
		shieldObj.showOffScreen = false;
		if (data.keepObjectsRotation) {
			shieldObj.AddEffect (new KeepRotationEffect(data.keepRotationData));
		}
		if (data.keepObjectsOrientation) {
			shieldObj.AddEffect (new KeepOrientationEffect(data.keepOrientationData, holder));
		}
		Singleton<Main>.inst.Add2Objects (shieldObj);
		return shieldObj;
	}

	float deltaTime;

	private IEnumerator SpawnShieldObjects() 
	{
		yield return null;
		for (int i = 0; i < data.maxShieldsCount; i++) {
			var shieldObj = CreateShieldObj ();
			shields.Add (shieldObj);
		}
		while (true) {
			bool hasEmpty = shields.Exists (a => a == null);
			if(hasEmpty) {
				AIHelper.MyTimer timer = new AIHelper.MyTimer (data.respawnShieldObjDuration, null);
				while (!timer.IsFinished ()) {
					timer.Tick (deltaTime);
					yield return null;
				}
				int indx = shields.FindIndex (a => a == null);
				if (indx >= 0) {
					var shieldObj = CreateShieldObj ();
					shields[indx] = shieldObj;
				}
			}
			yield return null;
		}
	}

	private IEnumerator RotateShields()
	{
		float deltaAngle = 360f / data.maxShieldsCount;
		while (true) {
			currentAngle += data.shieldRotationSpeed * deltaTime;
			float angle = angleOffsetDeg + currentAngle;
			for (int i = 0; i < shields.Count; i++) {
				var item = shields [i];
				if (Main.IsNull (item)) {
					shields [i] = null;
				} else {
					var radAngle = angle * Mathf.Deg2Rad;
					Vector2 targetPos = holder.position + data.asteroidShieldRadius * new Vector2 (Mathf.Cos (radAngle), Mathf.Sin (radAngle));
					Vector2 targetVelocity = holder.velocity + Mathf.Sign (data.shieldRotationSpeed) * rotationSpeed * (-Math2d.MakeRight (targetPos - holder.position) + 0.1f * (holder.position - targetPos)).normalized;
					FollowAim aim = new FollowAim (targetPos, targetVelocity, item.position, item.velocity, force);
					if (aim.forceDir != Vector2.zero) {
						item.Accelerate (deltaTime, force, data.asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
					}
				}
				angle += deltaAngle;
			}
			yield return null;
		}
	}

	[System.Serializable]
	public class Data: IHasDuration, IApplyable {

		public float duration = 30f; 
		public float iduration{get {return duration;} set{duration = value;}}
		public MSpawnDataBase spawn;
		public float shieldRotationSpeed;
		public float asteroidShieldRadius;
		public float respawnShieldObjDuration; 
		public float asteroidsStability;
		public int maxShieldsCount;
		public float overrideMaxPartSpeed = -1;
		public float overrideForce = -1;
		public bool collideWithAsteroids = false;
		public bool randomizeRotation = false;
		public bool killShieldsObjectsOnDeath = true;
		public bool disableExplosionOnKill = true;
        public float zOffset = 1;

		[Header ("keep rotation")]
		public bool keepObjectsRotation = false;
		public KeepRotationEffect.Data keepRotationData;
		[Header ("keep orientation")]
		public bool keepObjectsOrientation = false;
		public KeepOrientationEffect.Data keepOrientationData;
	
		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new RotatingObjectsShield (this);
			picker.AddEffect (effect);
			return effect;
		}
	}
}
