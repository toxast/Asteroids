using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MRocketGunData : MGunData {
	[Space(20)]
	[SerializeField] float explosionRangeCalculated;
	[SerializeField] float explosionDamageCalculated;
	[Header ("RocketLauncher")]
	public float overrideExplosionRadius = -1;
    public float overrideExplosionDamage = -1;
	public float accuracy = 0.5f;
	public SpaceshipData missleParameters;
	public List<ParticleSystemsData> thrusters;
	public Vector2 launchDirection;

    public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new RocketLauncher(place, this, t);
	}

	protected override void OnValidate(){
		base.OnValidate ();
		thrusters.SetDefaultValues ();
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
