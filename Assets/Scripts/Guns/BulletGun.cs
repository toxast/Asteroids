using UnityEngine;
using System;
using System.Collections;

public class BulletGun : GunShooterBase
{
	public Vector2[] vertices; 
	public PhysicalData physical;
	public float lifeTime;
	public float bulletSpeed;
	public Color color;
	public float damage;

	public BulletGun(Place place, MGunData data, PolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{
		this.vertices = data.vertices;
		this.physical = data.physical;
		this.lifeTime = data.lifeTime;
		this.bulletSpeed = data.bulletSpeed;
		this.color = data.color;
		this.damage = data.damage;
	}

	public override float Range
	{
		get{return bulletSpeed*lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return bulletSpeed; } }

	private PolygonGameObject CreateBullet()
	{
		PolygonGameObject bullet = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(vertices, color);
		bullet.gameObject.name = "bullet";

		Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);

		bullet.InitPolygonGameObject (physical);
		bullet.InitLifetime (lifeTime);
		bullet.damageOnCollision = damage;
		bullet.velocity = bullet.cacheTransform.right * bulletSpeed;
		bullet.destructionType = PolygonGameObject.DestructionType.eSptilOnlyOnHit;
		bullet.destroyOnBoundsTeleport = true;

		return bullet;
	}

	protected override void Fire()
	{
		var b = CreateBullet ();

		b.velocity += Main.AddShipSpeed2TheBullet(parent);
		b.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));

		Singleton<Main>.inst.HandleGunFire (b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}
