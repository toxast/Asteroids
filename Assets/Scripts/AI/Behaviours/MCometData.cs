﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnDataBase, IGotShape {
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;
    public float lifeTime;
    public Color color;
	public PowerupData powerupData;
	public List<ParticleSystemsData> particles;
	public List<ParticleSystemsData> destructionEffects;
	public Vector2[] iverts {get {return powerupData.verts;} set{powerupData.verts = value;}}

	protected override PolygonGameObject CreateInternal(int layer) {
        var spawn = ObjectsCreator.CreateComet(this);
        return spawn;
    }

	protected void OnValidate () {
		particles.SetDefaultValues ();
		destructionEffects.SetDefaultValues ();
	}
}

[System.Serializable]
public class PowerupData {
	public float lifeTime;
	public Vector2[] verts;
	public Color color;
	public UserComboPowerup effectData;
	public Color particleSystemColor;
	public ParticleSystemsData particles { get { return MParticleResources.Instance.powerUpDropParticles.data;} } 
}