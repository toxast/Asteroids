using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCometData : MSpawnData<Asteroid> {
    public EffectType effect;
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;
    public PhysicalData physical;
    public Color color;
    public ParticleSystem particleSystem;
    public override Asteroid Create() {
        var spawn = ObjectsCreator.CreateComet(this);
        return spawn;
    }
}