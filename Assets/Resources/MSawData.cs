using UnityEngine;
using System.Collections;

public class MSawData : MSpawnDataBase
{
	public SawInitData mdata;

	public override PolygonGameObject Create(int layer)
	{
		return ObjectsCreator.CreateSawEnemy(this, layer);
	}
}
