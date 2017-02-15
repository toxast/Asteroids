using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawnDataBase : MonoBehaviour, ISpawnable {
	[Header("editor fields")]
	public int spawnCount = 1;
	public bool spawn = true;
	public int editorSpawnLayer = CollisionLayers.ilayerTeamEnemies;
	[Space(20)]
	[Header("game fields")]
	public float difficulty = 10f;
	public int gameSpawnLayer = CollisionLayers.ilayerTeamEnemies;

	#if UNITY_EDITOR
	void Update() {
		if (spawn && Application.isPlaying && Application.isEditor) {
			spawn = false;
			for (int i = 0; i < spawnCount; i++) {
				Spawn ();
			}
		}
	}
	#endif

	public PolygonGameObject Create (){
		return Create (gameSpawnLayer);
	}

	public virtual PolygonGameObject Create (int layer){return null;}

	private void Spawn() {
		var obj = Create (editorSpawnLayer);

		if(obj.layerNum != CollisionLayers.ilayerAsteroids)
			obj.targetSystem = new TargetSystem (obj);

		Vector2 pos;
		float lookAngle;
		SpawnPositioning positioning = new SpawnPositioning ();
		Singleton<Main>.inst.GetRandomPosition(new Vector2(40, 50), positioning, out pos, out lookAngle);
		obj.cacheTransform.position = pos;
		obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);
		Singleton<Main>.inst.Add2Objects(obj);
	}
}

public interface ISpawnable {
	PolygonGameObject Create ();
	PolygonGameObject Create (int layer);
}