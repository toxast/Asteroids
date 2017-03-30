using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class RandomWave : IWaveSpawner{
//	public class SpawnObj {
//		public MSpawnDataBase spawnData;
//		public PolygonGameObject pgo;
//	}

	[Serializable]
	public class Data{
		public float diffucultyAtOnce = 50f;
		public float diffucultyTotal = 100f;
		public float overrideDificultyAdds = -1f;
		public RandomInt differentSpawnsCountRange = new RandomInt{max = 3, min = 1}; 
		public bool chooseNewObjectsEachTime = true;
		public float overrideSpawnTime = -1f;
		public float startNextWaveWhenDifficultyLeft = 0f;
		public RandomWave.eSpawnCountStrategy countStrategy =  RandomWave.eSpawnCountStrategy.PICK_RANDOM;
		public RandomWave.eSpawnStrategy spawnStrategy =  RandomWave.eSpawnStrategy.PICK_RANDOM;
		[Header ("DestroyAreaDelay")]
		public bool useDestroyAreaDelay = false;
		public float destroyAreaTimeThreshold = 20f;
		public float destroyAreaPersentage = 0.3f;
		[Header("objects")]
		public List<WeightedSpawn> objects;
	}

	Data data;
	Main main;
	float totalDifficulyLeft;
	IEnumerator spawnRoutine;
	float spawningDifficulty = 0;
	float totalAreaSpawned = 0;
	List<MSpawnBase.SpawnedObj> spawned = new List<MSpawnBase.SpawnedObj>();
	DestroyAreaWave destroyAreaWave;

	public RandomWave(Data data) {
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
		bool condition = false;
		if (data.useDestroyAreaDelay) {
			condition = destroyAreaWave != null && destroyAreaWave.Done ();
		} else {
			condition = CurrentDifficulty () <= data.startNextWaveWhenDifficultyLeft;
		}
		return (condition && totalDifficulyLeft <= 0);
	}

	private IEnumerator CheckSpawnNextRoutine() {
		bool first = true;
		bool prepareNextSpawnGroup = true;
		float preparedDifficulty = 0;
		List<int> selectedSpawnsCount = new List<int>();
		List<WeightedSpawn> selectedSpawns = SelectSpawns(data.objects, data.diffucultyAtOnce, data.differentSpawnsCountRange);
        while (true) {
			if (prepareNextSpawnGroup) {
                if (data.chooseNewObjectsEachTime) {
                    selectedSpawns = SelectSpawns(data.objects, data.diffucultyAtOnce, data.differentSpawnsCountRange);
                }
				prepareNextSpawnGroup = false;
				selectedSpawnsCount.Clear ();

				float minDifficulty = float.MaxValue;
				for (int i = 0; i < selectedSpawns.Count; i++) {
					if (selectedSpawns [i].difficulty < minDifficulty) {
						minDifficulty = selectedSpawns [i].difficulty;
					}
				}

				if (minDifficulty <= totalDifficulyLeft) { 
					if (first) {
						first = false;
						preparedDifficulty = data.diffucultyAtOnce;
					} else {
						float dif = data.overrideDificultyAdds > 0 ? data.overrideDificultyAdds : data.diffucultyAtOnce; 
						preparedDifficulty = Rnd.Range (minDifficulty, Mathf.Min (dif, totalDifficulyLeft));
					}
					selectedSpawnsCount = GetCountForSpawns (selectedSpawns, preparedDifficulty);//todo min values should be more frequent

					/////////////////
					float difficultyPicked = 0;
					for (int i = 0; i < selectedSpawns.Count; i++) {
						difficultyPicked += selectedSpawnsCount[i] * selectedSpawns [i].difficulty;
						if(selectedSpawnsCount[i] > 0){
							Debug.Log (selectedSpawns [i].spawn.name + " " + selectedSpawnsCount [i]);
						}
					}
					//Debug.LogWarning ("preparedDifficulty: " + preparedDifficulty + " difficultyPicked: " + difficultyPicked);
					////////////////
				} else {
					//Debug.LogWarning ("totalDifficulyLeft: " + totalDifficulyLeft + " => 0");
					totalDifficulyLeft = 0;
				}
			}

			if (selectedSpawnsCount.Count == selectedSpawns.Count) {
				if (preparedDifficulty <= data.diffucultyAtOnce - CurrentDifficulty ()) {
					CountInWhatWillBeSpawned (selectedSpawns, selectedSpawnsCount);
					yield return Singleton<Main>.inst.StartCoroutine(Spawn (selectedSpawns, selectedSpawnsCount)); 
					prepareNextSpawnGroup = true;
				}	
			} else {
				break;
			}
			yield return null;
		}

		if (data.useDestroyAreaDelay) {
			while (totalDifficulyLeft > 0 || spawningDifficulty > 0) {
				yield return null;
			}

			var desData = new DestroyAreaWave.Data {
				area = totalAreaSpawned * data.destroyAreaPersentage,
				time = data.destroyAreaTimeThreshold
			};
			destroyAreaWave = new DestroyAreaWave (desData);
			while (!destroyAreaWave.Done ()) {
				destroyAreaWave.Tick ();
				yield return null;
			}
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

	public enum eSpawnCountStrategy
	{
		PICK_RANDOM = 1,
		MIN,
		LESS_DIFFICULT,
		EQUAL,
		MORE_DIFFICULT,
		MAX,
	}

	private List<int> GetCountForSpawns(List<WeightedSpawn> selectedSpawns, float difficulty){
		List<int> selectedSpawnsCount = selectedSpawns.ConvertAll (s => 0);
		if (selectedSpawns.Count == 0) {
			return selectedSpawnsCount;
		}

		float currentDifficulty = 0;
//		for (int i = 0; i < selectedSpawns.Count; i++) {
//			if (selectedSpawns [i].difficulty + currentDifficulty <= difficulty) {
//				selectedSpawnsCount [i] = 1;
//				currentDifficulty += selectedSpawns [i].difficulty;
//			}
//		}
		eSpawnCountStrategy strategy;
		if (data.countStrategy == eSpawnCountStrategy.PICK_RANDOM) {
			strategy = (eSpawnCountStrategy)UnityEngine.Random.Range ((int)eSpawnCountStrategy.MIN + 1, (int)eSpawnCountStrategy.MAX);
		} else {
			strategy = data.countStrategy;
		}

		Func<WeightedSpawn, float> weightsFunc;
		if (strategy == eSpawnCountStrategy.LESS_DIFFICULT) {
			weightsFunc = (w) => 1 / w.difficulty;
		} else if (strategy == eSpawnCountStrategy.MORE_DIFFICULT) {
			weightsFunc = (w) => w.difficulty;
		} else {
			weightsFunc = (w) => 1;
		}

		//Debug.LogError (strategy);


		List<WeightedSpawn> selectedSpawnsAvaliable = new List<WeightedSpawn> (selectedSpawns); 
		bool anyAvaliable = true;
		while (true) {
			anyAvaliable = false;
			List<float> weights = selectedSpawnsAvaliable.ConvertAll (asp => {
				if(asp.difficulty <= difficulty - currentDifficulty) {
					anyAvaliable = true;
					return weightsFunc(asp);
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

	public enum eSpawnStrategy
	{
		PICK_RANDOM = 1,
		MIN,
		ALL_AT_ONCE,
		QUICK_DELAYS,
		LONG_DELAYS,
		MAX,
	}

	//TODO:
//	public enum SpawnPositionStrategy
//	{
//		RANDOM = 0,
//		MIN = 1,
//        CIRCLE_SECTORS,
//		CIRCLE_ARC,
//		TRIANGLE,
//		MAX,
//	}

	private IEnumerator Spawn(List<WeightedSpawn> selectedSpawns, List<int> selectedSpawnsCount) {
		eSpawnStrategy strategy;
		if (data.spawnStrategy == eSpawnStrategy.PICK_RANDOM) {
			strategy = (eSpawnStrategy)UnityEngine.Random.Range ((int)eSpawnStrategy.MIN + 1, (int)eSpawnStrategy.MAX);
		} else {
			strategy = data.spawnStrategy;
		}

		List<WeightedSpawn> toSpawn = new List<WeightedSpawn> ();

		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i];
			for (int k = 0; k < selectedSpawnsCount[i]; k++) {
				toSpawn.Add (item);
			}
		}

		while (toSpawn.Count > 0) {
			int index = UnityEngine.Random.Range (0, toSpawn.Count);
			var item = toSpawn [index];
			toSpawn.RemoveAt (index);
			MSpawnBase.PositionData posData;
			if (item.spawnAtViewEdge) {
				if (item.overridePositioning) {
					posData = main.GetEdgePositionData (item.positioning.positionAngle, item.positioning.lookAngle, item.positioning.lookAngleRange); 
				} else {
					posData = main.GetEdgePositionData (UnityEngine.Random.Range (1, 360)); 
				}
			} else {
				if (item.overridePositioning) {
					posData = main.GetPositionData (item.range, item.positioning); 
				} else {
					posData = main.GetPositionData (item.range, new SpawnPositioning{ lookAngleRange = 120, positionAngleRange = 360 });
				}
			}
			item.spawn.Spawn (posData, OnObjectSpawned);
			if (data.overrideSpawnTime < 0) {
				if (strategy == eSpawnStrategy.QUICK_DELAYS) {
					yield return new WaitForSeconds (UnityEngine.Random.Range (0.2f, 0.6f));
				} else if (strategy == eSpawnStrategy.LONG_DELAYS) {
					yield return new WaitForSeconds (UnityEngine.Random.Range (0.6f, 1.5f));
				} else {
					//no delay
				}
			} else {
				yield return new WaitForSeconds (data.overrideSpawnTime);
			}
		}

		yield break;
	}

	private void OnObjectSpawned(MSpawnBase.SpawnedObj obj) {
		totalAreaSpawned += obj.obj.polygon.area;
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
