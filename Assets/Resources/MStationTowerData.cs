using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStationTowerData : MSpawnData<TowerEnemy> {
    public RandomFloat size;	
    public RandomInt sidesCount;
    public Color color;
    public PhysicalData physical;
    public RandomFloat rotationSpeed;
    public MGunBaseData gun;

    public override TowerEnemy Create(int layer)
    {
        return ObjectsCreator.CreateStationTower(this, layer);
    }
}
