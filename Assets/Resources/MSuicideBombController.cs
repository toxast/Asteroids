using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MSuicideBombController : BaseSpaceshipController {
    AIHelper.Data tickData = new AIHelper.Data();

    float chargeDuration;
    float chargingDist;
    float accuracy;
    float explosionRange = 30f;
    List<PolygonGameObject> bullets;
    float distToCheck;

    public MSuicideBombController(SpaceShip thisShip, List<PolygonGameObject> bullets, AccuracyData accData) : base(thisShip) {
        if (thisShip.deathAnimation == null) {
            Debug.LogError("MSuicideBombController has to have explosion death");
            explosionRange = 30f;
        } else {
            explosionRange = thisShip.deathAnimation.GetFinalExplosionRadius();
        }
        distToCheck = explosionRange * 0.7f;

        accuracy = accData.startingAccuracy;
        this.bullets = bullets;
        if (accData.isDynamic) {
            thisShip.StartCoroutine(AccuracyChanger(accData));
        }
        thisShip.StartCoroutine(Logic());
    }

    private IEnumerator Logic() {
        shooting = false;
        accelerating = true;
        float duration;
        Vector2 newDir;
        while (true) {
            if (!Main.IsNull(target)) {
                //tickData.Refresh(thisShip, target);
                if (thisShip.velocity == Vector2.zero) {
                    SetAcceleration(true);
                    turnDirection = target.position - thisShip.position;
                    yield return null;
                } else {
                    var aim = getAim();
                    //transform perfect speed accordingly to current speed
                    Vector2 perfectSpeedDir = aim.directionDist.normalized;
                    Vector2 rightPerfect = makeRight(perfectSpeedDir);
                    var vProj = Vector2.Dot(rightPerfect, thisShip.velocity);
                    perfectSpeedDir = perfectSpeedDir * thisShip.velocity.magnitude - vProj * rightPerfect;
                    perfectSpeedDir.Normalize();

                    var angle = Math2d.ClosestAngleBetweenNormalizedRad(thisShip.velocity.normalized, perfectSpeedDir) * Mathf.Rad2Deg;
                    if (true) {//angle < 60f) {
                        SetAcceleration(true);
                        turnDirection = perfectSpeedDir;
                        float dtime = 0.2f;
                        float approximateTime = aim.time + 2 * angle / thisShip.turnSpeed;
                        Debug.LogWarning("approximateTime " + approximateTime + " " + angle);
                        if (approximateTime < dtime) {
                            yield return null;
                            thisShip.Kill();
                            yield break;
                        }
                        yield return null;
                    } else {
                        turnDirection = perfectSpeedDir;
                        if (thisShip.velocity.magnitude < thisShip.maxSpeed * 0.4f || (angle > 140f && Math2d.ClosestAngleBetweenNormalizedRad(thisShip.cacheTransform.right, perfectSpeedDir) * Mathf.Rad2Deg < 20f)) {
                        //if(IsBetterToAccelerateThanStop()) { 
                            Debug.LogWarning("SetAcceleration backward " + angle);
                            SetAcceleration(true);
                        } else {
                            Debug.LogWarning("Brake " + angle);
                            Brake();
                        }
                        yield return null;
                    }
                }
                
                //if (aim.canShoot) {

                //    float dtime = 1f;
                //    if (aim.time < dtime) {
                //        yield return new WaitForSeconds(dtime);
                //        thisShip.Kill();
                //        yield break;
                //    }

                //    turnDirection = aim.directionDist;
                //    SetAcceleration(true);
                //    yield return null;
                //} else {
                //    if (Vector2.Dot(thisShip.velocity, target.velocity) < 0) {
                //        Brake();
                //    } else {
                //        SetAcceleration(true);
                //    }
                //    turnDirection = target.position - thisShip.position;
                //    yield return null;
                //}
                //if (tickData.distCenter2Center > distToCheck) {
                //    GetDirectionForClosingIn(out newDir, out duration);
                //    Debug.LogWarning("fly closer " + duration);
                //yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration, accelerating: true));

                //    //if (Math2d.Chance(0.4f) && AIHelper.EvadeBullets(thisShip, bullets, out duration, out newDir)) {
                //    //    yield return thisShip.StartCoroutine(SetFlyDir(newDir, duration));
                //    //}
                //} else {
                //    float dtime = 1f;

                //    if ((thisShip.position + dtime * thisShip.velocity - target.position - target.velocity).magnitude < explosionRange*0.6f) {
                //        yield return new WaitForSeconds(1f);
                //        thisShip.Kill();
                //        yield break;
                //    }

                //    var aim = getAim();
                //    duration = Random.Range(0.5f, 1f);
                //    yield return thisShip.StartCoroutine(SetFlyDir(aim.directionDist, duration, accelerating: true));

                //    if ((thisShip.position + dtime * thisShip.velocity - target.position - target.velocity).magnitude < explosionRange * 0.6f) {
                //        yield return new WaitForSeconds(1f);
                //        thisShip.Kill();
                //        yield break;
                //    }
                //}
            } else {
                Brake();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    static private Vector2 makeRight(Vector2 v) {
        return new Vector2(v.y, -v.x);
    }

    private bool IsBetterToAccelerateThanStop() {
        if (Main.IsNull(target))
            return true;

        float deltaSecond = 0.01f;
        float sqrDistInDeltaSecond = (target.position + deltaSecond * target.velocity - (thisShip.position + deltaSecond * (thisShip.velocity +  deltaSecond * (Vector2)thisShip.cacheTransform.right * thisShip.thrust))).sqrMagnitude;
        float sqrDistInDeltaSecondIfStop = (target.position + deltaSecond * target.velocity - (thisShip.position + deltaSecond * (thisShip.velocity - thisShip.velocity.normalized * deltaSecond * thisShip.brake))).sqrMagnitude;
        return sqrDistInDeltaSecondIfStop < sqrDistInDeltaSecond;
    }

    private void GetDirectionForClosingIn(out Vector2 dir, out float time) {
        var aim = getAim();
        dir = aim.directionDist;
        float angle = Mathf.Atan2(distToCheck*0.6f, dir.magnitude);
        angle = Mathf.Sign(Random.Range(-1f, 1f)) * Random.Range(0, angle);

        Debug.DrawLine(thisShip.position, thisShip.position + dir);
        dir = Math2d.RotateVertex(dir, angle);
        Debug.DrawLine(thisShip.position, thisShip.position + dir, Color.cyan);

        time = Random.Range(1f, 1.5f);
    }

    private AimSystem getAim() {
        var aimVelocity = accuracy * (target.velocity);// - thisShip.velocity;
        float curSpeed = thisShip.velocity.magnitude;
        //float canShootHimSelfSpeed = Mathf.Min(thisShip.maxSpeed, curSpeed + 0.5f * thisShip.thrust) - curSpeed;
        AimSystem aim = new AimSystem(target.position, aimVelocity, thisShip.position, curSpeed);
        return aim;
    }

    private IEnumerator AccuracyChanger(AccuracyData data) {
        Vector2 lastDir = Vector2.one; //just not zero
        float dtime = data.checkDtime;
        while (true) {
            if (!Main.IsNull(target)) {
                AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
            }
            yield return new WaitForSeconds(dtime);
        }
    }
}
