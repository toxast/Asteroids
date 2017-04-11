using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBullet : PolygonGameObject {

    int affectLayer;
    float range;
    float damagePerSecond;
    float force;
    List<PolygonGameObject> gobjects;
    List<PolygonGameObject> bullets;
    ParticleSystem effect;

	public void InitGravityBullet(int affectLayer, MGravityGunData data, float rangeMultiplier = 1) {
        this.affectLayer = affectLayer;
		force = data.force;
		range = data.range * rangeMultiplier;
		damagePerSecond = data.hitDamage;
        gobjects = Singleton<Main>.inst.gObjects;
		bullets = Singleton<Main>.inst.pBullets;

		var clone = data.gravityEffect.Clone ();
		var effect = AddParticles (new List<ParticleSystemsData>{clone})[0];
		var shape = effect.shape;
		shape.radius = data.range;
		var main = effect.main;
		main.startSpeed = -data.range * 1.6f;
		main.startSizeMultiplier = Mathf.Pow(data.range, 0.4f);
    }

	const float checkEvery = 0.16f;
	float timeLeftForCheck = checkEvery;
    public override void Tick (float delta)
    {
        base.Tick (delta);
		timeLeftForCheck -= delta;
		if (timeLeftForCheck < 0) {
			timeLeftForCheck += checkEvery;

			var objectsAroundData = ExplosionData.CollectData (position, range, gobjects, affectLayer);
			new ForceExplosion (objectsAroundData, position, checkEvery * force);
			new DamageExplosion (objectsAroundData, position, checkEvery * damagePerSecond);

			var bulletsAroundData = ExplosionData.CollectData (position, range, bullets, affectLayer);
			new ForceExplosion (bulletsAroundData, position, checkEvery * force);
			new DamageExplosion (bulletsAroundData, position, checkEvery * damagePerSecond * 0.2f);
		}
    }
}
