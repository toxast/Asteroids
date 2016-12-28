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
        //Hack: specific layer so it wont collide with anything
		int affectLayer = CollisionLayers.GetGravityBulletCollisions (CollisionLayers.GetBulletLayerNum(parent.layer));
        bullet.InitPolygonGameObject (new PhysicalData ());
        bullet.InitGravityBullet(affectLayer, data);
        bullet.InitLifetime (lifeTime);
        bullet.velocity = bullet.cacheTransform.right * bulletSpeed;
        bullet.destructionType = PolygonGameObject.DestructionType.eJustDestroy;
        bullet.destroyOnBoundsTeleport = true;

        if (data.bulletEffect != null) {
            var ps = GameObject.Instantiate (data.bulletEffect, bullet.transform);
//            ps.velocityOverLifetime.
//            var shape = ps.shape.radius.my;
            var shape = ps.shape;
            shape.radius = data.range;
            var main = ps.main;
            main.startSpeed = -data.range * 2;
            var em = ps.emission;
            em.rateOverTime = Mathf.PI * data.range * data.range / 2f;
//              ps.shape.radius = data.range;
//            ps.main.startSpeed = data.range;
               
//            ps.set
            ps.transform.localPosition = new Vector3 (0, 0, -1);
        }

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
