using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : PolygonGameObject {
	MMinesGunData data;
	float deceleration;
	//float activateRangeSqr;
	bool setForDestruction = false;
	AIHelper.MyTimer timer;

	public void InitMine(MMinesGunData data) {
		this.data = data;
		deceleration = data.deceleration.RandomValue;
		//activateRangeSqr = data.activateRange * data.activateRange;
	}

	public override void Tick (float delta)	{
		bool wasExpired = Expired ();
		base.Tick (delta);
		if (!data.explodeOnExpire && !wasExpired && Expired () && !setForDestruction) {
			deathAnimation = null;
		}
		int enemyLayers = CollisionLayers.GetEnemyLayers (layerLogic);
		Brake(delta, deceleration);
		if (!setForDestruction && HasObjectInRange(enemyLayers, data.activateRange)) {
			setForDestruction = true;
			timer = new AIHelper.MyTimer (data.timerDuration, () => Kill ());
			SetParticles (new List<ParticleSystemsData>{data.triggetExplosionEffect });
		}
		if (timer != null) {
			timer.Tick (delta);
		}
	}

	private bool HasObjectInRange(int enemylayer, float range) {
		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++) {
			var obj = gobjects [i];
			if ((enemylayer & obj.layerLogic) != 0 && !obj.IsInvisible()) {
				if ((obj.position - position).magnitude - (obj.polygon.R / 2f) < range) {
					return true;				
				}
			}
		}

		return false;
	}
}