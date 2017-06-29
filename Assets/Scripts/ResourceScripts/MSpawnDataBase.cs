using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MSpawnDataBase : MSingleSpawn {
	[Header("editor fields")]
	[SerializeField] int spawnCount = 1;
	[SerializeField] bool spawn = true;
	[Space(20)]

	[Header("game fields")]
	[SerializeField] CollisionLayers.eLayerNum gameSpawnLayer = CollisionLayers.eLayerNum.TEAM_ENEMIES;
	[SerializeField]  CollisionLayers.eLayerNum overrideCollisionLayerNum = CollisionLayers.eLayerNum.SAME;
	[SerializeField] float priorityMultiplier = 1f;
	[SerializeField] int difficulty;
	[SerializeField] int reward = 0;
	[SerializeField] TeleportData teleportData;
	[SerializeField]  MEffectData startEffect;

	public override CollisionLayers.eLayerNum iGameSpawnLayer{ get{ return gameSpawnLayer;}}
	public override int sdifficulty { get { return difficulty; }	}
	public override TeleportData iTeleportData{ get{ return teleportData;}}

	public override PolygonGameObject Create(int layer){
		var obj = CreateInternal (layer);
		if (obj != null) {
			//obj.isBossObject = isBossObject;
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

