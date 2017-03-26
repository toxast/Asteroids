using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MMinesGunData : MGunData, IGotShape
{
	[Header ("MinesGun")]
//	public float overrideExplosionDamage = -1;
//	public float overrideExplosionRadius = -1;
	public RandomFloat deceleration;
	public float activateRange = 5f;
	public float timerDuration = 5f;
	public bool explodeOnExpire = false;
	public ParticleSystemsData triggetExplosionEffect;

	[Header ("flow towards target")]
	public bool useFlowTowerdsTargetBeh = false;
	public float flowRange = 30f;
	public float flowAcceleration = 5f;
	public float flowMaxSpeed = 10f;
	public float flowStability = 0;

	public override Gun GetGun(Place place, PolygonGameObject t) {
		return new MinesGun(place, this, t);
	}
}