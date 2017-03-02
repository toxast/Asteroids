using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[SerializeField]
public class MFlamerGunData : MGunData
{
    public DOTEffect.Data dot;
	public RandomFloat deceleration;
	public float velocityRandomRange = 5;
	public bool forceUseBulletLifetime = false; //for use in flame trail

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FlamerGun(place, this, t);
	}
}

public class FlamerGun : BulletGun<FlamerBullet>
{
	MFlamerGunData fdata;
	float _range;
	public override float Range	{
		get { 
			return _range;
		}
	}

	float aimVelocity;
	public override float BulletSpeedForAim{ get { return aimVelocity; } }

	public FlamerGun(Place place, MFlamerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
		fdata = data;

		var velocity = GetVelocityMagnitude ();
		aimVelocity = velocity * 0.8f;
		float t =  0.8f * velocity / fdata.deceleration.Middle ;
		_range = t * velocity - 0.5f * t * t * fdata.deceleration.Middle; 
	}

	protected override void InitPolygonGameObject (FlamerBullet bullet, PhysicalData ph)
	{
		base.InitPolygonGameObject (bullet, ph);
	}

	protected override void SetCollisionLayer(FlamerBullet b)
	{
		base.SetCollisionLayer(b);
		b.InitFlamingBullet(fdata, b.velocity.magnitude);
	}

	protected override PolygonGameObject.DestructionType SetDestructionType () {
		return PolygonGameObject.DestructionType.eDisappear;
	}

	protected override float GetVelocityMagnitude () {
		return base.GetVelocityMagnitude() + UnityEngine.Random.Range(-fdata.velocityRandomRange, fdata.velocityRandomRange);
	}
}
