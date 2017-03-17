using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjectsShield : DurationEffect {
	protected override eType etype { get { return eType.RotatingObjectsShield; } }
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

    public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		routineSpawn = SpawnShieldObjects ();
		routineRotate = RotateShields ();
		shields = new List<PolygonGameObject> ();
		rotationSpeed = 2f * Mathf.PI * data.asteroidShieldRadius / (360f / Mathf.Abs(data.shieldRotationSpeed));
		SpaceShip holderAsSpaceship = holder as SpaceShip;
		force = rotationSpeed / 0.5f;
		if (holderAsSpaceship != null) {
			force += holderAsSpaceship.thrust;
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
                }
            }
        }
    }

    private PolygonGameObject CreateShieldObj() {
		var shieldObj = data.spawn.Create(CollisionLayers.GetSpawnedLayer (holder.layerLogic));
		if (data.collideWithAsteroids) {
			shieldObj.collisions |= (int)CollisionLayers.eLayer.ASTEROIDS;
		}
		shieldObj.position = holder.position;
		shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(holder.cacheTransform.position.z + 1f);
		holder.AddObjectAsFollower(shieldObj);
		shieldObj.priorityMultiplier = 0.5f;
		if (data.keepObjectsRotation) {
			shieldObj.AddEffect (new KeepRotationEffect(data.keepRotationData));
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
			float angle = currentAngle;
			for (int i = 0; i < shields.Count; i++) {
				var item = shields [i];
				if (Main.IsNull (item)) {
					shields [i] = null;
				} else {
					var radAngle = angle * Mathf.Deg2Rad;
					Vector2 targetPos = holder.position + data.asteroidShieldRadius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
					Vector2 targetVelocity = holder.velocity + Mathf.Sign(data.shieldRotationSpeed) * rotationSpeed * (-Math2d.MakeRight(targetPos - holder.position) + 0.1f * (holder.position - targetPos)).normalized;
					FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force);
					item.Accelerate(deltaTime, force, data.asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
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
		public bool collideWithAsteroids = false;
		public bool killShieldsObjectsOnDeath = true;

		[Header ("keep rotation")]
		public bool keepObjectsRotation = false;
		public KeepRotationEffect.Data keepRotationData;
	
		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new RotatingObjectsShield (this));
		}
	}
}
