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

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new FlamerGun(place, this, t);
	}
}

public class FlamerGun : BulletGun<FlamerBullet>
{
	MFlamerGunData fdata;

	public override float Range	{
		get { 
			float t =  0.8f * bulletSpeed / fdata.deceleration.Middle ;
			return t * bulletSpeed - 0.5f * t * t * fdata.deceleration.Middle; 
		}
	}
	public override float BulletSpeedForAim{ get { return fdata.bulletSpeed * 0.8f; } }

	public FlamerGun(Place place, MFlamerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
		fdata = data;
	}

	protected override void InitBullet(FlamerBullet b)
	{
		base.InitBullet(b);
		b.InitFlamingBullet(fdata, b.velocity);
	}

	protected override PolygonGameObject.DestructionType GetBulletDestruction ()
	{
		return PolygonGameObject.DestructionType.eDisappear;
	}

	protected override float GetBulletVelocity ()
	{
		return base.GetBulletVelocity () + UnityEngine.Random.Range(-fdata.velocityRandomRange, fdata.velocityRandomRange);
	}
}
