using UnityEngine;
using System.Collections;

public class MAsteroidData : MSpawnData<Asteroid>
{
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;

	public MAsteroidCommonData commonData;

	public override Asteroid Create(int layer)
	{
        var spawn = ObjectsCreator.CreateAsteroid (this);
        Singleton<Main>.inst.CreateDropForObject(spawn, commonData); 
        return spawn;
	}
}
