﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEarthSpaceshipData : MSpaceshipData
{
	[Header ("spawned shield")]
	public int elementsCount = 8;
	public float shieldRadius = 20f;
	public float shieldRotationSpeed = 30f;
	public float respawnShieldObjDuration = 3f;
	public float applyShootingForceDuration = 5f;
	public bool shootByArc = false;
	public bool shootClosestByVelocity = true;
	[Header ("shoot intervals")]
	public bool stopWhileShooting = true;
	public float shootInterval = 1f;
	public float shootDuration = 4f;
	public float shootPause = 6f;

	/*[Header ("pause shoouting for some time after x-th attack")]
	public bool useShootPause = false;
	public int pauseAfterAttackNum = 0;
	public float pauseDuration = 0;*/

	public float comformDistanceMax = 60;
	public float comformDistanceMin = 30;

	[Header ("shield from broken elements")]
    public bool collectBrokenObjects = true;
	public float applyForceBrokenObj = 2f;
	public float collectMassThreshold = 30f;
	public float brokenShieldRadius = 20f;
    public int attackWithBrokenWhenCount = 8;
    public int attackWithBrokenCount = 4;
	public float collectBrokenObjectsInterval = 2f;
	public float minDeltaShootBrokenObjects = 1f;

	[Header ("other")]
	public float overrideAttackPartForce = -1;
	public float overrideMaxPartAttackSpeed = -1;
	public float asteroidsStability = 0.5f;
	public MSpawnDataBase spawnObj;
	public List<ParticleSystemsData> asteroidAttackByForceAnimations;
	public List<ParticleSystemsData> asteroidGrabByForceAnimations;


	protected override PolygonGameObject CreateInternal(int layer)
	{
		return ObjectsCreator.CreateEarthSpaceship<SpaceShip>(this, layer);
	}

	[Header("editor field")]
	[SerializeField] MSpaceshipData fillFrom;
	protected override void OnValidate() 
	{
		base.OnValidate ();
		asteroidAttackByForceAnimations.SetDefaultValues ();
		asteroidGrabByForceAnimations.SetDefaultValues ();

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
