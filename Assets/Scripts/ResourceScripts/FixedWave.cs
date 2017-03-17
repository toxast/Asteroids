using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;



public class FixedWave : IWaveSpawner{
	public class SpawnObj {
		public MSpawnDataBase spawnData;
		public PolygonGameObject pgo;
	}

	[Serializable]
	public class Data{
		public float startNextWaveWhenDifficultyLeft = 0f;
		public List<SpawnPos> objects;
	}

	Data  data;
	Main main;
	float totalDifficulyLeft;
	IEnumerator spawnRoutine;
	float spawningDifficulty = 0;
	List<MSpawnBase.SpawnedObj> spawned = new List<MSpawnBase.SpawnedObj>();

	public FixedWave(Data data) {
		this.data = data;
		main = Singleton<Main>.inst;

		totalDifficulyLeft = 0;
		for (int i = 0; i < data.objects.Count; i++) {
			totalDifficulyLeft += data.objects [i].difficulty;
		}
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
		CountInWhatWillBeSpawned (data.objects);
		yield return Singleton<Main>.inst.StartCoroutine(Spawn (data.objects));
	}

	public enum SpawnStrategy
	{
		PICK_RANDOM = 0,
		MIN = 1 ,
		ALL_AT_ONCE,
		QUICK_DELAYS,
		LONG_DELAYS,
		MAX,
	}

	private IEnumerator Spawn(List<SpawnPos> selectedSpawns){
		SpawnStrategy strategy = (SpawnStrategy)UnityEngine.Random.Range ((int)SpawnStrategy.MIN + 1, (int)SpawnStrategy.MAX);
		for (int i = 0; i < selectedSpawns.Count; i++) {
			int index = UnityEngine.Random.Range (0, selectedSpawns.Count);
			var item = selectedSpawns [index];
			item.spawn.Spawn (main.GetPositionData(item.range, item.positioning) , OnObjectSpawned);
			if (strategy == SpawnStrategy.QUICK_DELAYS) {
				yield return new WaitForSeconds (UnityEngine.Random.Range (0.2f, 0.6f));
			} else if (strategy == SpawnStrategy.LONG_DELAYS) {
				yield return new WaitForSeconds (UnityEngine.Random.Range (0.6f, 1.5f));
			} else {
				//no delay
			}
		}
		yield break;
	}

	private void OnObjectSpawned(MSpawnBase.SpawnedObj obj) {
		spawningDifficulty -= obj.difficulty;
		spawned.Add(obj);
	}

	private void CountInWhatWillBeSpawned(List<SpawnPos> selectedSpawns){
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i].spawn;
			totalDifficulyLeft -= item.sdifficulty;
			spawningDifficulty += item.sdifficulty;
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
