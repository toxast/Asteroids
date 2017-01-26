using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GlobalConfig : MonoBehaviour
{
	[SerializeField] public float DamageFromCollisionsModifier = 0.3f;

	[SerializeField] public float ExplosionDamageKff = 4f;

	[SerializeField] public Color AsteroidColor = Color.gray;
	[SerializeField] public Color GasteroidColor = Color.white;
	[SerializeField] public Color spaceshipEnemiesColor = Color.white;
	[SerializeField] public Color towerEnemiesColor = Color.white;

	[SerializeField] public ParticleSystem fireEffect;
	[SerializeField] public ParticleSystem fireEffect2;
	[SerializeField] public ParticleSystem thrusterEffect;

	[SerializeField] public List<ParticleSystem> smallDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> smallFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeFinalDeathExplosionEffects;

    [SerializeField] public ParticleSystem burningParticleEffect;
}
