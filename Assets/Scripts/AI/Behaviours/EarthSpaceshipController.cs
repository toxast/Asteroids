using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


//TODO custom collision layer for asteroids, which will leave them not primary target
public class EarthSpaceshipController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> objects;
	float comformDistanceMin, comformDistanceMax;
	float accuracy = 0f;

	AIHelper.Data tickData = new AIHelper.Data();

	int maxShieldsCount;
	float force;
	float partMaxSpeed;
	float partMaxSpeedSqr;
	float asteroidShieldRadius;
	float shieldRotationSpeed;
	float applyShootingForceDuration;
	List<ParticleSystemsData> asteroidAttackByForceAnimations;
	List<ParticleSystemsData> asteroidGrabByForceAnimations;
	List<PolygonGameObject> shields = new List<PolygonGameObject>(); //can contain null objects
	List<BrokenShieldObj> brokenShields = new List<BrokenShieldObj>(); //can contain null objects
	List<Vector2> targetPositions = new List<Vector2>();
	MAsteroidData ast;
	float asteroidsStability;

    float currentAngle = 0;
	float deltaAngle;
	float rotationSpeed; 
	List<BulletObj> objectShootingWith = new List<BulletObj>(); //can contain null objects
    List<BrokenBulletObj> brokenObjectShootingWith = new List<BrokenBulletObj>();

    class BulletObj{
		public PolygonGameObject obj;
		public float startTime;
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

    float teleportDistanceSqr = 120f * 120f; 
	MEarthSpaceshipData data;

	public EarthSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> objects, MEarthSpaceshipData data) : base(thisShip)
	{
        if (thisShip.teleportWithMe == null) {
            thisShip.teleportWithMe = new List<PolygonGameObject>();
        }
        this.data = data;
		asteroidsStability = data.asteroidsStability;
        asteroidAttackByForceAnimations = data.asteroidAttackByForceAnimations;
		asteroidGrabByForceAnimations = data.asteroidGrabByForceAnimations;
		asteroidShieldRadius = data.shieldRadius;
		shieldRotationSpeed = data.shieldRotationSpeed;
		maxShieldsCount = data.elementsCount;
		applyShootingForceDuration = data.applyShootingForceDuration;
		for (int i = 0; i < maxShieldsCount; i++) {
			targetPositions.Add (Vector2.zero);
		}
		rotationSpeed = 2f * Mathf.PI * asteroidShieldRadius / (360f / shieldRotationSpeed);
		ast = data.asteroidData;
		deltaAngle = 360f / maxShieldsCount;
		force = thisShip.thrust  + rotationSpeed / 0.5f;
		partMaxSpeed = data.overrideMaxPartSpeed > 0 ? data.overrideMaxPartSpeed : rotationSpeed + thisShip.maxSpeed;
		partMaxSpeedSqr = partMaxSpeed * partMaxSpeed;
		this.objects = objects;

		comformDistanceMax = 50;
		comformDistanceMin = 30;

		thisShip.StartCoroutine (LogicShip ());
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

        var accData = data.accuracy;
		accuracy = accData.startingAccuracy;
		if (accData.isDynamic) {
			thisShip.StartCoroutine (AccuracyChanger (accData));
		}
		
		Debug.LogWarning ("EarthShip:  part max speed: " + partMaxSpeed + " rotationSpeed " + rotationSpeed + " force: " + force);
	}

    private IEnumerator LogicShip() {
        float checkBehTimeInterval = 0.1f;
        float checkBehTime = 0;
        bool behaviourChosen = false;
        float duration;
        Vector2 newDir;
        while (true) {
            if (!Main.IsNull(target)) {
                behaviourChosen = false;
                checkBehTime -= Time.deltaTime;

                if (checkBehTime <= 0) {
                    checkBehTime = checkBehTimeInterval;

                    comformDistanceMin = Mathf.Min(target.polygon.R + thisShip.polygon.R, comformDistanceMax * 0.7f); // TODO on target change
                                                                                                                      //Debug.LogWarning(comformDistanceMin + " " + comformDistanceMax);
                    tickData.Refresh(thisShip, target);

                    if (!behaviourChosen) {
                        behaviourChosen = true;
                        if (tickData.distEdge2Edge > comformDistanceMax || tickData.distEdge2Edge < comformDistanceMin) {
                            AIHelper.OutOfComformTurn(thisShip, comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
                            yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
                        } else {
                            AIHelper.ComfortTurn(comformDistanceMax, comformDistanceMin, tickData, out duration, out newDir);
                            yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
                        }
                    }
                }
            }
            yield return null;
        }
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
		var shieldObj = ObjectsCreator.CreateAsteroid (ast);
		shieldObj.SetCollisionLayerNum (CollisionLayers.GetSpawnedLayer (thisShip.layer));
		shieldObj.position = thisShip.position;
		shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(thisShip.cacheTransform.position.z + 1f);
		shieldObj.capturedByEarthSpaceship = true;
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
                    if ((item.position - thisShip.position).sqrMagnitude < teleportDistanceSqr) { //hack for bounds teleport
                        var radAngle = angle * Mathf.Deg2Rad;
                        Vector2 targetPos = thisShip.position + asteroidShieldRadius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                        Vector2 targetVelocity = thisShip.velocity + rotationSpeed * (-Math2d.MakeRight(targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
                        targetPositions[i] = targetPos;
                        FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force);
						item.Accelerate(Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
                    }
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
                        if ((item.obj.position - thisShip.position).sqrMagnitude < teleportDistanceSqr) { //hack for bounds teleport
                            var bObj = item.obj;
                            var radAngle = item.angleDeg * Mathf.Deg2Rad;
							Vector2 targetPos = thisShip.position + data.brokenShieldRadius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                            Vector2 targetVelocity = thisShip.velocity - rotationSpeed * (-Math2d.MakeRight(targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
                            FollowAim aim = new FollowAim(targetPos, targetVelocity, bObj.position, bObj.velocity, force);
							bObj.Accelerate(Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.forceDir.normalized);
                        }
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator LogicShoot() {
		yield return new WaitForSeconds (data.shootInterval/2f);
		while (true) {
			if (target != null) {
				var notNull = shields.FindAll (a => !Main.IsNull (a));
				if (notNull.Count > 0) {
					int rnd = Random.Range (0, notNull.Count);
					var obj = notNull [rnd];
					for (int i = 0; i < shields.Count; i++) {
						if(shields[i] == obj) {
							shields [i] = null;
							break;
						}
					}
					foreach (var ps in asteroidAttackByForceAnimations) {
						var pmain = ps.prefab.main;
						pmain.startSizeMultiplier = 2 * obj.polygon.R;
						pmain.duration = applyShootingForceDuration;
					}
					obj.SetParticles (asteroidAttackByForceAnimations);
					obj.destroyOnBoundsTeleport = true;
					obj.capturedByEarthSpaceship = true;
					Main.PutOnFirstNullPlace (objectShootingWith, new BulletObj{ obj = obj, startTime = Time.time }); ;
				}
				yield return new WaitForSeconds (data.shootInterval);
			} else {
				yield return new WaitForSeconds (1f);
			}
		}
	}

    private IEnumerator ShootBrokenObjects() {
        while (true) {
            if (target != null) {
                var notNull = brokenShields.FindAll(a => a!= null && !Main.IsNull(a.obj));
				if (notNull.Count >= data.attackWithBrokenWhenCount) {
					Debug.LogWarning("broken obj: " + notNull.Count + " shoot " + data.attackWithBrokenWhenCount);
					for (int i = 0; i < data.attackWithBrokenCount; i++) {
                        notNull = brokenShields.FindAll(a => a != null && !Main.IsNull(a.obj));
                        if(notNull.Count == 0) {
                            break;
                        }
                        int rnd = Random.Range(0, notNull.Count);
                        var bobj = notNull[rnd];
                        var obj = bobj.obj;
                        brokenShields.Remove(bobj);
                        foreach (var ps in asteroidAttackByForceAnimations) {
                            var pmain = ps.prefab.main;
                            pmain.startSizeMultiplier = 2 * obj.polygon.R;
                            pmain.duration = applyShootingForceDuration;
                        }
                        obj.SetParticles(asteroidAttackByForceAnimations);
                        obj.capturedByEarthSpaceship = true;
                        AimSystem aim = new AimSystem(target.position, target.velocity, obj.position, partMaxSpeed * 0.8f);
						//float angleRange =  Mathf.Min(80f, (attackWithBrokenWhenCount - 1) * 3f);
						//float randomAngle = Random.Range(-angleRange, angleRange) * Mathf.Deg2Rad;
						Vector2 dir = (aim.directionDist * partMaxSpeed * 0.8f - obj.velocity * 0.15f).normalized;// , randomAngle);
						Main.PutOnFirstNullPlace(brokenObjectShootingWith, new BrokenBulletObj { obj = obj, startTime = Time.time, accelerateDir = dir }); ;
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
							if (Time.time - bulletObj.startTime > applyShootingForceDuration) {
								objectShootingWith [i] = null;
							} else {
								var item = bulletObj.obj;
								SuicideAim aim = new SuicideAim (target.position, target.velocity, item.position, item.velocity, 300f, 1f);
								item.Accelerate (Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, aim.direction.normalized); 
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
                            if (Time.time - bulletObj.startTime > applyShootingForceDuration) {
                                brokenObjectShootingWith[i] = null;
                            } else {
                                var item = bulletObj.obj;
								item.Accelerate(Time.deltaTime, force, asteroidsStability, partMaxSpeed, partMaxSpeedSqr, bulletObj.accelerateDir.normalized);
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
		float checkDensity = ast.commonData.density * 1.5f;
		float checkRadius = ast.size.max * 1.5f;

		while (true) {

			for (int i = 0; i < objects.Count; i++) {
				var obj = objects [i];
				if (Main.IsNull (obj) || obj.layerNum != CollisionLayers.ilayerAsteroids || obj.capturedByEarthSpaceship) {
					continue;
				}

				if (obj.density > checkDensity || obj.polygon.R > checkRadius) {
					continue;
				}

				if ((obj.position - thisShip.position).sqrMagnitude > checkDistSqr) {
					continue;
				}

				//capturedByEarthSpaceship check does that
//				if (shields.Contains (obj) || brokenShields.Exists(s => s!= null && s.obj == obj) || objectShootingWith.Exists(b => b!= null && b.obj == obj)) {
//					continue;
//				}

				Debug.LogWarning ("add " + obj.name);
				var angle = Math2d.AngleRad (new Vector2 (1, 0), (obj.position - thisShip.position).normalized) * Mathf.Rad2Deg;
				var newBroken = new BrokenShieldObj{ obj = obj, angleDeg = angle};
				obj.SetCollisionLayerNum (CollisionLayers.GetSpawnedLayer (thisShip.layer));
				obj.capturedByEarthSpaceship = true;
                thisShip.AddObjectAsFollower(obj);
                foreach (var ps in asteroidGrabByForceAnimations) {
					var pmain = ps.prefab.main;
					pmain.startSizeMultiplier = 2 * obj.polygon.R;
				}
				obj.SetParticles (asteroidGrabByForceAnimations);
				Main.PutOnFirstNullPlace (brokenShields,  newBroken);
			}

			yield return new WaitForSeconds(data.collectBrokenObjectsInterval);
		}
	}

	private IEnumerator AccuracyChanger(AccuracyData data)
	{
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds(dtime);
		}
	}

}

