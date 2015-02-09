using UnityEngine;
using System.Collections;
using System;

public class Gun
{
	public GunPlace place;
	public float bulletSpeed;

	public event Action<BulletBase> onFire;

	public Gun(GunPlace place)
	{
		this.place = place;
	}

	public virtual void Tick(float delta)
	{
	}

	public virtual void ShootIfReady()
	{
	}

	protected void Fire(BulletBase b)
	{
		if(onFire != null)
			onFire(b);
	}

	protected float PositionOnShooterPlace(PolygonGameObject bullet, Transform shooterTransform)
	{
		float angle = Math2d.GetRotation(place.dir);
		bullet.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		bullet.cacheTransform.position = place.pos;

		angle = Math2d.GetRotation(shooterTransform.right);
		bullet.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		bullet.cacheTransform.position += shooterTransform.position;
		return angle;
	}
}
