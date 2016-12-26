using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : Asteroid {
    PowerupData data;
    public void InitComet(PowerupData data, float lifeTime) {
        this.data = data;
        InitLifetime(lifeTime);
    }

    public override void OnDestroing() {
        if (leftlifeTime > 0) {
            Singleton<Main>.inst.CreatePowerUp(data, this.position);
        } else {

        }
    }
}
