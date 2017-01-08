using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	public float overrideExplosionDamage;
	public PhysicalData physical;
	List<ParticleSystemsData> thrusters;
	List<ParticleSystemsData> partcles;

	MRocketGunData data;

	public RocketLauncher(Place place, MRocketGunData data, PolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{ 
		this.data = data;
		vertices = data.vertices;
		missleParameters = data.missleParameters;
		physical = data.physical;
//		thrusterEffect = data.thrusterEffect;
//		thrusterPos = data.thrusterPos;
		thrusters = new List<ParticleSystemsData>(data.thrusters);
		partcles = new List<ParticleSystemsData> (data.particles);
		launchDirection = data.launchDirection;
		launchSpeed = data.launchSpeed;
		accuracy = data.accuracy;
		overrideExplosionRadius = data.overrideExplosionRadius;
		lifeTime = data.lifeTime;
		color = data.color;
		overrideExplosionDamage = data.overrideExplosionDamage;
	}

	public override float Range
	{
		get{return missleParameters.maxSpeed*lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return missleParameters.maxSpeed; } }

	private SpaceShip CreateMissile()
	{
		SpaceShip missile = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(vertices, color);



		Math2d.PositionOnParent (missile.cacheTransform, place, parent.cacheTransform);

		missile.gameObject.name = "missile";
		missile.InitSpaceShip(physical, missleParameters);
		missile.InitLifetime (lifeTime);

		missile.damageOnCollision = data.damageOnCollision;
		missile.destroyOnBoundsTeleport = true;
		missile.destructionType = PolygonGameObject.DestructionType.eDisappear;
		missile.overrideExplosionDamage = overrideExplosionDamage; 
		missile.overrideExplosionRange = overrideExplosionRadius;
		if(thrusters != null) {
			missile.SetThrusters (thrusters);
		}
		missile.SetParticles (partcles);
		missile.SetDestroyAnimationParticles (data.destructionEffects);

		var controller = new MissileController (missile, missleParameters.maxSpeed, accuracy);
		missile.SetController (controller);
		missile.targetSystem = new MissileTargetSystem (missile);

		if (data.explosionOnDestruction) {
			DeathAnimation.MakeDeathForThatFellaYo (missile, true);
		}

		if(launchDirection != Vector2.zero)
		{
			float angle = Math2d.GetRotationRad(missile.cacheTransform.right);
			var byPlace = Math2d.RotateVertex(launchDirection, angle);
			missile.velocity += byPlace.normalized * launchSpeed;
		}

		return missile;
	}

	protected override void Fire()
	{
		var fireElem = CreateMissile ();

		fireElem.velocity += Main.AddShipSpeed2TheBullet(parent);
		fireElem.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));

		Singleton<Main>.inst.HandleGunFire (fireElem);

		if (fireEffect != null)
			fireEffect.Emit (1);
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
