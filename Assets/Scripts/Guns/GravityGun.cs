using UnityEngine;
using System;
using System.Collections;

public class GravityGun : GunShooterBase
{
    public float lifeTime;
    public float bulletSpeed;
    private MGravityGunData data;

    public GravityGun(Place place, MGravityGunData data, PolygonGameObject parent)
        :base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
    {
        this.data = data;
        this.lifeTime = data.lifeTime;
        this.bulletSpeed = data.bulletSpeed;
    }

    public override float Range
    {
        get{return bulletSpeed*lifeTime;}
    }

    public override float BulletSpeedForAim{ get { return bulletSpeed; } }

    private GravityBullet CreateBullet()
    {
        GravityBullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<GravityBullet>(data.vertices, data.color);
        bullet.gameObject.name = "gravity bullet";
        Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);
        bullet.InitPolygonGameObject (new PhysicalData ());
		float forceMultipier = parent.heavyBulletData != null ? parent.heavyBulletData.multiplier : 1f;
		int affectLayer = CollisionLayers.GetGravityBulletCollisions (CollisionLayers.GetBulletLayerNum(parent.layer));
		bullet.InitGravityBullet(affectLayer, data, forceMultipier);
        bullet.velocity = bullet.cacheTransform.right * bulletSpeed;
        bullet.destructionType = PolygonGameObject.DestructionType.eDisappear;
        bullet.destroyOnBoundsTeleport = true;
        return bullet;
    }

    protected override void Fire()
    {
        var b = CreateBullet ();

        b.velocity += Main.AddShipSpeed2TheBullet(parent);

        Singleton<Main>.inst.HandleGunFire (b);

        if (fireEffect != null)
            fireEffect.Emit (1);
    }
}
