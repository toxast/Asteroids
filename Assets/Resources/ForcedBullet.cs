using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForcedBullet : PolygonGameObject {

    int affectLayer;
    Vector2 forceDir;
    MForcedBulletGun data;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject>();
    float maxSpeed;
    float maxSppedSqr;
     
    public void InitForcedBullet(MForcedBulletGun data, Vector2 firstForceDir, int affectLayer) {
        this.data = data;
        this.forceDir = firstForceDir.normalized;
        this.affectLayer = affectLayer;
        maxSpeed = 10f * data.bulletSpeed; //TODO: by force
        maxSppedSqr = maxSpeed * maxSpeed;
        gobjects = Singleton<Main>.inst.gObjects;
        StartCoroutine(ForceChange());
        Debug.LogError("start " + velocity.magnitude);
    }

    const float checkEvery = 0.16f;
    float timeLeftForCheck = checkEvery;
    public override void Tick(float delta) {
        base.Tick(delta);
        timeLeftForCheck -= delta;
        if (timeLeftForCheck < 0) {
            timeLeftForCheck += checkEvery;
            TickEvery(checkEvery);
        }
    }

    IEnumerator ForceChange() {
        while (true) {
            yield return new WaitForSeconds(2f * data.forceDuration);
            Debug.LogError("change force " + velocity.magnitude);
            forceDir = -forceDir;
        }
    }

    private void TickEvery(float sec) {
        Accelerate(sec, data.force, 0, maxSpeed, maxSppedSqr, forceDir);
        new PhExplosion(position, data.range, sec * data.damagePerSecond, 0, gobjects, affectLayer);
    }
}
