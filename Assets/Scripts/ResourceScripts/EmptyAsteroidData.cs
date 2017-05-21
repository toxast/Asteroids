using UnityEngine;
using System.Collections;

public class EmptyAsteroidData : MSpawnDataBase
{
	public Color color = Color.white;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
	public PhysicalData physical;

	protected override PolygonGameObject CreateInternal(int layer)
	{
		var spawn = ObjectsCreator.CreateEmptyAsteroid (this);
		return spawn;
	}
}
