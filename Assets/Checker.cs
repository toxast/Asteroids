using UnityEngine;
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
	[System.NonSerialized] int powerupsFrom = 100;
	[System.NonSerialized] int maxOneTypePowerups = 10;

	[SerializeField] bool assignPowerupsId = false;
	[SerializeField] bool assignJournalsId = false;
	[SerializeField] bool assignCometsColors = false;

	[SerializeField] bool checkJournalsId = false;
	void Update(){
		if (assignJournalsId) {
			assignJournalsId = false;
			AssignJournalsId ();
		}
		if (assignPowerupsId) {
			assignPowerupsId = false;
			AssignPowerupssId ();
		}

		if (checkJournalsId) {
			checkJournalsId = false;
			CheckJournalsIds ();
		}
//		if (assignCometsColors) {
//			assignCometsColors = false;
//		}
	}

//	void AssignCometsColor(){
//		var powerups = MPowerUpResources.Instance.powerups;
//		for (int i = 0; i < powerups.Count; i++) {
//			var list = powerups [i];
//			for (int k = 0; k < list.comets.Count; k++) {
//				var obj = list.comets [k];
//				var color = obj.color;
//				obj.powerupData.particleSystemColor = color;
////				foreach (var d in obj.destructionEffects) {
////					d.overrideStartColor = true;
////					d.startColor = color;
////				}
////				foreach (var d in obj.destructionEffects) {
////					d.overrideStartColor = true;
////					d.startColor = color;
////				}
//				EditorUtility.SetDirty (obj.gameObject);
//			}
//		}
//	}

	void AssignPowerupssId(){
		#if UNITY_EDITOR
		var powerups = MPowerUpResources.Instance.powerups;
		for (int i = 0; i < powerups.Count; i++) {
			var list = powerups [i];
			for (int k = 0; k < list.comets.Count; k++) {
				var obj = list.comets [k];
				obj.id = powerupsFrom + maxOneTypePowerups * i + k;
				EditorUtility.SetDirty (obj.gameObject);
			}
		}
		#endif
	}

	void AssignJournalsId(){
		#if UNITY_EDITOR
		var userSpaceships = MSpaceShipResources.Instance.userSpaceships;
		List<MSpaceshipData> allSpaceships = new List<MSpaceshipData> ();
		for (int i = 0; i < userSpaceships.Count; i++) {
			allSpaceships.AddRange (userSpaceships [i].ships);
		}
		for (int i = 0; i < allSpaceships.Count; i++) {
			var sp = allSpaceships [i];
			sp.journal.id = spaceshipsLogsFrom + i;
			EditorUtility.SetDirty (sp.journal.gameObject);
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
				log.id = powerupsFrom + maxOneTypePowerups * i + k;
				EditorUtility.SetDirty (log.gameObject);
			}
		}

		CheckJournalsIds();

		AssetDatabase.SaveAssets();
		#endif
	}


	void CheckJournalsIds(){
		#if UNITY_EDITOR
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
		#endif
	}
}
