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

	public BulletGun(Place place, MGunData data, IPolygonGameObject parent)
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

	protected override IBullet CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);

		Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);
		//PositionOnShooterPlace (bullet.cacheTransform);
		bullet.destroyOnBoundsTeleport = true;
		bullet.destructionType = PolygonGameObject.DestructionType.eJustDestroy;
		bullet.gameObject.name = "bullet";
		bullet.InitBullet(bulletSpeed, damage, lifeTime, physical); 
		
		return bullet;
	}
}
