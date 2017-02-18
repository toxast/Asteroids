using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MFireballGunData : MGunBaseData
{
	public float damageOnCollision = 0;
	public float lifeTime = 2;
	public float fireInterval = 0.5f;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	public PhysicalData physical;
	public float radius = 3f; 
	public ParticleSystem fireEffect;
	public SpaceshipData missleParameters;
	public float accuracy = 0.5f;
	public List<ParticleSystemsData> particles;
	public List<ParticleSystemsData> destructionEffects;
	public DOTEffect.Data dot;
	public Vector2 launchDirection;
	public float launchSpeed;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FireballGun(place, this, t);
	}
}

public class FireballGun : GunShooterBase
{
	MFireballGunData data;

	public FireballGun(Place place, MFireballGunData data, PolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{ 
		this.data = data;
	}

	public override float Range	{
		get{return data.missleParameters.maxSpeed*data.lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return data.missleParameters.maxSpeed; } }

	private SpaceShip CreateFireball() {
		var vertices = PolygonCreator.CreatePerfectPolygonVertices(data.radius, 6);
		SpaceShip fireball = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(vertices, Color.red);
		fireball.SetAlpha (0);
		Math2d.PositionOnParent (fireball.cacheTransform, place, parent.cacheTransform);
		fireball.gameObject.name = "missile";
		fireball.InitSpaceShip(data.physical, data.missleParameters);
		fireball.InitLifetime (data.lifeTime);
		fireball.burnDot = data.dot;
		fireball.damageOnCollision = data.damageOnCollision;
		fireball.destroyOnBoundsTeleport = true;
		fireball.destructionType = PolygonGameObject.DestructionType.eDisappear;
		fireball.SetParticles (data.particles);
		fireball.SetDestroyAnimationParticles (data.destructionEffects);
		var controller = new MissileController (fireball, data.accuracy);
		fireball.SetController (controller);
		fireball.targetSystem = new MissileTargetSystem (fireball);
		if (data.launchDirection != Vector2.zero) {
			float angle = Math2d.GetRotationRad (fireball.cacheTransform.right);
			var byPlace = Math2d.RotateVertex (data.launchDirection, angle);
			fireball.velocity += byPlace.normalized * data.launchSpeed;
		} else {
			Debug.LogError ("zero launch dir!");
		}
		return fireball;
	}

	protected override void Fire()
	{
		var fireElem = CreateFireball ();

		fireElem.velocity += Main.AddShipSpeed2TheBullet(parent);
		fireElem.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));

		Singleton<Main>.inst.HandleGunFire (fireElem);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}

