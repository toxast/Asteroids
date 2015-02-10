using UnityEngine;
using System;
using System.Collections;

public class RocketLauncher : Gun
{
	public ParticleSystem thrusterEffect;
	public Color color;
	public RocketLauncher(GunPlace place):base(place){}

	public override void Tick(float delta)
	{
		if(timeToNextShot > 0)
		{
			timeToNextShot -= delta;
		}
	}
	
	public override bool ReadyToShoot()
	{
		return timeToNextShot <= 0 && target != null;
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
			Fire(CreateMissile());
		}
	}
	
	private BulletBase CreateMissile()
	{
		Missile missile = PolygonCreator.CreatePolygonGOByMassCenter<Missile>(missileVertices, color);

		if(thrusterEffect != null)
		{
			ParticleSystem e = GameObject.Instantiate(thrusterEffect) as ParticleSystem;
			e.transform.parent = missile.cacheTransform;
			e.transform.localPosition = new Vector3(0,0,-1);
		}

		PositionOnShooterPlace (missile, transform);
		
		missile.gameObject.name = "missile";
		
		missile.Init(target, bulletSpeed, damage, lifeTime); 
		
		return missile;
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
