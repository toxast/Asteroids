using UnityEngine;
using System.Collections;
using System;

public class Gun : IGotTarget
{
	protected PolygonGameObject target;
	public GunPlace place;
	public float bulletSpeed;
	public Transform transform;
	public float lifeTime;
	public float damage;
	public float fireInterval;
	public float timeToNextShot = 0f;
	public ParticleSystem fireEffect;

	public event Action<IBullet> onFire;

	public Gun(GunPlace place)
	{
		this.place = place;
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public virtual void Tick(float delta)
	{
	}

	public virtual void ShootIfReady()
	{
	}

	public virtual bool ReadyToShoot()
	{
		return false;
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
