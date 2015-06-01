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

	public virtual float Range
	{
		get{return bulletSpeed*lifeTime;}
	}

	public Gun(){}

	public Gun(Place place, GunData data, IPolygonGameObject parent)
	{
		this.place = place;
		this.damage = data.damage;
		this.lifeTime = data.lifeTime;
		this.bulletSpeed = data.bulletSpeed;
		this.fireInterval = data.fireInterval;
		this.vertices = data.vertices;
		this.color = data.color;
		this.repeatCount = data.repeatCount;
		this.repeatInterval = data.repeatInterval;
		this.parent = parent;

		if(data.fireEffect != null)
		{
			fireEffect = GameObject.Instantiate(data.fireEffect) as ParticleSystem;
			Math2d.PositionOnParent(fireEffect.transform, place, parent.cacheTransform, true, -1);
		}
	}

	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}

	public virtual void Tick(float delta)
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
	
	public virtual void ShootIfReady()
	{
		if(ReadyToShoot())
		{
			Fire(CreateBullet());
			ResetTime();
		}
	}

	protected virtual IBullet CreateBullet()
	{
		throw new System.NotImplementedException ();
	}

	protected virtual void SetBulletLayer(IBullet b)
	{
		b.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));
	}

//	protected virtual void SetBulletTarget(IBullet b)
//	{
//
//	}

	protected virtual void Fire(IBullet b)
	{
		b.velocity += Main.AddShipSpeed2TheBullet(parent);
		SetBulletLayer (b);
//		SetBulletTarget (b);

		if(onFire != null)
			onFire(b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}
