using UnityEngine;
using System.Collections;
using System;

public abstract class Gun : IGotTarget, ITickable
{
	protected PolygonGameObject target;
	public Place place;
	public PolygonGameObject parent;

	public Gun(Place place, MGunBaseData basedata, PolygonGameObject parent)
	{
		this.place = place;
		this.parent = parent;
	}

	public virtual float Range
	{
		get{return 0;}
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public virtual void Tick(float delta){}

	public abstract float BulletSpeedForAim{ get;}

	public abstract void SetTimeForNextShot();

	public abstract void ShootIfReady();

	public abstract bool ReadyToShoot();

	public virtual void OnGunRemoving(){}

	protected PhysicalData ApplyHeavvyBulletModifier(PhysicalData physical) {
		if (parent.heavyBulletData != null) {
			var phClone = physical.Clone ();
			phClone.density *= parent.heavyBulletData.multiplier;
			return phClone;
		} else {
			return physical;
		}
	}
}





public abstract class GunShooterBase : Gun
{
	public float fireInterval;
	public ParticleSystem fireEffect;
	public int repeatCount = 0;
	public float repeatInterval = 0;
	private int currentRepeat = 0;
	public float timeToNextShot = 0f;

	public GunShooterBase(Place place, MGunBaseData basedata, PolygonGameObject parent, int repeatCount, float repeatInterval, float fireInterval, ParticleSystem pfireEffect): base(place, basedata, parent)
	{
		this.fireInterval = fireInterval;
		this.repeatCount = repeatCount;
		this.repeatInterval = repeatInterval;

		if(pfireEffect != null)
		{
			fireEffect = GameObject.Instantiate(pfireEffect) as ParticleSystem;
			Math2d.PositionOnParent(fireEffect.transform, place, parent.cacheTransform, true, -1);
		}
	}

	public override void Tick(float delta) {
		if (timeToNextShot > 0) {
			timeToNextShot -= delta;
		}
		if (currentRepeat != 0) {
			ShootIfReady ();
		}
	}

	public override bool ReadyToShoot()	{
		return timeToNextShot <= 0;
	}

	public override void SetTimeForNextShot() {
		float shootInterval = fireInterval;
		if (repeatCount > 0) {
			currentRepeat++;
			if (currentRepeat >= repeatCount) {
				currentRepeat = 0;
			}
			if (currentRepeat != 0) {
				shootInterval = repeatInterval;
			}
		}
		if (shootInterval <= 0) {
			shootInterval = 0.1f;
			Debug.LogError ("wrong shoot interval, infinite loop warning");
		}

		timeToNextShot += shootInterval;
	}

	public override void ShootIfReady()
	{
		while(ReadyToShoot()) {
			Fire();
			SetTimeForNextShot();
		}
	}

	public override void OnGunRemoving ()
	{
		base.OnGunRemoving ();
		if (fireEffect != null) {
			GameObject.Destroy (fireEffect.gameObject);
			fireEffect = null;
		}
	}

	protected abstract void Fire ();
}

