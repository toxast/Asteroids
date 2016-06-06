using UnityEngine;
using System;
using System.Collections;

public class RocketLauncher : GunShooterBase
{
	public ParticleSystem thrusterEffect;
	private SpaceshipData missleParameters;
	private Vector2 thrusterPos;
	private Vector2 launchDirection;
	private float launchSpeed;
	private float accuracy;
	private float overrideExplosionRadius;
	public Vector2[] vertices; 
	public float lifeTime;
	public Color color;
	public float damage;

	public RocketLauncher(Place place, MRocketGunData data, IPolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{ 
		vertices = data.vertices;
		missleParameters = data.missleParameters;
		thrusterEffect = data.thrusterEffect;
		thrusterPos = data.thrusterPos;
		launchDirection = data.launchDirection;
		launchSpeed = data.launchSpeed;
		accuracy = data.accuracy;
		overrideExplosionRadius = data.overrideExplosionRadius;
		lifeTime = data.lifeTime;
		this.color = data.color;
		this.damage = data.damage;
	}

	public override float Range
	{
		get{return missleParameters.maxSpeed*lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return missleParameters.maxSpeed; } }

	protected override IBullet CreateBullet()
	{
		Missile missile = PolygonCreator.CreatePolygonGOByMassCenter<Missile>(vertices, color);

		if(thrusterEffect != null)
		{
			ParticleSystem e = GameObject.Instantiate(thrusterEffect) as ParticleSystem;
			e.transform.parent = missile.cacheTransform;
			e.transform.localPosition = thrusterPos;
		}

		Math2d.PositionOnParent (missile.cacheTransform, place, parent.cacheTransform);

		missile.gameObject.name = "missile";
		var controller = new MissileController (missile, missleParameters.maxSpeed, accuracy);
		missile.InitMissile(1f, missleParameters, damage, overrideExplosionRadius, lifeTime);
		missile.destroyOnBoundsTeleport = true;
		missile.destructionType = PolygonGameObject.DestructionType.eJustDestroy;
		missile.SetController (controller);
		missile.targetSystem = new MissileTargetSystem (missile);
		if(launchDirection != Vector2.zero)
		{
			float angle = Math2d.GetRotationRad(missile.cacheTransform.right);
			var byPlace = Math2d.RotateVertex(launchDirection, angle);
			missile.velocity += byPlace.normalized * launchSpeed;
		}
		return missile;
	}

	public static Vector2[] missileVertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
		new Vector2(2f, 0f),
		new Vector2(1.5f, -0.2f),
		new Vector2(0.3f, -0.15f),
		new Vector2(0f, -0.35f),
	}
	, 1f).ToArray();
}
