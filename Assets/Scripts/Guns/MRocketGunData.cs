using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MRocketGunData : MGunData {
	
	[Header ("RocketLauncher")]
//	public float overrideExplosionRadius = -1;
//    public float overrideExplosionDamage = -1;
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

}
