using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MForcedBulletGun : MGunData {

    [Header ("move bullet by force")]
    public float force = 20f;
    public float forceDuration = 2f;

    [Header ("damage field")]
    public float range = 20;
	public float fieldForce = -1000f;
    public float damagePerSecond = 3;
	public List<ParticleSystemsData> effectsForcedBullet;

	protected override void OnValidate () {
		base.OnValidate ();
		effectsForcedBullet.SetDefaultValues ();
	}

    public override Gun GetGun(Place place, PolygonGameObject t) {
        return new ForcedBulletGun(place, this, t);
    }
}
