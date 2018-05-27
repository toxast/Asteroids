using UnityEngine;
using System;
using System.Collections;

public class BulletGun<T> : GunShooterBase where T : PolygonGameObject
{
	public event Action<T> OnCreated;
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

		Place placeActual = place;
		if (data.spreadAngle > 0) {
			placeActual = place.Clone ();
			placeActual.dir = Math2d.RotateVertexDeg (placeActual.dir, UnityEngine.Random.Range (-data.spreadAngle * 0.5f, data.spreadAngle * 0.5f));
		}
		Math2d.PositionOnParent(bullet.cacheTransform, placeActual, parent.cacheTransform, false, -0.01f);
		var ph = ApplyHeavvyBulletModifier (data.physical);
		InitPolygonGameObject (bullet, ph);
		bullet.InitLifetime (data.lifeTime);
		ObjectsCreator.ApplyDeathData(bullet, data.deathData);
		bullet.damageOnCollision = data.hitDamage;
		bullet.velocity = GetDirectionNormalized(bullet) * GetVelocityMagnitude();
		bullet.destroyOnBoundsTeleport = DestroyOnBoundsTeleport;
        bullet.AddParticles(data.effects);
		bullet.AddDestroyAnimationParticles (data.destructionEffects);
		SetCollisionLayer( bullet );
		AddShipSpeed2TheBullet (bullet);
		bullet.rotation += data.rotation.RandomValue;
        return bullet;
	}

	protected virtual void AddShipSpeed2TheBullet(T bullet){
		bullet.velocity += Main.AddShipSpeed2TheBullet(parent);
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
		if (data.scriptEffect != null) {
			data.scriptEffect.Apply (bullet);
		}
		if (data.burnDOT.dps > 0 && data.burnDOT.duration > 0) {
			bullet.burnDotData = data.burnDOT;
		}
		if (data.iceData.Initialized()){
			bullet.iceEffectData = data.iceData;
		}
	}

	protected virtual float GetVelocityMagnitude(){
		return data.velocity;
	}

	protected virtual Vector2 GetDirectionNormalized(T bullet){
		return bullet.cacheTransform.right;
//		var velocity = bullet.cacheTransform.right;
//		if (data.spreadAngle > 0) {
//			velocity = Math2d.RotateVertexDeg (velocity, UnityEngine.Random.Range (-data.spreadAngle * 0.5f, data.spreadAngle * 0.5f));
//		}
//		return velocity;
	}

//	protected virtual PolygonGameObject.DestructionType SetDestructionType(){
//		return PolygonGameObject.DestructionType.eSptilOnlyOnHit;
//	}

    protected virtual void SetCollisionLayer(T bullet) {
		bullet.SetLayerNum(CollisionLayers.GetBulletLayerNum(parent.layerLogic));
    }

	protected virtual void AddToMainLoop(T b) {
		Singleton<Main>.inst.HandleGunFire (b);
	}

	protected override void Fire() {
		var b = CreateBullet();
		AddToMainLoop (b);
		if (OnCreated != null) {
			OnCreated (b);
		}
		if (fireEffect != null) {
			fireEffect.Emit (1);
		}
	}
}



