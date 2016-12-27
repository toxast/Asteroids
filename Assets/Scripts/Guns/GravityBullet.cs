using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBullet :PolygonGameObject {

    int affectLayer;
    float range;
    float damagePerSecond;
    float force;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject> ();

    public void InitGravityBullet(int affectLayer, MGravityGunData data) {
        this.affectLayer = affectLayer;
        range = data.range;
        damagePerSecond = data.damagePerSecond;
        force = data.force;
        gobjects = Singleton<Main>.inst.gObjects;
    }

    public override void Tick (float delta)
    {
        base.Tick (delta);
        new PhExplosion (position, range, delta * damagePerSecond, delta * force, gobjects, affectLayer);
    }
}
