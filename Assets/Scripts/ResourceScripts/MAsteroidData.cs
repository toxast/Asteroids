using UnityEngine;
using System.Collections;

public class MAsteroidData : MSpawnDataBase
{
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;

	public MAsteroidCommonData commonData;

	protected override PolygonGameObject CreateInternal(int layer)
	{
        var spawn = ObjectsCreator.CreateAsteroid (this);
        Singleton<Main>.inst.CreateDropForObject(spawn, commonData); 
        return spawn;
	}
}


