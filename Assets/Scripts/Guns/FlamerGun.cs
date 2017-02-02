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


    //float currentPower;

	public override float Range
	{
		get { return flamerData.range; }
	}

    public FlamerGun(Place place, MFlamerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
        flamerData = data;
        //currentPower = 0.0f;
	}

    protected override void InitBullet(FlamingBullet b)
    {
        b.Init(flamerData);
        base.InitBullet(b);
		b.destructionType = PolygonGameObject.DestructionType.eDisappear;
    }

    public override float BulletSpeedForAim{ get { return Mathf.Infinity; } }
}
