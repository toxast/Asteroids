using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MLevel : MonoBehaviour
{
	[Serializable]
	public class Data{ 
		public List<MWaveBase> waves;
	}

	[SerializeField] public AreaSizeData areaData;
	[SerializeField] Data data;
	[SerializeField] public MJournalLog journal;
	[SerializeField] public MJournalLog journalFinish;

	[Header("edit helper")]
	[SerializeField] int levelDifficulty = 0; 
	[SerializeField] bool insertWave = false;
	[SerializeField] bool removeWave = false;
	[SerializeField] int index = 0;
//	[SerializeField] bool renameWaves = false;
	void OnValidate(){

		levelDifficulty = 0;
		data.waves.ForEach (w => {
			if (w != null)
				levelDifficulty += w.GetDiffuculty ();
		});

		if (insertWave) {
			insertWave = false;
			data.waves.Insert (index, null);
		}
		if (removeWave) {
			removeWave = false;
			data.waves.RemoveAt (index);
		}
//		if (renameWaves) {
//			renameWaves = false;
//			for (int i = 0; i < data.waves.Count; i++) {
//				data.waves [i].name = "LWave_" + i.ToString ();
//			}
//		}
	}

	public LevelSpawner GetLevel(){
		return new LevelSpawner (data);
	}
}


