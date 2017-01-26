using UnityEngine;
using System;
using System.Collections;

public class BulletGun : GunShooterBase
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

    protected T CreateBullet<T>()
        where T : PolygonGameObject
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

	protected override void Fire()
	{
		var b = CreateBullet<PolygonGameObject>();

		b.velocity += Main.AddShipSpeed2TheBullet(parent);
		b.SetCollisionLayerNum(CollisionLayers.GetBulletLayerNum(parent.layer));

		Singleton<Main>.inst.HandleGunFire (b);

		if (fireEffect != null)
			fireEffect.Emit (1);
	}
}


public class ForcedBulletGun : BulletGun 
{
    MForcedBulletGun fdata;

    public ForcedBulletGun(Place place, MForcedBulletGun fdata, PolygonGameObject parent)
        : base(place, fdata, parent) {
        this.fdata = fdata;
    }

    protected override void Fire() {
        var bullet = CreateBullet<ForcedBullet>();
        var affectedLayer = CollisionLayers.GetLayerCollisions(CollisionLayers.GetBulletLayerNum(parent.layer));
        bullet.InitForcedBullet(fdata, affectedLayer);
        bullet.velocity += Main.AddShipSpeed2TheBullet(parent);
        Singleton<Main>.inst.HandleGunFire(bullet);
        if (fireEffect != null) {
            fireEffect.Emit(1);
        }
    }
}
