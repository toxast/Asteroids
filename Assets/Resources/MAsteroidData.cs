using UnityEngine;
using System.Collections;

public class MAsteroidData : MSpawnData<Asteroid>
{
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;

	public MAsteroidCommonData commonData;

	public override Asteroid Create()
	{
		return ObjectsCreator.CreateAsteroid (this);
	}
}
