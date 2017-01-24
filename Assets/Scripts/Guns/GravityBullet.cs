using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBullet : PolygonGameObject {

    int affectLayer;
    float range;
    float damagePerSecond;
    float force;
    List<PolygonGameObject> gobjects = new List<PolygonGameObject> ();
	ParticleSystem effect;

    public void InitGravityBullet(int affectLayer, MGravityGunData data) {
		
        this.affectLayer = affectLayer;
        range = data.range;
        damagePerSecond = data.damagePerSecond;
        force = data.force;
        gobjects = Singleton<Main>.inst.gObjects;

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
			new PhExplosion (position, range, checkEvery * damagePerSecond, checkEvery * force, gobjects, affectLayer);
			new PhExplosion (position, range, checkEvery * damagePerSecond * 0.5f, checkEvery * force * 0.5f, Singleton<Main>.inst.bullets, affectLayer);
		}
    }

	public override void HandleDestroying ()
	{
		base.HandleDestroying ();

		Color c = Color.black;
		c.a = 0;
		var holder = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(RocketLauncher.missileVertices, c);
		holder.position = this.position;
		holder.velocity = this.velocity;

		var emission = effect.emission;
		emission.enabled = false;

//		var pos = effect.transform.localPosition;
//		var rot = effect.transform.localRotation;

		effect.transform.SetParent(holder.cacheTransform, true);
//		effect.transform.localPosition = pos;
//		effect.transform.localRotation = rot;
		Singleton<Main>.inst.AddToAlphaDetructor(holder, 5f);

	}
}
