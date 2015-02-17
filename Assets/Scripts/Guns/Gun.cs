using UnityEngine;
using System.Collections;
using System;

public class Gun : IGotTarget, ITickable
{
	protected IPolygonGameObject target;
	public GunPlace place;
	public Transform parentTransform;

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

	public Gun(GunPlace place, GunData data, Transform parentTransform)
	{
		this.place = place;
		this.damage = data.damage;
		this.lifeTime = data.lifeTime;
		this.bulletSpeed = data.bulletSpeed;
		this.fireInterval = data.fireInterval;
		this.vertices = data.vertices;
		this.color = data.color;
		this.parentTransform = parentTransform;

		if(data.fireEffect != null)
		{
			fireEffect = GameObject.Instantiate(data.fireEffect) as ParticleSystem;
			PositionOnShooterPlace(fireEffect.transform);
			fireEffect.transform.parent = parentTransform;
			fireEffect.transform.position -=  new Vector3(0,0,1);
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

	protected void PositionOnShooterPlace(Transform objTransform)
	{
		float angle = Math2d.GetRotation(place.dir);
		objTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		objTransform.position = place.pos;

		angle = Math2d.GetRotation(parentTransform.right);
		objTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		objTransform.position += parentTransform.position;
	}
}
