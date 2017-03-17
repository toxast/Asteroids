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
        bullets = Singleton<Main>.inst.bullets;

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
			new GravityForceExplosion(position, range, checkEvery * damagePerSecond, checkEvery * force, gobjects, affectLayer);
			new GravityForceExplosion(position, range, checkEvery * damagePerSecond * 0.5f, checkEvery * force, bullets, affectLayer);
		}
    }
}
