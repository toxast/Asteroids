using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGasteroidData : MSpawnDataBase
{
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;

    public PhysicalData physical;
//    public MAsteroidCommonData commonData;

	public override PolygonGameObject Create(int layer)
    {
        var spawn = ObjectsCreator.CreateGasteroid (this);
        return spawn;
    }
}
