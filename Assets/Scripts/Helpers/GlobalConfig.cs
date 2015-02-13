using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalConfig : MonoBehaviour{

	[SerializeField] public float GlobalHealthModifier = 1f;
	[SerializeField] public float SpaceshipHealthModifier = 1f;
	[SerializeField] public float TankEnemyHealthModifier = 1f;
	[SerializeField] public float SawEnemyHealthModifier = 1f;
	[SerializeField] public float TowerEnemyHealthModifier = 1f;
	[SerializeField] public float RogueEnemyHealthModifier = 1f;


	[SerializeField] public float DamageFromCollisionsModifier = 0.3f;


	[SerializeField] public Color GasteroidColor = Color.white;
	[SerializeField] public Color spaceshipEnemiesColor = Color.white;

	[SerializeField] public ParticleSystem fireEffect;
	[SerializeField] public ParticleSystem fireEffect2;
	[SerializeField] public ParticleSystem thrusterEffect;

	[SerializeField] public List<ParticleSystem> smallDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> smallFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeFinalDeathExplosionEffects;
}
