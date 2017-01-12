using UnityEngine;
using System.Collections;

public class MSpikyData : MSpawnData<SpikyAsteroid>
{
	public SpikeShooterInitData mdata;

	public override SpikyAsteroid Create(int layer)
	{
		return ObjectsCreator.CreateSpikyAsteroid(this, layer);
	}
}
