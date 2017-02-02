using UnityEngine;
using System;
using System.Collections;

public class BulletGun<T> : GunShooterBase where T : PolygonGameObject
{
	public Vector2[] vertices; 
	public PhysicalData physical;
	public float lifeTime;
	public float bulletSpeed;
	public Color color;
	public float damage;
    public MGunData data;

    public BulletGun(Place place, MGunData data, PolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{
        this.data = data;
		this.vertices = data.vertices;
		this.physical = data.physical;
		this.lifeTime = data.lifeTime;
		this.bulletSpeed = data.bulletSpeed;
		this.color = data.color;
		this.damage = data.damage;
	}

	public override float Range
	{
		get{return bulletSpeed*lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return bulletSpeed; } }

    protected T CreateBullet()
    {
        T bullet = PolygonCreator.CreatePolygonGOByMassCenter<T>(vertices, color);
		bullet.gameObject.name = "bullet";

		Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);

		bullet.InitPolygonGameObject (physical);
		bullet.InitLifetime (lifeTime);
		bullet.damageOnCollision = damage;
		bullet.velocity = bullet.cacheTransform.right * bulletSpeed;
		bullet.destructionType = PolygonGameObject.DestructionType.eSptilOnlyOnHit;
		bullet.destroyOnBoundsTeleport = true;
        bullet.SetParticles(data.effects);

        return bullet;
	}

    protected virtual void InitBullet(T bullet)
    {
        bullet.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));
    }

	protected override void Fire()
	{
		var b = CreateBullet();

        InitBullet( b );

		b.velocity += Main.AddShipSpeed2TheBullet(parent);

		Singleton<Main>.inst.HandleGunFire (b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}


public class ForcedBulletGun : BulletGun<ForcedBullet>
{
    MForcedBulletGun fdata;
    
    public ForcedBulletGun(Place place, MForcedBulletGun fdata, PolygonGameObject parent)
        : base(place, fdata, parent) {
        this.fdata = fdata;
    }

    protected override void InitBullet(ForcedBullet bullet)
    {
        var affectedLayer = CollisionLayers.GetLayerCollisions(CollisionLayers.GetBulletLayerNum(parent.layer));
        bullet.InitForcedBullet(fdata, affectedLayer);
    }
}


public class SpreadBulletGun<T> : BulletGun<T> where T : PolygonGameObject
{
    MSpreadBulletGunData sdata;

    public SpreadBulletGun(Place place, MSpreadBulletGunData data, PolygonGameObject parent)
        : base(place, data, parent) {
        this.sdata = data;
    }

    protected override void InitBullet( T bullet )
    {
        base.InitBullet(bullet);
        bullet.velocity = Math2d.RotateVertex(bullet.velocity, UnityEngine.Random.Range(-sdata.spreadAngle * 0.5f, sdata.spreadAngle * 0.5f));
    }
}