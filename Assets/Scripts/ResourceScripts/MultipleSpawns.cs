using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSpawns : MSpawnBase {

	[SerializeField] List<MultiSpawnElement> elems;

	public override float difficulty{ get{
			float dif = 0;
			for (int i = 0; i < elems.Count; i++) {
				dif += elems [i].spawn.difficulty;
			}
			return dif;
		}}

	public override void Spawn(PositionData data, Action<SpawnedObj> callback){
		var main = Singleton<Main>.inst;
		for (int k = 0; k < elems.Count; k++) {
			main.StartCoroutine (SpawnRoutine (elems [k].spawn, data, elems [k].place, callback));
		}
	}

	void OnValidate(){
		for (int i = 0; i < elems.Count; i++) {
			if (elems [i].spawn.prefab == null) {
				elems [i] = new MultiSpawnElement ();
			}
		}
	}

	[SerializeField]
	public class MultiSpawnElement {
		public SpawnElem spawn;
		public Place place;
	}
}
