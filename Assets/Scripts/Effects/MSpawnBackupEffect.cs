using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MSpawnBackupEffect: MEffectData {
	public SpawnBackupEffect.Data data;
	public override IHasProgress Apply (PolygonGameObject picker) {
		return data.Apply (picker);
	}
}
