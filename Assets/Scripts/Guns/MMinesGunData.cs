using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MMinesGunData : MGunData, IGotShape
{
	[Space(20)]
	[SerializeField] float explosionRangeCalculated;
	[SerializeField] float explosionDamageCalculated;

	[Header ("MinesGun")]
	public float overrideExplosionDamage = -1;
	public float overrideExplosionRadius = -1;
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

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new MinesGun(place, this, t);
	}

	protected override float HitDamage ()
	{
		float explosionDamage = 0;
		if (vertices.Length > 2) {
			float area;
			Math2d.GetMassCenter (vertices, out area);
			explosionRangeCalculated = overrideExplosionRadius >= 0 ? overrideExplosionRadius : DeathAnimation.ExplosionRadius (area);
			explosionDamageCalculated = DeathAnimation.ExplosionDamage (explosionRangeCalculated);
		}
		if (overrideExplosionDamage > 0) {
			explosionDamage += overrideExplosionDamage;
		} else {
			explosionDamage += explosionDamageCalculated;
		}

		return base.HitDamage () + explosionDamage;
	}

}