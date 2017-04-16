﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Checker : MonoBehaviour {

	[System.NonSerialized] int spaceshipsLogsFrom = 1;
	[System.NonSerialized] int levelsLogsFrom = 50;
	[System.NonSerialized] int powerupLogsFrom = 100;
	[System.NonSerialized] int maxOneTypePowerups = 10;

	[SerializeField] bool assignJournalsId = false;
	void Update(){
		if (assignJournalsId) {
			assignJournalsId = false;
			AssignJournalsId ();
		}
	}

	void AssignJournalsId(){

		var userSpaceships = MSpaceShipResources.Instance.userSpaceships;
		List<MSpaceshipData> allSpaceships = new List<MSpaceshipData> ();
		for (int i = 0; i < userSpaceships.Count; i++) {
			allSpaceships.AddRange (userSpaceships [i].ships);
		}
		for (int i = 0; i < allSpaceships.Count; i++) {
			var sp = allSpaceships [i];
			sp.journal.id = spaceshipsLogsFrom + i;
			EditorUtility.SetDirty (sp.gameObject);
		}

		var levels = MLevelsResources.Instance.levels;
		for (int i = 0; i < levels.Count; i++) {
			var lvl = levels [i];
			lvl.journal.id = levelsLogsFrom + 2 * i;
			lvl.journalFinish.id = levelsLogsFrom + 2 * i + 1;
			EditorUtility.SetDirty (lvl.journal.gameObject);
			EditorUtility.SetDirty (lvl.journalFinish.gameObject);
		}

		var powerups = MPowerUpResources.Instance.powerups;
		for (int i = 0; i < powerups.Count; i++) {
			var list = powerups [i];
			for (int k = 0; k < list.comets.Count; k++) {
				var log = list.comets [k].journal;
				if (log == null) {
					Debug.LogError (list.comets [k].name + " no log");
					continue;
				}
				log.id = powerupLogsFrom + maxOneTypePowerups * i + k;
				EditorUtility.SetDirty (log.gameObject);
			}
		}

		CheckJournalsIds();

		AssetDatabase.SaveAssets();
	}


	void CheckJournalsIds(){
		Dictionary<int, string> id2name = new Dictionary<int, string> ();
		var list = ShipEditor.LoadPrefabsContaining<MJournalLog> ();
		foreach (var item in list) {
			string ename;
			if (id2name.TryGetValue (item.id, out ename)) {
				Debug.LogError ("same id: " + item.id + " " + ename + " " + item.name);
			} else {
				id2name [item.id] = item.name;
			}
		}
	}
}
