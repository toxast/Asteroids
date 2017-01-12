using UnityEngine;
using System.Collections;

public class MSawData : MSpawnData<SawEnemy>
{
	public SawInitData mdata;

	public override SawEnemy Create(int layer)
	{
		return ObjectsCreator.CreateSawEnemy(this, layer);
	}
}
