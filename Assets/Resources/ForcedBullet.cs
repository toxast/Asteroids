﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForcedBullet : PolygonGameObject {

    int affectLayer;
    Vector2 forceDir;
    MForcedBulletGun data;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject>();
     
    public void InitForcedBullet(MForcedBulletGun data, int affectLayer) {
        this.data = data;
        this.affectLayer = affectLayer;
        forceDir = Math2d.RandomSign() * Math2d.MakeRight(cacheTransform.right);
        //velocity -= forceDir * data.forceDuration * data.force;
        gobjects = Singleton<Main>.inst.gObjects;
        StartCoroutine(ForceChange());
    }

    const float checkEvery = 0.16f;
    float timeLeftForCheck = checkEvery;
    public override void Tick(float delta) {
        base.Tick(delta);

        velocity += forceDir * delta * data.force;

        timeLeftForCheck -= delta;
        if (timeLeftForCheck < 0) {
            timeLeftForCheck += checkEvery;
            TickEvery(checkEvery);
        }
    }

    IEnumerator ForceChange() {
        yield return new WaitForSeconds(data.forceDuration / Mathf.Sqrt(2));
        forceDir = -forceDir;
        yield return new WaitForSeconds(data.forceDuration / Mathf.Sqrt(2));
        yield return new WaitForSeconds(data.forceDuration);
        while (true) {
            forceDir = -forceDir;
            yield return new WaitForSeconds(2f * data.forceDuration);
        }
    }

    private void TickEvery(float sec) {
        
        new PhExplosion(position, data.range, sec * data.damagePerSecond, 0, gobjects, affectLayer);
    }
}
