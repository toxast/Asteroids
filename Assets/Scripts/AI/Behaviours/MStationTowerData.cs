﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStationTowerData : MSpawnDataBase {
    public RandomFloat size;	
    public RandomInt sidesCount;
    public Color color;
    public PhysicalData physical;
    public RandomFloat rotationSpeed;
    public MGunBaseData gun;

	protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateStationTower(this, layer);
    }
}
