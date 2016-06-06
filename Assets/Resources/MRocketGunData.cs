using UnityEngine;
using System.Collections;

public class MRocketGunData : MGunBaseData, IGotShape
{
	public float damage = 3;
	public float lifeTime = 2;
	public float fireInterval = 0.5f;
	public PhysicalData physical;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public Vector2[] vertices = PolygonCreator.GetRectShape(0.4f, 0.2f); 
	public Color color = Color.red;
	public ParticleSystem fireEffect;

	public float overrideExplosionRadius = -1;
	public SpaceshipData missleParameters;
	public float accuracy = 0.5f;
	public ParticleSystem thrusterEffect;
	public Vector3 thrusterPos;

	public Vector2 launchDirection;
	public float launchSpeed;

	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	public override Gun GetGun(Place place, IPolygonGameObject t)
	{
		return new RocketLauncher(place, this, t);
	}

}
