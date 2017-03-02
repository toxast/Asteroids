using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RocketLauncher : BulletGun<SpaceShip>
{
	List<ParticleSystemsData> thrusters;

	new MRocketGunData data;

	public RocketLauncher(Place place, MRocketGunData data, PolygonGameObject parent)
		:base(place, data, parent)
	{ 
		this.data = data;
		range = data.missleParameters.maxSpeed * data.lifeTime;
        thrusters = data.thrusters.Clone();
	}

	float range;
	public override float Range	{
		get{return range;}
	}

	public override float BulletSpeedForAim{ get { return data.missleParameters.maxSpeed; } } //TODO?

	protected override void InitPolygonGameObject (SpaceShip bullet, PhysicalData ph) {
		bullet.InitSpaceShip(ph, data.missleParameters); 

		if (CreateExplosion ()) {
			bullet.overrideExplosionDamage = data.overrideExplosionDamage; 
			bullet.overrideExplosionRange = data.overrideExplosionRadius;
			DeathAnimation.MakeDeathForThatFellaYo (bullet, true);
		}

		if(thrusters != null) {
			bullet.SetThrusters (thrusters);
		}

		var controller = new MissileController (bullet, data.accuracy);
		bullet.SetController (controller);
		bullet.targetSystem = new MissileTargetSystem (bullet);

	}
	protected override PolygonGameObject.DestructionType SetDestructionType () {
		return PolygonGameObject.DestructionType.eDisappear;
	}

	protected virtual bool CreateExplosion() {
		return true;
	}

	protected override Vector2 GetDirectionNormalized (SpaceShip bullet) {
		var dir = base.GetDirectionNormalized (bullet);
		if (data.launchDirection != Vector2.zero) {
			float angle = Math2d.GetRotationRad (dir);
			var byPlace = Math2d.RotateVertex (data.launchDirection, angle);
			return byPlace.normalized;
		} else {
			return dir;
		}
	}
}
