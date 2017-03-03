using UnityEngine;
using System;
using System.Collections;

public class BulletGun<T> : GunShooterBase where T : PolygonGameObject
{
    public MGunData data;

    public BulletGun(Place place, MGunData data, PolygonGameObject parent)
		:base(place, data, parent, data.repeatCount, data.repeatInterval, data.fireInterval, data.fireEffect)
	{
        this.data = data;
		range = data.velocity * data.lifeTime;
	}

	float range;
	public override float Range	{
		get{return range;}
	}

	public override float BulletSpeedForAim{ get { return data.velocity; } }

	public T CreateBullet( )
    {
		T bullet = PolygonCreator.CreatePolygonGOByMassCenter<T>(GetVerts(), data.color);
		bullet.gameObject.name = data.name;

		Math2d.PositionOnParent(bullet.cacheTransform, place, parent.cacheTransform);

		var ph = ApplyHeavvyBulletModifier (data.physical);
		InitPolygonGameObject (bullet, ph);
		bullet.InitLifetime (data.lifeTime);
		bullet.damageOnCollision = data.hitDamage;
		bullet.velocity = GetDirectionNormalized(bullet) * GetVelocityMagnitude();
		bullet.destructionType = SetDestructionType();
		bullet.destroyOnBoundsTeleport = DestroyOnBoundsTeleport;
        bullet.SetParticles(data.effects);
		bullet.SetDestroyAnimationParticles (data.destructionEffects);
		SetCollisionLayer( bullet );
		bullet.velocity += Main.AddShipSpeed2TheBullet(parent);
		bullet.rotation += data.rotation.RandomValue;
        return bullet;
	}

	protected virtual Vector2[] GetVerts() {
		return data.vertices;
	}

	protected virtual bool DestroyOnBoundsTeleport{
		get{ return true;}
	}

	protected virtual void InitPolygonGameObject(T bullet, PhysicalData ph)
	{
		bullet.InitPolygonGameObject (ph);
	}

	protected virtual float GetVelocityMagnitude(){
		return data.velocity;
	}

	protected virtual Vector2 GetDirectionNormalized(T bullet){
		var velocity = bullet.cacheTransform.right;
		if (data.spreadAngle > 0) {
			velocity = Math2d.RotateVertexDeg (velocity, UnityEngine.Random.Range (-data.spreadAngle * 0.5f, data.spreadAngle * 0.5f));
		}
		return velocity;
	}

	protected virtual PolygonGameObject.DestructionType SetDestructionType(){
		return PolygonGameObject.DestructionType.eSptilOnlyOnHit;
	}

    protected virtual void SetCollisionLayer(T bullet) {
		bullet.SetLayerNum(CollisionLayers.GetBulletLayerNum(parent.layerLogic));
    }

	protected virtual void AddToMainLoop(T b) {
		Singleton<Main>.inst.HandleGunFire (b);
	}

	protected override void Fire() {
		var b = CreateBullet();
		AddToMainLoop (b);

		if (fireEffect != null) {
			fireEffect.Emit (1);
		}
	}
}


public class ForcedBulletGun : BulletGun<ForcedBullet>
{
    MForcedBulletGun fdata;
    
    public ForcedBulletGun(Place place, MForcedBulletGun fdata, PolygonGameObject parent)
        : base(place, fdata, parent) {
        this.fdata = fdata;
    }

	protected override void SetCollisionLayer (ForcedBullet bullet)
	{
		base.SetCollisionLayer (bullet);
		bullet.collisions = 0;

		var affectedLayer = CollisionLayers.GetLayerCollisions(CollisionLayers.GetBulletLayerNum(parent.layerLogic));
		bullet.InitForcedBullet(fdata, affectedLayer);
	}
}