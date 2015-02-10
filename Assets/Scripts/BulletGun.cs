using UnityEngine;
using System;
using System.Collections;

public class BulletGun : Gun
{
	public Vector2[] vertices; 
	public Color color;

	public BulletGun(GunPlace place):base(place)
	{
	}

	public override void Tick(float delta)
	{
		if(timeToNextShot > 0)
		{
			timeToNextShot -= delta;
		}
	}
	
	public override bool ReadyToShoot()
	{
		return timeToNextShot <= 0;
	}
	
	public void ResetTime()
	{
		timeToNextShot = fireInterval;
	}
	
	public override void ShootIfReady()
	{
		if(ReadyToShoot())
		{
			ResetTime();
			Fire(CreateBullet());
		}
	}

	public BulletBase CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);
		
		PositionOnShooterPlace (bullet, transform);
		
		bullet.gameObject.name = "bullet";
		
		bullet.Init(bulletSpeed, damage, lifeTime); 
		
		return bullet;
	}
}
