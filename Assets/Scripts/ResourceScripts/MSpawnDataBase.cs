using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MSpawnDataBase : MSpawnBase {
	[Header("editor fields")]
	public int spawnCount = 1;
	public bool spawn = true;
	[Space(20)]

	[Header("game fields")]
	public CollisionLayers.eLayerNum gameSpawnLayer = CollisionLayers.eLayerNum.TEAM_ENEMIES;
	public CollisionLayers.eLayerNum overrideCollisionLayerNum = CollisionLayers.eLayerNum.SAME;
	public float priorityMultiplier = 1f;
	public int difficulty;
    public int reward = 0;
	public override int sdifficulty { get { return difficulty; }	}
	public TeleportData teleportData;
	public MEffectData startEffect;

	public PolygonGameObject Create(){
		return Create((int)gameSpawnLayer);
	}

	public PolygonGameObject Create(int layer){
		var obj = CreateInternal (layer);
		if (obj != null) {
			obj.name = name;
            obj.reward = reward;
			if (layer == (int)gameSpawnLayer && overrideCollisionLayerNum != CollisionLayers.eLayerNum.SAME) {
				obj.SetLayerNum ((int)obj.logicNum, (int)overrideCollisionLayerNum);
			}

			if (priorityMultiplier != 0) {
				obj.priorityMultiplier = priorityMultiplier;
			} else {
				Debug.LogError ("priorityMultiplier is Zero for " + name);
			}
			if (startEffect != null) {
				startEffect.Apply (obj);
			}
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
		var obj = Create ();
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

