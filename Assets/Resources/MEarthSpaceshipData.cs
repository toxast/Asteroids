using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEarthSpaceshipData : MSpaceshipData
{
	public int elementsCount = 8;
	public float shieldRadius = 20f;
	public float shieldRotationSpeed = 30f;
	public float respawnShieldObjDuration = 3f;
	public float shootInterval = 5f;
	public float applyShootingForceDuration = 5f;
	public MAsteroidData asteroidData;
	public List<ParticleSystemsData> asteroidAttackByForceAnimations;
	public List<ParticleSystemsData> asteroidGrabByForceAnimations;

	public override SpaceShip Create(int layer)
	{
		return ObjectsCreator.CreateEarthSpaceship<SpaceShip>(this, layer);
	}

	[Header("editor field")]
	[SerializeField] MSpaceshipData fillFrom;
	private void OnValidate() {
		if (fillFrom != null) {
			System.Type type = fillFrom.GetType();
			Component copy = this;
			System.Reflection.FieldInfo[] fields = type.GetFields(); 
			foreach (System.Reflection.FieldInfo field in fields) {
				field.SetValue(copy, field.GetValue(fillFrom));
			}
			fillFrom = null;
		}
	}
}
