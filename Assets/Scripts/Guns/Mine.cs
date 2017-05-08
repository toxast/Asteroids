using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mine : PolygonGameObject {
	MMinesGunData data;
	float deceleration;
	//float activateRangeSqr;
	bool setForDestruction = false;
	AIHelper.MyTimer destructionTimer;
	float flowMsxSpeedSqr;

	public void InitMine(MMinesGunData data) {
		this.data = data;
		deceleration = data.deceleration.RandomValue;
		//activateRangeSqr = data.activateRange * data.activateRange;
		flowMsxSpeedSqr =  data.flowMaxSpeed *  data.flowMaxSpeed;
	}

	public override void Tick (float delta)	{
		//check if expired this tick
		bool wasExpired = Expired ();
		base.Tick (delta);
		if (!data.explodeOnExpire && !wasExpired && Expired () && !setForDestruction) {
			deathAnimation = null;
		}

		TickDestructionLogic (delta);

		if (!TickFlowLogic (delta)) {
			Brake(delta, deceleration);
		}
	}

	//kill if someone is in range
	private void TickDestructionLogic(float delta) {
		int enemyLayers = CollisionLayers.GetEnemyLayers (layerLogic);
		if (!setForDestruction && HasObjectInRange(enemyLayers, data.activateRange) != null) {
			setForDestruction = true;
			destructionTimer = new AIHelper.MyTimer (data.timerDuration, () => Kill ());
			AddParticles (new List<ParticleSystemsData>{data.triggetExplosionEffect });
		}

		if (destructionTimer != null) {
			destructionTimer.Tick (delta);
		}
	}

	//flow to someone if in data.flowRange
	//returns true if flowing
	private bool TickFlowLogic(float delta) {
		int enemyLayers = CollisionLayers.GetEnemyLayers (layerLogic);
		if (data.useFlowTowerdsTargetBeh == false) {
			return false;
		}

		if (target != null && !IsInRange(target, data.flowRange)) {
			SetTarget(null);
		}

		if (target == null) {
			SetTarget(HasObjectInRange (enemyLayers, data.flowRange));
		}

		if (target != null) {
			Accelerate (delta, data.flowAcceleration, data.flowStability, data.flowMaxSpeed, flowMsxSpeedSqr, (target.position - position).normalized);
			return true;
		}

		return false;
	}

	private PolygonGameObject HasObjectInRange(int enemylayer, float range) {
		var gobjects = Singleton<Main>.inst.gObjects;
		for (int i = 0; i < gobjects.Count; i++) {
			var obj = gobjects [i];
			if ((enemylayer & obj.layerLogic) != 0 && !obj.IsInvisible()) {
				if (IsInRange(obj, range)) {
					return obj;				
				}
			}
		}

		return null;
	}

	private bool IsInRange(PolygonGameObject obj, float range) {
		return (obj.position - position).magnitude - (obj.polygon.R / 2f) < range;
	}
}