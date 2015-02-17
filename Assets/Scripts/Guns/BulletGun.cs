using UnityEngine;
using System;
using System.Collections;

public class BulletGun : Gun
{
	public BulletGun(GunPlace place, GunData data, Transform parentTransform):base(place, data, parentTransform){}

	protected override IBullet CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);
		
		PositionOnShooterPlace (bullet.cacheTransform);
		
		bullet.gameObject.name = "bullet";
		
		bullet.Init(bulletSpeed, damage, lifeTime); 
		
		return bullet;
	}
}
