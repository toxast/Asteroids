using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class MultipleSpawns : MSpawnBase {

	[Header("editor")]
	[SerializeField] float showDifficultyInEditor;
	[SerializeField] bool spawn = true;

	[Header ("game")]
	[SerializeField] List<MultiSpawnElement> elems;

	public override int sdifficulty{ get{
			int dif = 0;
			for (int i = 0; i < elems.Count; i++) {
				dif += elems [i].spawn.sdifficulty;
			}
			return dif;
		}}

	public override void Spawn(PositionData data, Action<SpawnedObj> callback){
		var main = Singleton<Main>.inst;
		for (int k = 0; k < elems.Count; k++) {
			main.StartCoroutine (SpawnRoutine (elems [k].spawn, data, elems [k].place, callback));
		}
	}

	#if UNITY_EDITOR
	void Update() {
		if (spawn && Application.isPlaying && Application.isEditor) {
			spawn = false;
			var main = Singleton<Main>.inst;
			var pos = main.GetPositionData (new RandomFloat (40, 65f), new SpawnPositioning{ positionAngleRange = 360f });
			Spawn (pos, null);
		}
	}
	#endif

	void OnValidate(){
		showDifficultyInEditor = sdifficulty;
		for (int i = 0; i < elems.Count; i++) {
			if (elems [i].spawn == null) {
				elems [i] = new MultiSpawnElement ();
			}
		}
	}
    void WarningFix() {
        if (showDifficultyInEditor < 0) showDifficultyInEditor = 0;
    }

    [Serializable]
	public class MultiSpawnElement {
		public MSingleSpawn spawn;
		public Place place;
	}
}
