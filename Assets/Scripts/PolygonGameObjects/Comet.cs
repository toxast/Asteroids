using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : Asteroid {

    EffectType powerupEffect;

    public void InitComet(EffectType powerupEffect, float lifeTime) {
        this.powerupEffect = powerupEffect;
        InitLifetime(lifeTime);
    }

    public override void OnDestroing() {
        Singleton<Main>.inst.CreatePowerUp(powerupEffect, this.position);
    }
}
