using UnityEngine;
using System.Collections;
using System;

public class Gun : IGotTarget, ITickable
{
	protected IPolygonGameObject target;
	public Place place;
	public IPolygonGameObject parent;

	public float damage;
	public float lifeTime;
	public float bulletSpeed;
	public float fireInterval;
	public Vector2[] vertices; 
	public Color color;
	public ParticleSystem fireEffect;

	public int repeatCount = 0;
	public float repeatInterval = 0;
	private int currentRepeat = 0;

	public float timeToNextShot = 0f;

	public event Action<IBullet> onFire;

	public Gun(Place place, GunData data, IPolygonGameObject parent)
	{
		this.place = place;
		this.damage = data.damage;
		this.lifeTime = data.lifeTime;
		this.bulletSpeed = data.bulletSpeed;
		this.fireInterval = data.fireInterval;
		this.vertices = data.vertices;
		this.color = data.color;
		this.parent = parent;

		if(data.fireEffect != null)
		{
			fireEffect = GameObject.Instantiate(data.fireEffect) as ParticleSystem;
			Math2d.PositionOnShooterPlace(fireEffect.transform, place, parent.cacheTransform, true, -1);
//			PositionOnShooterPlace(fireEffect.transform);
//			fireEffect.transform.parent = parentTransform;
//			fireEffect.transform.position -=  new Vector3(0,0,1);
		}
	}

	public void SetTarget(IPolygonGameObject target)
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
	
	public virtual bool ReadyToShoot()
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

	protected virtual void SetBulletLayer(IBullet b)
	{
		if(parent.layer == (int)GlobalConfig.eLayer.USER || parent.layer == (int)GlobalConfig.eLayer.TEAM_USER)
		{
			b.SetCollisionLayerNum(GlobalConfig.ilayerBulletsUser);
		}
		else if(parent.layer == (int)GlobalConfig.eLayer.TEAM_ENEMIES)
		{
			b.SetCollisionLayerNum(GlobalConfig.ilayerBulletsEnemies);
		}
	}

	protected void Fire(IBullet b)
	{
		b.velocity += parent.velocity/2f;
		SetBulletLayer (b);

		if(onFire != null)
			onFire(b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}
