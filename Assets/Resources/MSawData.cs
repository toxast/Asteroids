using UnityEngine;
using System.Collections;

public class MSawData : MSpawnData<SawEnemy>
{
	public SawInitData mdata;

	public override SawEnemy Create()
	{
		return ObjectsCreator.CreateSawEnemy(this);
	}
}
