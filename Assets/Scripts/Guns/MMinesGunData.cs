using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MMinesGunData : MGunData, IGotShape
{
	[Header ("MinesGun")]
	public float overrideExplosionDamage = -1;
	public float overrideExplosionRadius = -1;
	public RandomFloat deceleration;
	public float activateRange = 5f;
	public float timerDuration = 5f;
	public bool explodeOnExpire = false;
	public ParticleSystemsData triggetExplosionEffect;

	public override Gun GetGun(Place place, PolygonGameObject t)
	{
		return new MinesGun(place, this, t);
	}
}

public class MinesGun : BulletGun<Mine>
{
	new MMinesGunData data;

	float _range;
	public override float Range	{
		get { 
			return _range;
		}
	}

	float aimVelocity;
	public override float BulletSpeedForAim{ get { return aimVelocity; } }


	public MinesGun(Place place, MMinesGunData data, PolygonGameObject parent)
		:base(place, data, parent)
	{ 
		this.data = data;

		var velocity = GetVelocityMagnitude ();
		aimVelocity = velocity * 0.8f;
		float t =  0.8f * velocity / data.deceleration.Middle ;
		_range = t * velocity - 0.5f * t * t * data.deceleration.Middle; 
	}

	protected override void InitPolygonGameObject (Mine bullet, PhysicalData ph) {
		base.InitPolygonGameObject (bullet, ph);

		bullet.overrideExplosionDamage = data.overrideExplosionDamage; 
		bullet.overrideExplosionRange = data.overrideExplosionRadius;
		DeathAnimation.MakeDeathForThatFellaYo (bullet, true);

		bullet.targetSystem = new TargetSystem (bullet);
	}

	protected override void SetCollisionLayer (Mine bullet)
	{
		bullet.SetLayerNum(CollisionLayers.GetSpawnedLayer (parent.layerLogic));
		bullet.InitMine (data);
		bullet.priorityMultiplier = 0.5f;
	}

	protected override void AddToMainLoop (Mine b)
	{
		Singleton<Main>.inst.Add2Objects (b);
	}

	protected override bool DestroyOnBoundsTeleport {
		get {
			return false;
		}
	}

	protected override PolygonGameObject.DestructionType SetDestructionType () {
		return PolygonGameObject.DestructionType.eComplete;
	}
}

