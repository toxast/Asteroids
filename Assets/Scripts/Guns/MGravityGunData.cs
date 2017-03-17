using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGravityGunData : MGunData {
	[Header ("GravityGun")]
    public float force = 10;
    public float range = 20;
	public ParticleSystemsData gravityEffect;

    public override Gun GetGun(Place place, PolygonGameObject t)
    {
        return new GravityGun(place, this, t);
    }
}
