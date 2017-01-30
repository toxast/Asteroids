using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DeceleratingBullet : PolygonGameObject
{
    float deceleration;

    public void Init( MSpreadBulletGunData data )
    {
        this.deceleration = data.deceleration;
    }

    public override void Tick(float delta)
    {
        Brake(delta, deceleration);
        base.Tick(delta);
    }
}

public class FlamingBullet : DeceleratingBullet
{
    DOTEffect.Data dot;

    public void Init( MFlamerGunData data )
    {
        this.dot = data.dot;
        base.Init(data);
    }

    public override void OnHit(PolygonGameObject other)
    {
        other.AddEffect(new BurningEffect(dot));
    }
}

public class FlamerGun : SpreadBulletGun<FlamingBullet>
{
    MFlamerGunData flamerData;

    float currentPower;
    //bool isShooting;

    //int layer;

	public override float Range
	{
		get { return flamerData.range; }
	}

    public FlamerGun(Place place, MFlamerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
        flamerData = data;

        currentPower = 0.0f;
        //isShooting = false;

        //layer = 1 << CollisionLayers.GetBulletLayerNum(parent.layer);
	}

    protected override void InitBullet(FlamingBullet b)
    {
        b.Init(flamerData);
        base.InitBullet(b);
    }

    //public override void Tick(float delta)
    //{
    //    if ( isShooting )
    //    {
    //        currentPower = Mathf.Min( currentPower + delta / timeToFullpower, 1.0f );

    //        // tick-discrete shooting
    //        isShooting = false;
    //    }
    //    else
    //    {
    //        currentPower = Mathf.Max( currentPower - delta / timeToCool, 0.0f );

    //        return;
    //    }

    //    if (fireEffect != null)
    //  fireEffect.Emit (1);

    //    var currentRange = range * currentPower;
    //}

    public override float BulletSpeedForAim{ get { return Mathf.Infinity; } }

    //public override void ResetTime() { }

	//public override void ShootIfReady()
 //   {
 //       isShooting = true;
 //   }

	//public override bool ReadyToShoot() { return true; }
}
