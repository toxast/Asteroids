using UnityEngine;
using System;
using System.Collections;

public class BulletGun : Gun
{
	public BulletGun(Place place, GunData data, IPolygonGameObject parent):base(place, data, parent){}

	protected override IBullet CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);

		Math2d.PositionOnShooterPlace(bullet.cacheTransform, place, parent.cacheTransform);
		//PositionOnShooterPlace (bullet.cacheTransform);
		
		bullet.gameObject.name = "bullet";
		
		bullet.Init(bulletSpeed, damage, lifeTime); 
		
		return bullet;
	}
}
