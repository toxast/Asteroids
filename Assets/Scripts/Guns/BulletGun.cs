using UnityEngine;
using System;
using System.Collections;

public class BulletGun : Gun
{
	public Vector2[] vertices; 
	public Color color;

	public BulletGun(GunPlace place):base(place){}

	protected override IBullet CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);
		
		PositionOnShooterPlace (bullet, transform);
		
		bullet.gameObject.name = "bullet";
		
		bullet.Init(bulletSpeed, damage, lifeTime); 
		
		return bullet;
	}
}
