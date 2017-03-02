using UnityEngine;
using System.Collections;

public class MSpawnData<T> : MSpawnDataBase
	where T: PolygonGameObject
{
	public virtual T CreateByType (int layer){return Create(gameSpawnLayer) as T;}
}
