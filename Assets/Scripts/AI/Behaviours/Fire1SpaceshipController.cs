using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire1SpaceshipController : BaseSpaceshipController, IGotTarget {

	//TODO better bound teleport check
	MFireShip1Data data;
	float accuracy = 0f;

    float force;
    float deltaAngle;
    List<SpaceShip> fireballs = new List<SpaceShip>();
    float teleportDistanceSqr = 120f * 120f;
    float comformDistanceMin = 30f;
    float comformDistanceMax = 50f;
    AIHelper.Data tickData = new AIHelper.Data();

	bool shootBeh = false;

	public Fire1SpaceshipController(SpaceShip thisShip, List<PolygonGameObject> objects, MFireShip1Data data) : base(thisShip) {
        this.data = data;
        force = thisShip.thrust;
        deltaAngle = 360f / data.fireballCount;
        //comformDistanceMax = data.fireballData.lifeTime

        comformDistanceMax = data.fireballData.lifeTime * data.fireballData.missleParameters.maxSpeed * 0.8f;
        thisShip.StartCoroutine (LogicShip ());
		thisShip.StartCoroutine (SpawnFireballs ());
		thisShip.StartCoroutine (KeepFireballs ());
		thisShip.StartCoroutine (LogicShoot ());

		var accData = data.accuracy;
		accuracy = accData.startingAccuracy;
		if (accData.isDynamic) {
			thisShip.StartCoroutine (AccuracyChanger (accData));
		}

		thisShip.OnDestroying += HandleDestroying;
    }

    private IEnumerator LogicShip() {
        float checkBehTimeInterval = 0.1f;
        float checkBehTime = 0;
        bool behaviourChosen = false;
        float duration;
        Vector2 newDir;
        while (true) {
            if (!Main.IsNull(target)) {
				if (!shootBeh) {
					behaviourChosen = false;
					checkBehTime -= Time.deltaTime;

					if (checkBehTime <= 0) {
						checkBehTime = checkBehTimeInterval;

						comformDistanceMin = Mathf.Min(target.polygon.R + thisShip.polygon.R + 20f, comformDistanceMax * 0.7f); // TODO on target change
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
            }
            yield return null;
        }
    }

    private IEnumerator SpawnFireballs() {
        yield return null;

        for (int i = 0; i < data.fireballCount; i++) {
            if (i < data.startFireballs) {
                var fireball = CreateFireball();
                fireballs.Add(fireball);
            } else {
                fireballs.Add(null);
            }
        }

        while (true) {
            bool hasEmpty = fireballs.Exists(a => a == null);
			if (hasEmpty && !shootBeh) {
                yield return new WaitForSeconds(data.respawnFireballDuration);
                int indx = fireballs.FindIndex(a => a == null);
                if (indx >= 0) {
                    var obj = CreateFireball();
                    fireballs[indx] = obj;
                }
            }
            yield return null;
        }
    }

    private IEnumerator KeepFireballs() {
        while (true) {
            float angle = 0;
            for (int i = 0; i < fireballs.Count; i++) {
                var item = fireballs[i];
                if (Main.IsNull(item)) {
                    fireballs[i] = null;
                } else {
                    if ((item.position - thisShip.position).sqrMagnitude < teleportDistanceSqr) { //hack for bounds teleport, TODO: teleport it with the ship
                        var radAngle = angle * Mathf.Deg2Rad;
                        Vector2 targetPos = thisShip.position + data.radius * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
                        Vector2 targetVelocity = thisShip.velocity;// + rotationSpeed * (-Math2d.MakeRight(targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
                        FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force);
                        item.Accelerate(Time.deltaTime, force, item.stability, item.maxSpeed, item.maxSpeedSqr, aim.forceDir.normalized);
                    }
                }
                angle += deltaAngle;
            }
            yield return null;
        }
    }

    private IEnumerator LogicShoot() {
        yield return null;
        while (true) {
            if (target != null) {
                bool fireballsFull = !fireballs.Exists(a => Main.IsNull(a));
                if (fireballsFull) {
					shootBeh = true;
					Brake ();
					float time = 1f;
					while (time > 0) {
						turnDirection = target.position - thisShip.position;
						yield return null;
						time -= Time.deltaTime;
					}

                    for (int i = 0; i < fireballs.Count; i++) {
						turnDirection = target.position - thisShip.position;
                        var obj = fireballs[i];
                        if (!Main.IsNull(obj)) {
                            fireballs[i] = null;
                            var controller = new MissileController(obj, data.fireballData.accuracy);
                            obj.SetController(controller);
                            obj.SetTarget(target);
                            float rndAngle = Random.Range(-data.randomizeAimAngle, data.randomizeAimAngle);
                            var dir = thisShip.cacheTransform.right;
                            dir = Math2d.RotateVertexDeg(dir, rndAngle);
                            obj.cacheTransform.right = dir; 
							obj.InitLifetime (data.fireballData.lifeTime); 
                            obj.destroyOnBoundsTeleport = true;
                        }
                        yield return new WaitForSeconds(data.shootInterval);
                    }
					shootBeh = false;
                }
            }
            yield return null;
        }
    }

    private SpaceShip CreateFireball() {
        MRocketGunData rd = data.fireballData;
        SpaceShip fireball = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(rd.vertices, rd.color);
        fireball.position = thisShip.position;
        fireball.gameObject.name = "fireball";
        fireball.InitSpaceShip(rd.physical, rd.missleParameters);
        fireball.damageOnCollision = rd.damageOnCollision;
        fireball.destructionType = PolygonGameObject.DestructionType.eDisappear;
        fireball.SetParticles(rd.particles);
        fireball.SetDestroyAnimationParticles(rd.destructionEffects);
		fireball.SetController (new StaticInputController ());
		fireball.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(thisShip.layer));
        thisShip.AddObjectAsFollower(fireball);
		Singleton<Main>.inst.HandleGunFire (fireball);
        return fireball;
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

	void HandleDestroying() {
		for (int i = 0; i < fireballs.Count; i++) {
			var obj = fireballs[i];
			if (!Main.IsNull(obj)) {
				obj.InitLifetime (5f);
				obj.destroyOnBoundsTeleport = true;
			}
		}
	}

}
