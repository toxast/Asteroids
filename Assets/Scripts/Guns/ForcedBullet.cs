using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForcedBullet : PolygonGameObject {

    int affectLayer;
    Vector2 forceDir;
    MForcedBulletGun data;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject>();
	List<ParticleSystemsData> effects2;

    public void InitForcedBullet(MForcedBulletGun data, int affectLayer) {
        this.data = data;
        this.affectLayer = affectLayer;
        forceDir = Math2d.RandomSign() * Math2d.MakeRight(cacheTransform.right);
        gobjects = Singleton<Main>.inst.gObjects;
        StartCoroutine(ForceChange());
        effects2 = data.effectsForcedBullet.ConvertAll(e => e.Clone());
        effects2.ForEach(e => e.overrideSize = 2 * data.range);
		SetParticles (effects2);
    }

    const float checkEvery = 0.18f;
    float timeLeftForCheck = checkEvery;
    public override void Tick(float delta) {
        base.Tick(delta);
        velocity += forceDir * delta * data.force;
        timeLeftForCheck -= delta;
        if (timeLeftForCheck < 0) {
            timeLeftForCheck += checkEvery;
            TickEvery(checkEvery);
        }
    }

    IEnumerator ForceChange() {
        yield return new WaitForSeconds(data.forceDuration / Mathf.Sqrt(2));
        forceDir = -forceDir;
        yield return new WaitForSeconds(data.forceDuration * ( 1f + 1f / Mathf.Sqrt(2)));
        while (true) {
            forceDir = -forceDir;
            yield return new WaitForSeconds(2f * data.forceDuration);
        }
    }

    private void TickEvery(float sec) {
		if (iceEffectData.Initialized()) {
			new IceWave (position, data.range, iceEffectData, checkEvery, gobjects, affectLayer);
		}
		new PhExplosion(position, data.range, sec * data.damagePerSecond, sec * data.fieldForce, gobjects, affectLayer);
    }
}
