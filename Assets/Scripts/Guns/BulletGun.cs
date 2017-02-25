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
	public float spreadAngle;
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
		this.spreadAngle = data.spreadAngle;
	}

	public override float Range
	{
		get{return bulletSpeed*lifeTime;}
	}

	public override float BulletSpeedForAim{ get { return bulletSpeed; } }

	protected T CreateBullet( )
    {
        T bullet = PolygonCreator.CreatePolygonGOByMassCenter<T>(vertices, color);
		bullet.gameObject.name = "bullet";

		Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);

		var ph = ApplyHeavvyBulletModifier (physical);
		bullet.InitPolygonGameObject (ph);
		bullet.InitLifetime (lifeTime);
		bullet.damageOnCollision = damage;
		var velocity = bullet.cacheTransform.right * GetBulletVelocity();
		if (spreadAngle > 0) {
			velocity = Math2d.RotateVertexDeg (velocity, UnityEngine.Random.Range (-spreadAngle * 0.5f, spreadAngle * 0.5f));
		}
		bullet.velocity = velocity;
		bullet.destructionType = GetBulletDestruction();
		bullet.destroyOnBoundsTeleport = true;
        bullet.SetParticles(data.effects);
		bullet.SetDestroyAnimationParticles (data.destructionEffects);

        return bullet;
	}

	protected virtual float GetBulletVelocity(){
		return bulletSpeed;
	}

	protected virtual PolygonGameObject.DestructionType GetBulletDestruction(){
		return PolygonGameObject.DestructionType.eSptilOnlyOnHit;
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