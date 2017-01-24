using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire1SpaceshipController : BaseSpaceshipController, IGotTarget {

    public class FData {
        public int startFireballs = 2;
        public int fireballCount = 3;
        public float respawnFireballDuration = 2;
        public float radius = 15f;
        public float shootInterval = 0.5f;
        public MRocketGunData fireballData;
    }

    FData data;

    float force;
    float deltaAngle;
    List<SpaceShip> fireballs = new List<SpaceShip>();
    float teleportDistanceSqr = 120f * 120f;
    float comformDistanceMin = 30f;
    float comformDistanceMax = 50f;
    AIHelper.Data tickData = new AIHelper.Data();

    public Fire1SpaceshipController(SpaceShip thisShip, List<PolygonGameObject> objects, FData data) : base(thisShip) {
        this.data = data;
        force = thisShip.thrust;
        deltaAngle = 360f / data.fireballCount;
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
            if (hasEmpty) {
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
                        item.Accelerate(Time.deltaTime, force, 0.5f, item.maxSpeed, item.maxSpeedSqr, aim.forceDir.normalized);
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
                    for (int i = 0; i < fireballs.Count; i++) {
                        var obj = fireballs[i];
                        if (!Main.IsNull(obj)) {
                            fireballs[i] = null;
                            var controller = new MissileController(obj, data.fireballData.missleParameters.maxSpeed, data.fireballData.accuracy);
                            obj.velocity = Vector2.zero;
                            obj.SetController(controller);
                            obj.SetTarget(target);
                            obj.destroyOnBoundsTeleport = true;
                        }
                        yield return new WaitForSeconds(data.shootInterval);
                    }
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
        fireball.destroyOnBoundsTeleport = true;
        fireball.destructionType = PolygonGameObject.DestructionType.eDisappear;
        fireball.SetParticles(rd.particles);
        fireball.SetDestroyAnimationParticles(rd.destructionEffects);
        

        return fireball;
    }
}
