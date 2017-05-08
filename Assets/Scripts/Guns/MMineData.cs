using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMineData : MSpawnDataBase {
	public MMinesGunData gunRef; 

	protected override PolygonGameObject CreateInternal (int layer) {
		var bullet = PolygonCreator.CreatePolygonGOByMassCenter<Mine>(gunRef.vertices, gunRef.color);
		bullet.gameObject.name = gunRef.name;
		InitPolygonGameObject (bullet, gunRef.physical);
		bullet.InitLifetime (gunRef.lifeTime);
		ObjectsCreator.ApplyDeathData(bullet, gunRef.deathData);
		bullet.damageOnCollision = gunRef.hitDamage;
		bullet.destroyOnBoundsTeleport = false;
		bullet.AddParticles(gunRef.effects);
		bullet.AddDestroyAnimationParticles (gunRef.destructionEffects);
		bullet.SetLayerNum(layer);
		bullet.InitMine (gunRef);
		bullet.priorityMultiplier = 0.1f;
		bullet.showOffScreen = false;
		bullet.rotation += gunRef.rotation.RandomValue;
		return bullet;
	}

	protected void InitPolygonGameObject (Mine bullet, PhysicalData ph) {
		bullet.InitPolygonGameObject (ph);
		if (gunRef.burnDOT.dps > 0 && gunRef.burnDOT.duration > 0) {
			bullet.burnDotData = gunRef.burnDOT;
		}
		if (gunRef.iceData.Initialized()){
			bullet.iceEffectData = gunRef.iceData;
		}
		bullet.targetSystem = new TargetSystem (bullet);
	}

}
