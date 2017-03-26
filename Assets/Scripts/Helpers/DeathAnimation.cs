using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DeathAnimation
{
	public int explosionsCount;
	public float duration = 0f;
	public float timeLeft = 0f;

	public bool started = false;
	private bool instant = false;
	public bool finished = false;
	List<ParticleSystem> explosionPrefabs;
	List<ParticleSystem> finishExplosions;
	public List<ParticleSystem> instantiatedExplosions;
	PolygonGameObject obj;

    //public float finalExplosionPowerKoeff = 1f;
	public float explosionRadius;

	const float DeathAnimationZOffset = -3f;

	public static void MakeDeathForThatFellaYo(PolygonGameObject go, bool instant = false)// float finalExplosionPowerKoeff = 1f)
	{
        var config = Singleton<GlobalConfig>.inst;
		float duration = (instant)? 0 :  Mathf.Pow(go.polygon.area, 0.4f) / 4f;
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
			anim = new DeathAnimation(go, duration, instant, explosionsCount, explosions, config.smallFinalDeathExplosionEffects);
		}
		else if(go.polygon.area < 20f)
		{
			//int explosionsCount = Random.Range(2,5);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.smallDeathExplosionEffects);
			explosions.AddRange(config.mediumDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, instant, explosionsCount, explosions, config.mediumFinalDeathExplosionEffects);
		}
		else
		{
 			//int explosionsCount = Random.Range(4,7);
			List<ParticleSystem> explosions = new List<ParticleSystem>(config.mediumDeathExplosionEffects);
			explosions.AddRange(config.largeDeathExplosionEffects);
			anim = new DeathAnimation(go, duration, instant, explosionsCount, explosions, config.largeFinalDeathExplosionEffects);
		}
		go.deathAnimation = anim; //rest in pieces!
	}

	public DeathAnimation(PolygonGameObject obj, float duration, bool instant, int explosionsCount, List<ParticleSystem> explosionPrefabs, List<ParticleSystem> finishExplosion)//, float finalExplosionPowerKoeff = 1f)
	{
		this.instant = instant;
		this.timeLeft = duration;
		this.duration = duration;
		this.explosionsCount = explosionsCount;
		this.explosionPrefabs = explosionPrefabs;
		this.finishExplosions = finishExplosion;
        //this.finalExplosionPowerKoeff = finalExplosionPowerKoeff;
        this.obj = obj;
    }

	struct DelayedExplosion{
		public float delay;
		public Vector2 localPos;
	}
	List<DelayedExplosion> delayedExplosions = new List<DelayedExplosion>();

	public void AnimateDeath()
	{
		started = true;
		instantiatedExplosions = new List<ParticleSystem> ();
		if(duration > 0)
		{
			for (int k = 0; k < explosionsCount; k++) {
				int i = Random.Range(0, explosionPrefabs.Count);
                var eprefab = explosionPrefabs[i];
                var e = GameObject.Instantiate(explosionPrefabs[i]) as ParticleSystem;
				var emain = e.main;
				emain.startDelayMultiplier = ((float)(k) / explosionPrefabs.Count) * duration;
				emain.duration = duration - emain.startDelayMultiplier;
				obj.globalPolygon.SetTriangles (obj.polygon.GetTriangles ());
				e.transform.position = (Vector3)obj.globalPolygon.GetRandomAreaVertex() + new Vector3(0,0,DeathAnimationZOffset);
				e.transform.parent = obj.cacheTransform;
				Vector2 offset = e.transform.localPosition;
				e.Play();
				instantiatedExplosions.Add(e);
				delayedExplosions.Add (new DelayedExplosion{ delay = emain.startDelayMultiplier, localPos = offset });
            }
		}

		turretsDestructionDelays = new List<float> ();
		foreach(var t in obj.turrets)
		{
			turretsDestructionDelays.Add(Random.Range(0, duration));
		}
	}

	List<float> turretsDestructionDelays = new List<float>();

    public float GetFinalExplosionRadius() {
        float overrideR = obj.overrideExplosionRange;
		return overrideR >= 0 ? overrideR : ExplosionRadius(obj.polygon.area);//* finalExplosionPowerKoeff);
    }

	public static float ExplosionRadius(float area) {
		return 3f * Mathf.Sqrt (area);
	}

	public static float ExplosionDamage(float radius) {
		return 2f * Mathf.Pow (radius, 0.65f);
	}

    public void Tick(float delta)
	{
		if(started && !finished)
		{
			timeLeft -= delta;

			if (!instant) {
				for (int i = delayedExplosions.Count - 1; i >= 0; i--) {
					if (delayedExplosions [i].delay < duration - timeLeft) {
						Vector3 pos = obj.cacheTransform.position + (Vector3)delayedExplosions [i].localPos + new Vector3(0,0,DeathAnimationZOffset);
						CreateBurningPart (0.5f, 1f, 6f, pos, 1f);
						delayedExplosions.RemoveAt (i);
					}
				}
			}

			for (int i = obj.turrets.Count - 1; i >= 0; i--) {
				if (turretsDestructionDelays [i] < duration - timeLeft) {
					var t = obj.turrets [i];
					t.cacheTransform.parent = null;
					t.velocity += obj.velocity;
					t.cacheTransform.position = t.cacheTransform.position.SetZ(obj.cacheTransform.position.z -0.1f);
					t.rotation += UnityEngine.Random.Range(-150f, 150f);
					bool instantTurrentExplosion = Math2d.Chance (0.6f);
					DeathAnimation.MakeDeathForThatFellaYo (t, instantTurrentExplosion);
					Singleton<Main>.inst.Add2Objects (t);
					t.Kill ();
					obj.turrets.RemoveAt (i);
					turretsDestructionDelays.RemoveAt (i);
				}
			}

			if(timeLeft <= 0)
			{
                explosionRadius = GetFinalExplosionRadius();
                float lifetime = Mathf.Pow(explosionRadius, 0.33f) * 0.5f;

				{
					for (int k = 0; k < finishExplosions.Count; k++) {
						var e = GameObject.Instantiate(finishExplosions[k]) as ParticleSystem;
                        var emain = e.main;
                        emain.startSizeMultiplier = 2 * explosionRadius;
                        emain.startLifetimeMultiplier = lifetime;
						e.transform.position = obj.cacheTransform.position + new Vector3(0,0,DeathAnimationZOffset);
						e.Play();
						instantiatedExplosions.Add(e);
					}
				}

				if (!instant) {
					int minExplosions = (int)(3 * Mathf.Pow (obj.polygon.R, 0.3f));
					int maxExplosions = (int)(3 * Mathf.Pow (obj.polygon.R, 0.7f));
					int explosionsCount = Random.Range (minExplosions, maxExplosions);
					for (int i = 0; i < explosionsCount; i++) {
						float animDuration = Random.Range (0.7f, 1.2f) * lifetime;
						float r = Random.Range (1f, obj.polygon.R / 3f);
						var speed = Random.Range (0.5f, 1.5f) * explosionRadius / animDuration;
						var pos = obj.cacheTransform.position + new Vector3(0,0,DeathAnimationZOffset);
						CreateBurningPart (animDuration, r, speed, pos, 0.3f);
					}
				}

				if (obj.turrets.Count > 0) {
					Debug.LogError ("turrets left after death");
					obj.turrets.Clear ();
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

	private void CreateBurningPart(float duration, float r, float speed, Vector3 pos, float objSpeedMultipier = 0) {
        int vcount = 4;
        Color b = Color.black;
        Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(r, vcount);
        var firepart = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(vertices, b);
        firepart.SetAlpha(0);
		firepart.name = "burning part";

        var eprefab = Singleton<GlobalConfig>.inst.burningParticleEffect;
        var eff = GameObject.Instantiate(eprefab) as ParticleSystem;
		var pmain = eff.main;
		pmain.duration = duration;
		pmain.startSizeMultiplier = 2 * r;
        eff.transform.parent = firepart.cacheTransform;
        eff.transform.localPosition = new Vector3(0, 0, -1);
		eff.Play ();

        var controller = new StaticInputController();
        controller.turnDirection = Math2d.RotateVertex(new Vector2(1, 0), Random.Range(0f, 6f));
        //controller.accelerating = true;
        controller.braking = true;
        firepart.SetController(controller);

        var spData = new SpaceshipData();
        //spData.turnSpeed = Random.Range(10, 30);
        spData.maxSpeed = speed;
		spData.brake = 0.8f * speed / duration;
		firepart.InitPolygonGameObject (new PhysicalData ());
        firepart.InitSpaceShip(spData);
        var VelocityDir = Math2d.RotateVertex(new Vector2(1, 0), Random.Range(0f, 6f));
		firepart.velocity = speed * VelocityDir + obj.velocity * objSpeedMultipier;
        firepart.cacheTransform.right = firepart.velocity.normalized;
        firepart.position = pos;
		Singleton<Main>.inst.AddToDestructor(firepart,pmain.duration + 4f, lowerAlphaTo0:false);
    }
}

[System.Serializable]
public class DeathData {
	public PolygonGameObject.DestructionType destructionType = PolygonGameObject.DestructionType.eNormal;
	public bool createExplosionOnDeath = false;
    public bool instantExplosion = false;
    public float overrideExplosionRange = -1;
    public float overrideExplosionDamage = -1;
}

