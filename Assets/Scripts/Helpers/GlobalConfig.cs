using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GlobalConfig : MonoBehaviour
{
	public static float DamageFromCollisionsModifier = 0.013f;

	[SerializeField] public Color AsteroidColor = Color.gray;
	[SerializeField] public Color GasteroidColor = Color.white;

	[SerializeField] public List<ParticleSystem> smallDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> smallFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeFinalDeathExplosionEffects;

    [SerializeField] public ParticleSystem burningParticleEffect;
}
