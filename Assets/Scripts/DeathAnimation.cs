﻿using UnityEngine;
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

	List<ParticleSystem> explosions;


	public static void MakeDeathForThatFellaYo(PolygonGameObject g, bool instant = false)
	{
		var config = Singleton<GlobalConfig>.inst;
		DeathAnimation anim;
		Debug.LogWarning (g.polygon.area);
		if(g.polygon.area < 5f)
		{
			int explosionsCount = UnityEngine.Random.Range(1,4);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			anim = new DeathAnimation((instant)? 0f:1f, explosionsCount, explosions, config.smallFinalDeathExplosionEffects);
		}
		else if(g.polygon.area < 50f)
		{
			int explosionsCount = UnityEngine.Random.Range(2,5);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			explosions.AddRange(config.mediumDeathExplosionEffects);
			anim = new DeathAnimation((instant)? 0f:2f, explosionsCount, explosions, config.mediumFinalDeathExplosionEffects);
		}
		else
		{
			int explosionsCount = UnityEngine.Random.Range(4,7);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.mediumDeathExplosionEffects);
			explosions.AddRange(config.largeDeathExplosionEffects);
			anim = new DeathAnimation((instant)? 0f:3f, explosionsCount, explosions, config.largeFinalDeathExplosionEffects);
		}
		g.deathAnimation = anim; //rest in pieces!
	}

	public DeathAnimation(float duration, int explosionsCount, List<ParticleSystem> explosionPrefabs, List<ParticleSystem> finishExplosion)
	{
		this.duration = duration;
		this.explosionsCount = explosionsCount;
		this.explosionPrefabs = explosionPrefabs;
		this.finishExplosions = finishExplosion;
	}

	public void AnimateDeath(PolygonGameObject obj)
	{
		started = true;
		explosions = new List<ParticleSystem> ();
		if(duration > 0)
		{
			for (int k = 0; k < explosionsCount; k++) {
				Debug.LogWarning ("bax");
				int i = UnityEngine.Random.Range(0, explosionPrefabs.Count);
				var e = GameObject.Instantiate(explosionPrefabs[i]) as ParticleSystem;
				e.startDelay = ((float)(k)/explosionPrefabs.Count) * duration;
				e.transform.position = obj.cacheTransform.position - new Vector3(0,0,1);
				var r = 2f*obj.polygon.R/3f;
				e.transform.position += new Vector3(UnityEngine.Random.Range(-r, r),
				                                    UnityEngine.Random.Range(-r, r), 0);
				e.transform.parent = obj.cacheTransform;
				e.Play();
				explosions.Add(e);
			}
		}

		{
			for (int k = 0; k < finishExplosions.Count; k++) {
				var e = GameObject.Instantiate(finishExplosions[k]) as ParticleSystem;
				e.startDelay = duration;
				e.transform.parent = obj.cacheTransform;
				e.transform.localPosition = - new Vector3(0,0,1);
				e.Play();
				explosions.Add(e);
			}
		}
	}

	public void Tick(float delta)
	{
		if(started && !finished)
		{
			duration -= delta;

			if(duration <= 0)
			{
				finished = true;
				if(explosions != null)
				{
					explosions.ForEach (e => 
					{
						e.transform.parent = null;
					});
				}
			}
		}
	}
}
