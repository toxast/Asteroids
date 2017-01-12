using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnData<Asteroid> {
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;
    public float lifeTime;
    public Color color;
    public ParticleSystem particles;
    public Color particleSystemColor;
    public PowerupData powerupData;
    public override Asteroid Create(int layer) {
        var spawn = ObjectsCreator.CreateComet(this);
        return spawn;
    }
}

[System.Serializable]
public class PowerupData {
    public EffectType effect;
    public float lifeTime;
    public Color color;
    public ParticleSystem particles;
    public Color particleSystemColor;
}