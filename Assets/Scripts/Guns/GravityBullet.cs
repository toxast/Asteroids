using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBullet :PolygonGameObject {

    int affectLayer;
    float range;
    float damagePerSecond;
    float force;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject> ();
    public void InitGravityBullet(int affectLayer, MGravityGunData data) {
        this.affectLayer = affectLayer;
        range = data.range;
        damagePerSecond = data.damagePerSecond;
        force = data.force;
        gobjects = Singleton<Main>.inst.gObjects;
    }

	const float checkEvery = 0.16f;
	float timeLeftForCheck = checkEvery;
	//TODO: every x/second
    public override void Tick (float delta)
    {
        base.Tick (delta);

		timeLeftForCheck -= delta;
		if (timeLeftForCheck < 0) {
			timeLeftForCheck += checkEvery;
			new PhExplosion (position, range, checkEvery * damagePerSecond, checkEvery * force, gobjects, affectLayer);
			//new PhExplosion (position, range, 0, 							checkEvery * force, Singleton<Main>.inst.bullets, affectLayer);
		}
    }
}
