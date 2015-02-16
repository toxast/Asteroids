using UnityEngine;
using System;
using System.Collections;

public class BulletGun : Gun
{
	public Vector2[] vertices; 
	public Color color;

	public int repeatCount = 0;
	public float repeatInterval = 0;
	private int currentRepeat = 0;


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

		if(repeatCount > 0)
		{
			currentRepeat ++;
			if(currentRepeat >= repeatCount)
				currentRepeat = 0;

			if(currentRepeat == 0)
			{
				timeToNextShot = fireInterval;
			}
			else
			{
				timeToNextShot = repeatInterval;
			}
		}
		else
		{
			timeToNextShot = fireInterval;
		}
	}
	
	public override void ShootIfReady()
	{
		if(ReadyToShoot())
		{
			ResetTime();
			Fire(CreateBullet());
		}
	}

	public IBullet CreateBullet()
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(vertices, color);
		
		PositionOnShooterPlace (bullet, transform);
		
		bullet.gameObject.name = "bullet";
		
		bullet.Init(bulletSpeed, damage, lifeTime); 
		
		return bullet;
	}
}
