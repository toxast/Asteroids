using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGasteroidData : MSpawnData<Gasteroid>
{
    public RandomFloat speed;
    public RandomFloat rotation;
    public RandomFloat size;

    public PhysicalData physical;
//    public MAsteroidCommonData commonData;

    public override Gasteroid Create()
    {
        var spawn = ObjectsCreator.CreateGasteroid (this);
        return spawn;
    }
}
