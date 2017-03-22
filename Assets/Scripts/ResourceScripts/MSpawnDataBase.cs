using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MSpawnDataBase : MSpawnBase {
	[Header("editor fields")]
	public int spawnCount = 1;
	public bool spawn = true;
	public int editorSpawnLayer = CollisionLayers.ilayerTeamEnemies;
	[Space(20)]

	[Header("game fields")]
	public int gameSpawnLayer = CollisionLayers.ilayerTeamEnemies;
	public float difficulty = 10f;
    public int reward = 0;
    public override float sdifficulty { get { return difficulty; }	}
	public TeleportData teleportData;

	public PolygonGameObject Create(){
		return Create(gameSpawnLayer);
	}

	public PolygonGameObject Create(int layer){
		var obj = CreateInternal (layer);
		if (obj != null) {
			obj.name = name;
            obj.reward = reward;
        }
		return obj;
	}

	protected virtual PolygonGameObject CreateInternal (int layer){return null;}

	public override void Spawn (PositionData posData, Action<SpawnedObj> callback)
	{
		var main = Singleton<Main>.inst;
		main.StartCoroutine (SpawnRoutine (this, posData, new Place(), callback));
	}

	#if UNITY_EDITOR
	void Update() {
		if (spawn && Application.isPlaying && Application.isEditor) {
			spawn = false;
			for (int i = 0; i < spawnCount; i++) {
				EditorSpawn ();
			}
		}
	}
	#endif
	private void EditorSpawn() {
		var obj = Create (editorSpawnLayer);
		if (obj == null) {
            Debug.LogWarning ("Cretae obj is null " + name);
			return;
		}

		Vector2 pos;
		float lookAngle;
		SpawnPositioning positioning = new SpawnPositioning ();
		positioning.positionAngleRange = 360;
		positioning.lookAngleRange = 360;
		Singleton<Main>.inst.GetRandomPosition(new RandomFloat(40, 50), positioning, out pos, out lookAngle);
		obj.cacheTransform.position = pos;
		obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);
		Singleton<Main>.inst.Add2Objects(obj);
	}
}

