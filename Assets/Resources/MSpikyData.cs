using UnityEngine;
using System.Collections;

public class MSpikyData : MSpawnDataBase
{
	public SpikeShooterInitData mdata;

	public override PolygonGameObject Create(int layer)
	{
		return ObjectsCreator.CreateSpikyAsteroid(this, layer);
	}
}
