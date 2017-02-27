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
	float forceMultipier;

	public void InitGravityBullet(int affectLayer, MGravityGunData data, float forceMultipier = 1) {
        this.affectLayer = affectLayer;
        range = data.range;
        damagePerSecond = data.damagePerSecond;
		force = data.force * forceMultipier;
        gobjects = Singleton<Main>.inst.gObjects;
        bullets = Singleton<Main>.inst.bullets;
        //effect
        effect = GameObject.Instantiate (data.bulletEffect, transform);
		var shape = effect.shape;
		shape.radius = data.range;
		var main = effect.main;
		main.startSpeed = -data.range * 2;
		var em = effect.emission;
		em.rateOverTime = Mathf.PI * data.range * data.range / 2.5f;
		effect.transform.localPosition = new Vector3 (0, 0, -1);

		InitLifetime (data.lifeTime);
		this.SetAlpha (0);
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

	public override void HandleDestroying ()
	{
		base.HandleDestroying ();

		Color c = Color.black;
		c.a = 0;
		var verts = PolygonCreator.CreatePerfectPolygonVertices (1, 4);
		var holder = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(verts, c);
		holder.position = this.position;
		holder.velocity = this.velocity;

		var emission = effect.emission;
		emission.enabled = false;

//		var pos = effect.transform.localPosition;
//		var rot = effect.transform.localRotation;

		effect.transform.SetParent(holder.cacheTransform, true);
//		effect.transform.localPosition = pos;
//		effect.transform.localRotation = rot;
		Singleton<Main>.inst.AddToDetructor(holder, 5f);

	}
}
