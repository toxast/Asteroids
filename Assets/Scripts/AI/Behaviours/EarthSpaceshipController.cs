using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class EarthSpaceshipController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> objects;

	int maxShieldsCount;
	float force;
	float attackForce;
	float partMaxSpeed;
	float partMaxSpeedSqr;
	float partAttackMaxSpeed;
	float partAttackMaxSpeedSqr;
	float asteroidShieldRadius;
	float shieldRotationSpeed;
	float applyShootingForceDuration;
	List<ParticleSystemsData> asteroidAttackByForceAnimations;
	List<ParticleSystemsData> asteroidGrabByForceAnimations;
	List<PolygonGameObject> shields = new List<PolygonGameObject>(); //can contain null objects
	List<BrokenShieldObj> brokenShields = new List<BrokenShieldObj>(); //can contain null objects
	float asteroidsStability;

    float currentAngle = 0;
	float deltaAngle;
	float rotationSpeed; 
	List<BulletObj> objectShootingWith = new List<BulletObj>(); //can contain null objects
    List<BrokenBulletObj> brokenObjectShootingWith = new List<BrokenBulletObj>();
	bool lastArcIsRight = false;

    class BulletObj{
		public PolygonGameObject obj;
		public float startTime;
		public bool rightArc = true;
	}

    class BrokenBulletObj {
        public PolygonGameObject obj;
        public float startTime;
        public Vector2 accelerateDir;
    }

    class BrokenShieldObj{
		public PolygonGameObject obj;
		public float angleDeg;
	}

	MEarthSpaceshipData data;

	public EarthSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> objects, MEarthSpaceshipData data) : base(thisShip, data.accuracy)
	{
        this.data = data;
		asteroidsStability = data.asteroidsStability;
        asteroidAttackByForceAnimations = data.asteroidAttackByForceAnimations.Clone();
		asteroidGrabByForceAnimations = data.asteroidGrabByForceAnimations.Clone();
        asteroidShieldRadius = data.shieldRadius;
		shieldRotationSpeed = data.shieldRotationSpeed;
		maxShieldsCount = data.elementsCount;
		applyShootingForceDuration = data.applyShootingForceDuration;
		rotationSpeed = 2f * Mathf.PI * asteroidShieldRadius / (360f / shieldRotationSpeed);
		deltaAngle = 360f / maxShieldsCount;
		force = 1.5f * thisShip.thrust  + rotationSpeed * 2f;
		attackForce = data.overrideAttackPartForce > 0 ? data.overrideAttackPartForce : force;
		partMaxSpeed = rotationSpeed + 1.5f * thisShip.maxSpeed;
		partMaxSpeedSqr = partMaxSpeed * partMaxSpeed;
		partAttackMaxSpeed = data.overrideMaxPartAttackSpeed > 0 ? data.overrideMaxPartAttackSpeed : partMaxSpeed;
		partAttackMaxSpeedSqr = partAttackMaxSpeed * partAttackMaxSpeed; 
		this.objects = objects;

		Debug.LogWarning (thisShip.name + " partMaxSpeed:" + partMaxSpeed + " force:" + force + " rotationSpeed:" + rotationSpeed);


		CommonBeh.Data behData = new CommonBeh.Data {
			accuracyChanger = accuracyChanger,
			comformDistanceMax = data.comformDistanceMax,
			comformDistanceMin = data.comformDistanceMin,
			getTickData = GetTickData,
			mainGun = null,
			thisShip = thisShip,
		};
		EvadeTargetBeh evadeBeh = new EvadeTargetBeh(behData, new NoDelayFlag());
		logics.Add(evadeBeh);

		bool useCowardAction = true;
		if (useCowardAction) {
			DelayFlag cowardDelay = new DelayFlag(true, 12, 20);
			CowardBeh cowardBeh = new CowardBeh(behData, cowardDelay);
			logics.Add(cowardBeh);
		}

//		bool evadeBullets = false;
//		if (evadeBullets) {
//			DelayFlag checkBulletsDelay = new DelayFlag(false, 1 , 4);
//			EvadeBulletsBeh evadeBulletsBeh = new EvadeBulletsBeh(behData, bullets, checkBulletsDelay);
//			logics.Add(evadeBulletsBeh);
//		}
		if (data.stopWhileShooting) {
			var rotateOntarget = new ShootWhileRotateBeh (behData, new DelayFlag (false, data.shootPause, data.shootPause), AimFunction, data.shootDuration, data.applyShootingForceDuration);
			logics.Add (rotateOntarget);
		} else {
			shooting = true;
			shootDelay = new ShootDelayBeh (behData, data.shootDuration, data.shootPause);
			shootDelay.OnShootChange += delegate(bool shooting) {
				this.shooting = shooting;
			};
			if (shootDelay.IsReadyToAct ()) {
				shootDelay.Start ();
			}
		}

		var turnBeh = new TurnDontChangeShoot (behData, new NoDelayFlag ());
		turnBeh.SetPassiveTickOthers (true);
		logics.Add (turnBeh);

		FlyAroundBeh flyAround = new FlyAroundBeh(behData);
		logics.Add(flyAround);

		AssignCurrentBeh(null);

		thisShip.StartCoroutine (SpawnShieldObjects ());
		thisShip.StartCoroutine (RotateShields ());
		thisShip.StartCoroutine (LogicShoot ());
		thisShip.StartCoroutine (AimShootingObjects ());

        if (data.collectBrokenObjects) {
            thisShip.StartCoroutine(CollectBrokenObjects());
            thisShip.StartCoroutine(RotateBrokenShields());
            thisShip.StartCoroutine(ShootBrokenObjects());
            thisShip.StartCoroutine(AimBrokenObjects());
        }
	}

	ShootDelayBeh shootDelay = null;
	public override void Tick (float delta) {
		base.Tick (delta);
		if (shootDelay != null) {
			shootDelay.Tick (delta);
		}
	}

	Vector2 AimFunction(){
		return target.position - thisShip.position;
	}

	private IEnumerator SpawnShieldObjects() 
	{
		yield return null;

		for (int i = 0; i < maxShieldsCount; i++) {
			var shieldObj = CreateShieldObj ();
			shields.Add (shieldObj);
		}

		while (true) {
			bool hasEmpty = shields.Exists (a => a == null);
			if(hasEmpty) {
				yield return new WaitForSeconds (data.respawnShieldObjDuration);
				int indx = shields.FindIndex (a => a == null);
				if (indx >= 0) {
					var shieldObj = CreateShieldObj ();
					shields[indx] = shieldObj;
				}
			}
			yield return null;
		}
	}

   

    private PolygonGameObject CreateShieldObj() {
		var shieldObj = CollisionLayers.SpawnObjectFriendlyToParent (thisShip, data.spawnObj);
		shieldObj.position = thisShip.position;
		shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(thisShip.cacheTransform.position.z + 1f);
		shieldObj.controlledBySomeone = true;
		//Debug.LogError ("shieldObj massL" + shieldObj.mass);
        thisShip.AddObjectAsFollower(shieldObj);
		Singleton<Main>.inst.Add2Objects (shieldObj);
		return shieldObj;
	}

	private IEnumerator RotateShields()
	{
		while (true) {
			currentAngle += shieldRotationSpeed * Time.deltaTime;
			float angle = currentAngle;
			for (int i = 0; i < shields.Count; i++) {
				var item = shields [i];
				if (Main.IsNull (item)) {
					shields [i] = null;
				} else {
                    var radAngle = angle * Mathf.Deg2Rad;
                    Vector2 targetPos = thisShip.position + asteroidShieldRadius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                    Vector2 targetVelocity = thisShip.velocity + rotationSpeed * (-Math2d.MakeRight(targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
					FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force, partMaxSpeed);
					item.Accelerate(Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
				}
				angle += deltaAngle;
			}
			yield return null;
		}
	}

    private IEnumerator RotateBrokenShields() {
        while (true) {
            float deltaAngle = -shieldRotationSpeed * Time.deltaTime;
            for (int i = 0; i < brokenShields.Count; i++) {
                var item = brokenShields[i];
                if (item != null) {
                    if (Main.IsNull(item.obj)) {
                        brokenShields[i] = null;
                    } else {
                        item.angleDeg += deltaAngle;
                        var bObj = item.obj;
                        var radAngle = item.angleDeg * Mathf.Deg2Rad;
						Vector2 targetPos = thisShip.position + data.brokenShieldRadius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                        Vector2 targetVelocity = thisShip.velocity - rotationSpeed * (-Math2d.MakeRight(targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
						FollowAim aim = new FollowAim(targetPos, targetVelocity, bObj.position, bObj.velocity, force, partMaxSpeed);
						bObj.Accelerate(Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator LogicShoot() {
		yield return new WaitForSeconds (data.shootInterval/2f);
		while (true) {
			if (!Main.IsNull(target) && shooting) {
				ShootPart ();
				yield return new WaitForSeconds (data.shootInterval);
			} else {
				yield return new WaitForSeconds (0.2f);
			}
		}
	}

	private void ShootPart(){
		var notNull = shields.FindAll (a => !Main.IsNull (a));
		if (notNull.Count > 0) {
			int indx = -1;
			if (data.shootClosestByVelocity) {
				Vector2 aimdir = ((target.position + target.velocity) - thisShip.position).normalized;
				float maxDot = float.MinValue;
				for (int i = 0; i < notNull.Count; i++) {
					var dot = Vector2.Dot (aimdir, notNull [i].velocity);
					if (dot > 0 && dot > maxDot) {
						maxDot = dot;
						indx = i;
					}
				}
			} 
			if (indx < 0) {
				indx = Random.Range (0, notNull.Count);
			}
			PolygonGameObject obj = notNull [indx];

			for (int i = 0; i < shields.Count; i++) {
				if(shields[i] == obj) {
					shields [i] = null;
					break;
				}
			}

			asteroidAttackByForceAnimations.ForEach(e => { e.overrideSize = 2 * obj.polygon.R; e.overrideDuration = applyShootingForceDuration; });
			obj.AddParticles (asteroidAttackByForceAnimations);
			obj.destroyOnBoundsTeleport = true;
			obj.controlledBySomeone = true;
			thisShip.RemoveFollower (obj);
			var bulletObj = new BulletObj{ obj = obj, startTime = Time.time };
			if (data.shootByArc) {
				lastArcIsRight = !lastArcIsRight;
				bulletObj.rightArc = lastArcIsRight;
			}
			Main.PutOnFirstNullPlace (objectShootingWith, bulletObj); ;
		}
	}

    private IEnumerator ShootBrokenObjects() {
        while (true) {
            if (target != null) {
                var notNull = brokenShields.FindAll(a => a!= null && !Main.IsNull(a.obj));
				if (notNull.Count >= data.attackWithBrokenWhenCount) {
					//Debug.LogWarning("broken obj: " + notNull.Count + " shoot " + data.attackWithBrokenWhenCount);
					for (int i = 0; i < data.attackWithBrokenCount; i++) {
                        notNull = brokenShields.FindAll(a => a != null && !Main.IsNull(a.obj));
                        if(notNull.Count == 0) {
                            break;
                        }
                        int rnd = Random.Range(0, notNull.Count);
                        var bobj = notNull[rnd];
                        var obj = bobj.obj;
                        brokenShields.Remove(bobj);

						asteroidAttackByForceAnimations.ForEach(e => { e.overrideSize = 2 * obj.polygon.R; e.overrideDuration = data.applyForceBrokenObj; });
                        obj.AddParticles(asteroidAttackByForceAnimations);
                        obj.controlledBySomeone = true;
						thisShip.RemoveFollower (obj);
						//AimSystem aim = new AimSystem(target.position, target.velocity, obj.position, partAttackMaxSpeed * 0.8f);
						//Vector2 dir = (aim.directionDist * partAttackMaxSpeed * 0.8f - obj.velocity * 0.15f).normalized;// , randomAngle);
						Main.PutOnFirstNullPlace(brokenObjectShootingWith, new BrokenBulletObj { obj = obj, startTime = Time.time, accelerateDir = Vector2.one }); ;
                    }
                }
            } 
			yield return new WaitForSeconds(data.minDeltaShootBrokenObjects);
        }
    }

    private IEnumerator AimShootingObjects() 
	{
		while (true) {
			if (target != null) {
				for (int i = 0; i < objectShootingWith.Count; i++) {
					var bulletObj = objectShootingWith [i];
					if (bulletObj != null) {
						if (Main.IsNull (bulletObj.obj)) {
							objectShootingWith [i] = null;
						} else {
							float timeLeft = applyShootingForceDuration - (Time.time - bulletObj.startTime);
							if (timeLeft < 0) {
								objectShootingWith [i] = null;
							} else {
								var item = bulletObj.obj;
								SuicideAim aim = new SuicideAim (target.position, target.velocity, item.position, item.velocity, 300f, 1f);
								var aimDir = aim.direction.normalized;
								if (data.shootByArc && aim.time > 2f && timeLeft > applyShootingForceDuration * 0.4f) {
									aimDir = Math2d.RotateVertexDeg (aimDir, 30 * (bulletObj.rightArc ? 1 : -1));
								}
								item.Accelerate (Time.deltaTime, attackForce, asteroidsStability, partAttackMaxSpeed, partAttackMaxSpeedSqr, aimDir); 
							}
						}
					} 
				}
			}
			yield return null;
		}
	}


    private IEnumerator AimBrokenObjects() {
        while (true) {
            if (target != null) {
                for (int i = 0; i < brokenObjectShootingWith.Count; i++) {
                    var bulletObj = brokenObjectShootingWith[i];
                    if (bulletObj != null) {
                        if (Main.IsNull(bulletObj.obj)) {
                            brokenObjectShootingWith[i] = null;
                        } else {
							float timeLeft = data.applyForceBrokenObj - (Time.time - bulletObj.startTime);
							if (timeLeft < 0) {
                                brokenObjectShootingWith[i] = null;
                            } else {
								var item = bulletObj.obj;
								SuicideAim aim = new SuicideAim (target.position, target.velocity, item.position, item.velocity, 300f, 0);
								if (aim.direction != Vector2.zero) {
									var aimDir = aim.direction.normalized;
									item.Accelerate (Time.deltaTime, attackForce, asteroidsStability, partAttackMaxSpeed, partAttackMaxSpeedSqr, aimDir); 
								}
                            }
                        }
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator CollectBrokenObjects() 
	{
		float checkDistSqr = (2 * asteroidShieldRadius);
		checkDistSqr = checkDistSqr * checkDistSqr;

		while (true) {

			for (int i = 0; i < objects.Count; i++) {
				var obj = objects [i];
				if (Main.IsNull (obj) || obj.logicNum != CollisionLayers.ilayerAsteroids || obj.controlledBySomeone) {
					continue;
				}

				if (obj.mass > data.collectMassThreshold) {
					continue;
				}

				if ((obj.position - thisShip.position).sqrMagnitude > checkDistSqr) {
					continue;
				}

				//Debug.LogWarning ("add " + obj.name);
				var angle = Math2d.AngleRad (new Vector2 (1, 0), (obj.position - thisShip.position).normalized) * Mathf.Rad2Deg;
				var newBroken = new BrokenShieldObj{ obj = obj, angleDeg = angle};
				obj.SetLayerNum (obj.logicNum, CollisionLayers.GetCollisionLayer (thisShip.layerLogic));
				obj.controlledBySomeone = true;
                thisShip.AddObjectAsFollower(obj);
                asteroidGrabByForceAnimations.ForEach(e => e.overrideSize = 2 * obj.polygon.R);
                obj.AddParticles (asteroidGrabByForceAnimations);
				Main.PutOnFirstNullPlace (brokenShields,  newBroken);
			}
			yield return new WaitForSeconds(data.collectBrokenObjectsInterval);
		}
	}

	public class ShootDelayBeh : DelayedActionBeh{
		float duration;
		float pause;
		public ShootDelayBeh(CommonBeh.Data data, float duration, float delay ) : base(data, new NoDelayFlag()){
			this.duration = duration;
			this.pause = delay;
		}

		protected override IEnumerator Action () {
			while (true) {
				var wait = WaitForSeconds (duration);
				while (wait.MoveNext ()) {
					FireShootChange (true);
					yield return true;
				}
				FireShootChange (false);
				wait = WaitForSeconds (pause);
				while (wait.MoveNext ()) yield return true;
				yield return true;
			}
		}
	}
}

