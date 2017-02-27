using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnDataBase {
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;
    public float lifeTime;
    public Color color;
    public ParticleSystem particles;
    public Color particleSystemColor;
    public PowerupData powerupData;
	public override PolygonGameObject Create(int layer) {
        var spawn = ObjectsCreator.CreateComet(this);
        return spawn;
    }
}

[System.Serializable]
public class PowerupData {
    public PowerUpEffect effect;
    public float lifeTime;
    public Color color;
    public ParticleSystem particles;
    public Color particleSystemColor;
}