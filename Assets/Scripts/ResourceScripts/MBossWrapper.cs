using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MBossWrapper : MSingleSpawn{
	[SerializeField] MSingleSpawn bossElem;

	void OnValidate(){
		if (bossElem == null) {
			Debug.LogError (name + " boss elem null");
		}
		if (this == bossElem) {
			Debug.LogError (name + " this == bossElem infinite loop");
		}
	}

	public override int sdifficulty {
		get {
			return bossElem.sdifficulty;
		}
	}
	public override CollisionLayers.eLayerNum iGameSpawnLayer {
		get {
			return bossElem.iGameSpawnLayer;
		}
	}
	public override TeleportData iTeleportData {
		get {
			return bossElem.iTeleportData;
		}
	}

	public override PolygonGameObject Create (int layer)
	{
		var obj = bossElem.Create (layer);
		if (obj != null) {
			obj.isBossObject = true;
		}
		return obj;
	}

}
