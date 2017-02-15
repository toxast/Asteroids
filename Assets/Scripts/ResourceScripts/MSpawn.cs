using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawn : MonoBehaviour {
	public MSpawnDataBase prefab;
	public Vector2 spawnRange = new Vector2(40,80);
	public float spawnInterval = 0;
	public float teleportDuration = 1.5f;
	public float teleportRingSize = 10f;
	public Color teleportationColor = new Color (1, 174f / 255f, 0);
	public SpawnPositioning positioning;

	public float difficulty {
		get{ return prefab.difficulty;}
	}


	public PolygonGameObject Spawn(Vector2 pos, float lookAngle)
	{
		PolygonGameObject obj = null;
		var main = Singleton<Main>.inst;
		if (prefab != null) {
			var mdata  = prefab as ISpawnable;
			if (mdata != null) {
				obj = mdata.Create ();
			}
		}

		if (obj == null) {
			Debug.LogError ("null spawn");
		} else {
			if (obj.layerNum != CollisionLayers.ilayerAsteroids) {
				obj.targetSystem = new TargetSystem (obj);
			}
			obj.cacheTransform.position = pos;
			obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);
			Singleton<Main>.inst.Add2Objects (obj);
		}

		return obj;
	}
}
