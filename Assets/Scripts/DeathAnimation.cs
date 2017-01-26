﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DeathAnimation
{
	public int explosionsCount;
	public float duration = 0f;
	public bool started = false;
	//private bool instant = false;
	public bool finished = false;
	List<ParticleSystem> explosionPrefabs;
	List<ParticleSystem> finishExplosions;
	public List<ParticleSystem> instantiatedExplosions;
	PolygonGameObject obj;

    public float finalExplosionPowerKoeff = 1f;
	public float explosionRadius;

	public static void MakeDeathForThatFellaYo(PolygonGameObject go, bool instant = false, float finalExplosionPowerKoeff = 1f)
	{
        var config = Singleton<GlobalConfig>.inst;
		float duration = (instant)? 0 : Mathf.Pow(go.polygon.area, 0.4f) / 4f;
		DeathAnimation anim;
		int minExplosions = (int)(2*Mathf.Pow(go.polygon.R, 0.5f));
		int maxExplosions = (int)(2*Mathf.Pow(go.polygon.R, 0.8f));
		//Debug.LogWarning (minExplosions + " - " + maxExplosions);
		int explosionsCount = Random.Range(minExplosions,maxExplosions);
		//Debug.LogWarning (g.polygon.area);
		if(go.polygon.area < 10f)
		{
			//int explosionsCount = Random.Range(1,4);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, explosionsCount, explosions, config.smallFinalDeathExplosionEffects);
		}
		else if(go.polygon.area < 20f)
		{
			//int explosionsCount = Random.Range(2,5);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			explosions.AddRange(config.mediumDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, explosionsCount, explosions, config.mediumFinalDeathExplosionEffects);
		}
		else
		{
 			//int explosionsCount = Random.Range(4,7);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.mediumDeathExplosionEffects);
			explosions.AddRange(config.largeDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, explosionsCount, explosions, config.largeFinalDeathExplosionEffects);
		}
		go.deathAnimation = anim; //rest in pieces!
	}

	public DeathAnimation(PolygonGameObject obj, float duration, int explosionsCount, List<ParticleSystem> explosionPrefabs, List<ParticleSystem> finishExplosion, float finalExplosionPowerKoeff = 1f)
	{
		this.duration = duration;
		this.explosionsCount = explosionsCount;
		this.explosionPrefabs = explosionPrefabs;
		this.finishExplosions = finishExplosion;
        this.finalExplosionPowerKoeff = finalExplosionPowerKoeff;
        this.obj = obj;
    }

	public void AnimateDeath()
	{
		started = true;
		instantiatedExplosions = new List<ParticleSystem> ();
		if(duration > 0)
		{
			for (int k = 0; k < explosionsCount; k++) {
				int i = Random.Range(0, explosionPrefabs.Count);
                var eprefab = explosionPrefabs[i];
                var pMain = eprefab.main;
                pMain.startDelayMultiplier = ((float)(k) / explosionPrefabs.Count) * duration;
                pMain.duration = duration - pMain.startDelayMultiplier;
                var e = GameObject.Instantiate(explosionPrefabs[i]) as ParticleSystem;
				e.transform.position = obj.cacheTransform.position - new Vector3(0,0,1);
				var r = 2f*obj.polygon.R/3f;
				e.transform.position += new Vector3(Random.Range(-r, r),
				                                    Random.Range(-r, r), 0);
				e.transform.parent = obj.cacheTransform;
				e.Play();
				instantiatedExplosions.Add(e);

                CreateBurningPart(0.5f, 1f, 6f, e.transform.position, pMain.startDelayMultiplier);
            }
		}
	}

    public float GetFinalExplosionRadius() {
        float overrideR = obj.overrideExplosionRange;
        return overrideR >= 0 ? overrideR : (3f * Mathf.Sqrt(obj.polygon.area) * finalExplosionPowerKoeff);
    }


    public void Tick(float delta)
	{
		if(started && !finished)
		{
			duration -= delta;

			if(duration <= 0)
			{
                explosionRadius = GetFinalExplosionRadius();
                float lifetime = Mathf.Pow(explosionRadius, 0.33f) * 0.5f;

				{
					for (int k = 0; k < finishExplosions.Count; k++) {
						var e = GameObject.Instantiate(finishExplosions[k]) as ParticleSystem;
                        var emain = e.main;
                        emain.startSizeMultiplier = 2 * explosionRadius;
                        emain.startLifetimeMultiplier = lifetime;
                        e.transform.position = obj.cacheTransform.position - new Vector3(0,0,1);
						e.Play();
						instantiatedExplosions.Add(e);
					}
				}

                int minExplosions = (int)(3 * Mathf.Pow(obj.polygon.R, 0.3f));
                int maxExplosions = (int)(3 * Mathf.Pow(obj.polygon.R, 0.7f));
                int explosionsCount = Random.Range(minExplosions, maxExplosions);
                for (int i = 0; i < explosionsCount; i++) {
                    float duration = lifetime + Random.Range(-0.3f, 1.5f);
                    float r = Random.Range(1f, obj.polygon.R / 3f);
                    var speed = 0.5f * explosionRadius / duration;
                    var pos = obj.cacheTransform.position - new Vector3(0, 0, 1);
                    CreateBurningPart(duration, r, speed, pos, 0f);
                }

                finished = true;
				if(instantiatedExplosions != null)
				{
					instantiatedExplosions.ForEach (e => 
					{
						e.transform.parent = null;
					});
				}
			}
		}
	}

    private void CreateBurningPart(float duration, float r, float speed, Vector3 pos, float delay) {
        int vcount = 4;
        Color b = Color.black;
        Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(r, vcount);
        var firepart = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(vertices, b);
        firepart.SetAlpha(0);
        var eprefab = Singleton<GlobalConfig>.inst.burningParticleEffect;
        var pmain = eprefab.main;
        pmain.duration = duration;
        pmain.startSizeMultiplier = 2 * r;
        pmain.startDelayMultiplier = delay;
        var eff = GameObject.Instantiate(eprefab) as ParticleSystem;
        eff.transform.parent = firepart.cacheTransform;
        eff.transform.localPosition = new Vector3(0, 0, -1);

        var controller = new StaticInputController();
        controller.turnDirection = Math2d.RotateVertex(new Vector2(1, 0), Random.Range(0f, 6f));
        //controller.accelerating = true;
        controller.braking = true;
        firepart.SetController(controller);

        var spData = new SpaceshipData();
        spData.turnSpeed = Random.Range(10, 30);
        spData.maxSpeed = speed;
        spData.brake = 4f;
        firepart.InitSpaceShip(new PhysicalData(), spData);
        var VelocityDir = Math2d.RotateVertex(new Vector2(1, 0), Random.Range(0f, 6f));
        firepart.velocity = speed * VelocityDir + obj.velocity;
        firepart.cacheTransform.right = firepart.velocity.normalized;
        firepart.position = pos;
        Singleton<Main>.inst.AddToAlphaDetructor(firepart, pmain.duration + 1f);
    }
}

[System.Serializable]
public class DeathData {
    public bool createExplosionOnDeath = true;
    public bool instantExplosion = false;
    public PolygonGameObject.DestructionType destructionType = PolygonGameObject.DestructionType.eNormal;
    public float overrideExplosionRange = -1;
    public float overrideExplosionDamage = -1;
}

