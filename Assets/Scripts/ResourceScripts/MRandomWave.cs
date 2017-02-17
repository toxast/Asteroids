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
		[SerializeField] public MSpawn spawn;
		[SerializeField] public float weight = 4;
		[SerializeField] public SpawnPositioning positioning;

		public float difficulty{
			get{ return spawn.prefab.difficulty;}
		}
	}

	public class SpawnObj
	{
		public MSpawn spawnData;
		public PolygonGameObject pgo;
	}

	[SerializeField] float diffucultyAtOnce = 50f;
	[SerializeField] float diffucultyTotal = 100f;
	[SerializeField] float startNextWaveWhenDifficultyLeft = 0f;
	[SerializeField] List<WeightedSpawn> objects;

	[NonSerialized] bool startedSpawn = false;
	[NonSerialized] bool forceDone = false;
	[NonSerialized] List<MSpawn> spawning = new List<MSpawn>();
	[NonSerialized] List<SpawnObj> spawned = new List<SpawnObj>();
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
				StartCoroutine(Spawn (selectedSpawns, selectedSpawnsCount)); 
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
		selectedSpawnsAvaliable = selectedSpawnsAvaliable.FindAll (s => s.difficulty <= difficulty - currentDifficulty);
		while (selectedSpawnsAvaliable.Count > 0) {
			var indx = Math2d.Roll (selectedSpawnsAvaliable.ConvertAll (asp => 1f/asp.difficulty));
			currentDifficulty += selectedSpawnsAvaliable [indx].difficulty;
			selectedSpawnsCount [indx] = selectedSpawnsCount [indx] + 1;
			selectedSpawnsAvaliable = selectedSpawnsAvaliable.FindAll (s => s.difficulty <= difficulty - currentDifficulty);
		}
		return selectedSpawnsCount;
	}

	public enum eSpawnStrategy {
		Random = -1,
		ByPositioningAllAtOnce = 1,
		ByPositioningWithIntervals = 2,
		FiewGroups = 4,

	}

	private IEnumerator Spawn(List<WeightedSpawn> selectedSpawns, List<int> selectedSpawnsCount){
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i];
			for (int k = 0; k < selectedSpawnsCount[i]; k++) {
				Vector2 pos;
				float lookAngle;
				main.GetRandomPosition (item.spawn.spawnRange, item.positioning, out pos, out lookAngle);
				StartCoroutine(SpawnObjectWithTeleportAnimation(item.spawn, pos, lookAngle));
			}
		}
		yield break;
	}

	private IEnumerator SpawnObjectWithTeleportAnimation(MSpawn item, Vector2 pos, float lookAngle) {
		//animation
		var anim = main.CreateTeleportationRing2(pos, item);
		yield return new WaitForSeconds(item.teleportDuration);
		anim.Stop ();
		main.PutObjectOnDestructionQueue (anim.gameObject, 5f);

		spawning.Remove (item);
		spawned.Add(new SpawnObj{pgo = item.Spawn(pos, lookAngle), spawnData = item});
	}

	private void CountInWhatWillBeSpawned(List<WeightedSpawn> selectedSpawns, List<int> selectedSpawnsCount){
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i].spawn;
			for (int k = 0; k < selectedSpawnsCount[i]; k++) {
				totalDifficulyLeft -= item.difficulty;
				spawning.Add (item);
			}
		}
	}

	//how many alive spawned and current spawning count
	private int Left() {
		int left = spawning.Count;
		for (int i = spawned.Count - 1; i >= 0; i--) {
			if (!Main.IsNull (spawned [i].pgo)) {
				left++;
			} else {
				spawned.RemoveAt (i);
			}
		} 
		return left;
	}

	//difficulty of alive spawned and current spawning
	private float CurrentDifficulty() {
		float current = 0;
		for (int i = 0; i < spawning.Count; i++) {
			current += spawning [i].difficulty;
		}

		for (int i = spawned.Count - 1; i >= 0; i--) {
			if (!Main.IsNull (spawned [i].pgo)) {
				current += spawned [i].spawnData.difficulty;
			} else {
				spawned.RemoveAt (i);
			}
		} 
		return current;
	}
	
}


