using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[Serializable]
public class MRandomWave : MWaveBase {
	public RandomWaveData waveData;

	[Serializable]
	public class RandomWaveData{
		public float diffucultyAtOnce = 50f;
		public float diffucultyTotal = 100f;
		public float startNextWaveWhenDifficultyLeft = 0f;
		public RandomInt differentSpawnsCountRange = new RandomInt{max = 3, min = 1}; 
		public List<WeightedSpawn> objects;
	}

	public override IWaveSpawner GetWave() { 
		return new RandomWave(waveData); 
	}

	public override List<MSpawnBase> GetElements ()
	{
		return waveData.objects.ConvertAll (e => e.spawn);
	}

	[Space (30)]
	[SerializeField] bool createDefWaveEditor = false;
	void OnValidate(){
		if (createDefWaveEditor) {
			createDefWaveEditor = false;
			waveData = new RandomWaveData ();
			waveData.objects = new List<WeightedSpawn> ();
			waveData.objects.Add (new WeightedSpawn ());
		}
	}
}

[Serializable]
public class WeightedSpawn
{
	[SerializeField] public MSpawnBase spawn;
	[SerializeField] public float weight = 4;
	[SerializeField] public RandomFloat range = new RandomFloat(40, 50);
	[SerializeField] public SpawnPositioning positioning = new SpawnPositioning {positionAngleRange = 360} ;

	public float difficulty{
		get{ return spawn.sdifficulty;}
	}
} 

public class RandomWave : IWaveSpawner{
	public class SpawnObj {
		public MSpawnDataBase spawnData;
		public PolygonGameObject pgo;
	}

	MRandomWave.RandomWaveData data;
	Main main;
	float totalDifficulyLeft;
	IEnumerator spawnRoutine;
	float spawningDifficulty = 0;
	List<MSpawnBase.SpawnedObj> spawned = new List<MSpawnBase.SpawnedObj>();

	public RandomWave(MRandomWave.RandomWaveData data) {
		this.data = data;
		main = Singleton<Main>.inst;
		totalDifficulyLeft = data.diffucultyTotal;
		spawnRoutine = CheckSpawnNextRoutine ();
	}

	public void Tick() { 
		if (spawnRoutine != null) {
			spawnRoutine.MoveNext ();
		}
	}

	public bool Done() {
		return (CurrentDifficulty() <= data.startNextWaveWhenDifficultyLeft && totalDifficulyLeft <= 0);
	}

	private IEnumerator CheckSpawnNextRoutine() {
		bool first = true;
		bool prepareNextSpawnGroup = true;
		float preparedDifficulty = 0;
		List<int> selectedSpawnsCount = new List<int>();
		List<WeightedSpawn> selectedSpawns = new List<WeightedSpawn> ();
		while (true) {
			if (prepareNextSpawnGroup) {
				selectedSpawns = SelectSpawns(data.objects, data.diffucultyAtOnce, data.differentSpawnsCountRange);
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
						preparedDifficulty = data.diffucultyAtOnce;
					} else {
						preparedDifficulty = Rnd.Range (minDifficulty, Mathf.Min (data.diffucultyAtOnce, totalDifficulyLeft));
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

			if (selectedSpawnsCount.Count == selectedSpawns.Count) {
				if (preparedDifficulty <= data.diffucultyAtOnce - CurrentDifficulty ()) {
					CountInWhatWillBeSpawned (selectedSpawns, selectedSpawnsCount);
					Spawn (selectedSpawns, selectedSpawnsCount); 
					prepareNextSpawnGroup = true;
				}	
			} else {
				break;
			}
			yield return null;
		}
	}

	private List<WeightedSpawn> SelectSpawns(List<WeightedSpawn> list, float maxOneDifficulty, RandomInt differentSpawnsCountRange){
		List<WeightedSpawn> selectedSpawns = new List<WeightedSpawn> ();
		var avaliableSpawns = list.FindAll (ws => ws.difficulty <= maxOneDifficulty);
		int differentSpawnsCount = Mathf.Min (differentSpawnsCountRange.RandomValue, avaliableSpawns.Count);
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
				item.spawn.Spawn (main.GetPositionData(item.range, item.positioning) , OnObjectSpawned);
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
				totalDifficulyLeft -= item.sdifficulty;
				spawningDifficulty += item.sdifficulty;
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



