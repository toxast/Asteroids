using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[System.Serializable]
public class MRandomWave : MWaveBase {

	[System.Serializable]
	public class WeightedSpawn
	{
		[SerializeField] public MSpawnBase spawn;
		[SerializeField] public float weight = 4;
		[SerializeField] public RandomFloat range;
		[SerializeField] public SpawnPositioning positioning;

		public float difficulty{
			get{ return spawn.difficulty;}
		}
	}

	public class SpawnObj
	{
		public MSpawnDataBase spawnData;
		public PolygonGameObject pgo;
	}

	[SerializeField] float diffucultyAtOnce = 50f;
	[SerializeField] float diffucultyTotal = 100f;
	[SerializeField] float startNextWaveWhenDifficultyLeft = 0f;
	[SerializeField] List<WeightedSpawn> objects;

	[NonSerialized] bool startedSpawn = false;
	[NonSerialized] bool forceDone = false;
	[NonSerialized] float spawningDifficulty = 0;
	[NonSerialized] List<MSpawnBase.SpawnedObj> spawned = new List<MSpawnBase.SpawnedObj>();
	[NonSerialized] Main main;
	[NonSerialized] float totalDifficulyLeft;

	void Awake() {
		totalDifficulyLeft = diffucultyTotal;
		main = Singleton<Main>.inst;
	}

	public override void Tick() { 
		if(!startedSpawn) {
			startedSpawn = true;
			List<WeightedSpawn> selectedSpawns = SelectSpawns(objects, diffucultyAtOnce, 1, 3);
			if (selectedSpawns.Any ()) {
				StartCoroutine (CheckSpawnNextRoutine (selectedSpawns));
			} else {
				forceDone = true;
			}
		}
	}

	public override bool Done() {
		return forceDone || (startedSpawn && CurrentDifficulty() <= startNextWaveWhenDifficultyLeft && totalDifficulyLeft <= 0);
	}

	private IEnumerator CheckSpawnNextRoutine(List<WeightedSpawn> selectedSpawns) {
		bool first = true;
		bool prepareNextSpawnGroup = true;
		float preparedDifficulty = 0;
		List<int> selectedSpawnsCount = new List<int>();
		while (true) {
			if (prepareNextSpawnGroup) {
				prepareNextSpawnGroup = false;
				selectedSpawnsCount.Clear ();

				float minDifficulty = float.MaxValue;
				for (int i = 0; i < selectedSpawns.Count; i++) {
					if (selectedSpawns [i].difficulty < minDifficulty) {
						minDifficulty = selectedSpawns [i].difficulty;
					}
				}

				if (minDifficulty < totalDifficulyLeft) { 
					if (first) {
						first = false;
						preparedDifficulty = diffucultyAtOnce;
					} else {
						preparedDifficulty = Rnd.Range (minDifficulty, Mathf.Min (diffucultyAtOnce, totalDifficulyLeft));
					}
					Debug.LogWarning ("preparedDifficulty: " + preparedDifficulty);
					selectedSpawnsCount = GetCountForSpawns (selectedSpawns, preparedDifficulty);//todo min values should be more frequent

					/////////////////
					float difficultyPicked = 0;
					for (int i = 0; i < selectedSpawns.Count; i++) {
						difficultyPicked += selectedSpawnsCount[i] * selectedSpawns [i].difficulty;
					}
					Debug.LogWarning ("difficultyPicked: " + difficultyPicked);
					////////////////
				} else {
					Debug.LogWarning ("totalDifficulyLeft: " + totalDifficulyLeft + " => 0");
					totalDifficulyLeft = 0;
				}
			}

			if (selectedSpawnsCount.Count == selectedSpawns.Count && preparedDifficulty <= diffucultyAtOnce - CurrentDifficulty ()) {
				CountInWhatWillBeSpawned (selectedSpawns, selectedSpawnsCount);
				Spawn (selectedSpawns, selectedSpawnsCount); 
				prepareNextSpawnGroup = true;
			}
			yield return null;
		}
	}

	private List<WeightedSpawn> SelectSpawns(List<WeightedSpawn> list, float maxOneDifficulty, int minDifferentSpawns, int maxDifferentSpawns){
		List<WeightedSpawn> selectedSpawns = new List<WeightedSpawn> ();
		var avaliableSpawns = list.FindAll (ws => ws.difficulty <= maxOneDifficulty);
		int differentSpawnsCount = Mathf.Min (Rnd.Range (minDifferentSpawns, maxDifferentSpawns + 1), avaliableSpawns.Count);
		for (int i = 0; i < differentSpawnsCount && avaliableSpawns.Any(); i++) {
			var indx = Math2d.Roll (avaliableSpawns.ConvertAll (asp => asp.weight));
			selectedSpawns.Add (avaliableSpawns [indx]);
			avaliableSpawns.RemoveAt (indx);
		}
		return selectedSpawns;
	}

	//TODO different strategies
	//current - less diffuculty more chance
	private List<int> GetCountForSpawns(List<WeightedSpawn> selectedSpawns, float difficulty){
		float currentDifficulty = 0;
		List<int> selectedSpawnsCount = selectedSpawns.ConvertAll (s => 0);
		List<WeightedSpawn> selectedSpawnsAvaliable = new List<WeightedSpawn> (selectedSpawns); 
		while (selectedSpawnsAvaliable.Count > 0) {
			bool anyAvaliable = false;
			List<float> weights = selectedSpawnsAvaliable.ConvertAll (asp => {
				if(asp.difficulty <= difficulty - currentDifficulty) {
					anyAvaliable = true;
					return 1f; //1f/asp.difficulty));
				} else {
					return 0f;
				}
			});
			if (!anyAvaliable) {
				break;
			}
			var indx = Math2d.Roll (weights);
			currentDifficulty += selectedSpawnsAvaliable [indx].difficulty;
			selectedSpawnsCount [indx] = selectedSpawnsCount [indx] + 1;
		}
		return selectedSpawnsCount;
	}

	public enum eSpawnStrategy {
		Random = -1,
		ByPositioningAllAtOnce = 1,
		ByPositioningWithIntervals = 2,
		FiewGroups = 4,

	}

	private void Spawn(List<WeightedSpawn> selectedSpawns, List<int> selectedSpawnsCount){
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i];
			for (int k = 0; k < selectedSpawnsCount[i]; k++) {
				item.spawn.Spawn (main.GetPositionData(item.range,item.positioning) , OnObjectSpawned);
			}
		}
	}

	private void OnObjectSpawned(MSpawnBase.SpawnedObj obj) {
		spawningDifficulty -= obj.difficulty;
		spawned.Add(obj);
	}

	private void CountInWhatWillBeSpawned(List<WeightedSpawn> selectedSpawns, List<int> selectedSpawnsCount){
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i].spawn;
			for (int k = 0; k < selectedSpawnsCount[i]; k++) {
				totalDifficulyLeft -= item.difficulty;
				spawningDifficulty += item.difficulty;
			}
		}
	}

	//difficulty of alive spawned and current spawning
	private float CurrentDifficulty() {
		float current = spawningDifficulty;
		for (int i = spawned.Count - 1; i >= 0; i--) {
			if (!Main.IsNull (spawned [i].obj)) {
				current += spawned [i].difficulty;
			} else {
				spawned.RemoveAt (i);
			}
		} 
		return current;
	}
	
}


