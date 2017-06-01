using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSawBossData : MSawData {

    public RotatingObjectsShield.Data generatorsShieldData;
    public MSpawnDataBase chargeSpawn;

    protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateSawBossEnemy(this, layer);
    }
}
