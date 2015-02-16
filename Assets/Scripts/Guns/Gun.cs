using UnityEngine;
using System.Collections;
using System;

public class Gun : IGotTarget, ITickable
{
	protected PolygonGameObject target;
	public GunPlace place;
	public float bulletSpeed;
	public Transform transform;
	public float lifeTime;
	public float damage;
	public float fireInterval;

	public ParticleSystem fireEffect;

	public int repeatCount = 0;
	public float repeatInterval = 0;
	private int currentRepeat = 0;

	public float timeToNextShot = 0f;

	public event Action<IBullet> onFire;

	public Gun(GunPlace place)
	{
		this.place = place;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(float delta)
	{
		if(timeToNextShot > 0)
		{
			timeToNextShot -= delta;
		}
	}
	
	public bool ReadyToShoot()
	{
		return timeToNextShot <= 0;
	}
	
	public void ResetTime()
	{
		if(repeatCount > 0)
		{
			SetNextRepeatTime();
		}
		else
		{
			timeToNextShot = fireInterval;
		}
	}

	private void SetNextRepeatTime()
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
	
	public void ShootIfReady()
	{
		if(ReadyToShoot())
		{
			ResetTime();
			Fire(CreateBullet());
		}
	}

	protected virtual IBullet CreateBullet()
	{
		throw new System.NotImplementedException ();
	}

	protected void Fire(IBullet b)
	{
		if(onFire != null)
			onFire(b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}

	protected void PositionOnShooterPlace(PolygonGameObject bullet, Transform shooterTransform)
	{
		float angle = Math2d.GetRotation(place.dir);
		bullet.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		bullet.cacheTransform.position = place.pos;

		angle = Math2d.GetRotation(shooterTransform.right);
		bullet.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		bullet.cacheTransform.position += shooterTransform.position;
	}
}
