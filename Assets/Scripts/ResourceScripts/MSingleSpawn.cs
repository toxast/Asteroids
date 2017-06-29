using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MSingleSpawn : MSpawnBase {
	public abstract CollisionLayers.eLayerNum iGameSpawnLayer{ get;}
	public abstract TeleportData iTeleportData{ get;}
	protected virtual PolygonGameObject CreateInternal (int layer){return null;}
	public virtual PolygonGameObject Create(){
		return Create((int)iGameSpawnLayer);
	}
	public abstract PolygonGameObject Create (int layer);

	public override void Spawn (PositionData posData, Action<SpawnedObj> callback)
	{
		var main = Singleton<Main>.inst;
		main.StartCoroutine (SpawnRoutine (this, posData, new Place(), callback));
	}
}
