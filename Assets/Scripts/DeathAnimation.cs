using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	public float explosionSize;

	public static void MakeDeathForThatFellaYo(PolygonGameObject go, bool instant = false, float finalExplosionPowerKoeff = 1f)
	{
        var config = Singleton<GlobalConfig>.inst;
		float duration = (instant)? 0 : Mathf.Pow(go.polygon.area, 0.4f) / 4f;
		DeathAnimation anim;
		int minExplosions = (int)(2*Mathf.Pow(go.polygon.R, 0.5f));
		int maxExplosions = (int)(2*Mathf.Pow(go.polygon.R, 0.8f));
		//Debug.LogWarning (minExplosions + " - " + maxExplosions);
		int explosionsCount = UnityEngine.Random.Range(minExplosions,maxExplosions);
		//Debug.LogWarning (g.polygon.area);
		if(go.polygon.area < 5f)
		{
			//int explosionsCount = UnityEngine.Random.Range(1,4);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, explosionsCount, explosions, config.smallFinalDeathExplosionEffects);
		}
		else if(go.polygon.area < 50f)
		{
			//int explosionsCount = UnityEngine.Random.Range(2,5);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			explosions.AddRange(config.mediumDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, explosionsCount, explosions, config.mediumFinalDeathExplosionEffects);
		}
		else
		{
 			//int explosionsCount = UnityEngine.Random.Range(4,7);
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
				int i = UnityEngine.Random.Range(0, explosionPrefabs.Count);
				var e = GameObject.Instantiate(explosionPrefabs[i]) as ParticleSystem;
				e.startDelay = ((float)(k)/explosionPrefabs.Count) * duration;
				e.transform.position = obj.cacheTransform.position - new Vector3(0,0,1);
				var r = 2f*obj.polygon.R/3f;
				e.transform.position += new Vector3(UnityEngine.Random.Range(-r, r),
				                                    UnityEngine.Random.Range(-r, r), 0);
				e.transform.parent = obj.cacheTransform;
				e.Play();
				instantiatedExplosions.Add(e);
			}
		}
	}

    public float GetFinalExplosionRadius() {
        float overrideR = obj.overrideExplosionRange;
        return overrideR >= 0 ? overrideR : (6f * Mathf.Sqrt(obj.polygon.area) * finalExplosionPowerKoeff);
    }


    public void Tick(float delta)
	{
		if(started && !finished)
		{
			duration -= delta;

			if(duration <= 0)
			{
				explosionSize = GetFinalExplosionRadius();
				{
					for (int k = 0; k < finishExplosions.Count; k++) {
						var e = GameObject.Instantiate(finishExplosions[k]) as ParticleSystem;
						e.transform.position = obj.cacheTransform.position - new Vector3(0,0,1);
						e.startSize = explosionSize;
						e.startLifetime =  Mathf.Pow(e.startSize, 0.33f) * 0.5f;
						e.Play();
						instantiatedExplosions.Add(e);
					}
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
}

[System.Serializable]
public class DeathData {
    public bool createExplosionOnDeath = true;
    public bool instantExplosion = false;
    public PolygonGameObject.DestructionType destructionType = PolygonGameObject.DestructionType.eNormal;
    public float overrideExplosionRange = -1;
    public float overrideExplosionDamage = -1;
}

