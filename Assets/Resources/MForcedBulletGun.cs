using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MForcedBulletGun : MGunData {
    [Header ("move bullet by force")]
    public float force = 20f;
    public float forceDuration = 2f;

    [Header ("damage field")]
    public float range = 20;
    public float damagePerSecond = 3;

    public override Gun GetGun(Place place, PolygonGameObject t) {
        return new ForcedBulletGun(place, this, t);
    }
}
