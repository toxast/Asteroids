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
	public Vector2[] vertices; 
	public float lifeTime;
	public Color color;
	private float overrideExplosionRadius;
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
        thrusters = data.thrusters.Clone();
        partcles = data.particles.Clone();

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
		var ph = ApplyHeavvyBulletModifier (physical);
		missile.InitSpaceShip(ph, missleParameters);
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

		var controller = new MissileController (missile, accuracy);
		missile.SetController (controller);
		missile.targetSystem = new MissileTargetSystem (missile);

		DeathAnimation.MakeDeathForThatFellaYo (missile, true);

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
}
