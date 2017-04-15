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
		public RandomWave.eSpawnStrategy spawnStrategy =  RandomWave.eSpawnStrategy.PICK_RANDOM;
		public int startNextWaveWhenDifficultyLeft = 0;
		public List<SpawnPos> objects;
	}

	Data  data;
	Main main;
	int totalDifficulyLeft;
	IEnumerator spawnRoutine;
	int spawningDifficulty = 0;
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

	private IEnumerator Spawn(List<SpawnPos> selectedSpawns){
		RandomWave.eSpawnStrategy strategy;
		if (data.spawnStrategy == RandomWave.eSpawnStrategy.PICK_RANDOM) {
			strategy = (RandomWave.eSpawnStrategy)UnityEngine.Random.Range ((int)RandomWave.eSpawnStrategy.MIN + 1, (int)RandomWave.eSpawnStrategy.MAX);
		} else {
			strategy = data.spawnStrategy;
		}

        float totalAngleOffset = UnityEngine.Random.Range(0, 360);
		for (int i = 0; i < selectedSpawns.Count; i++) {
			var item = selectedSpawns [i];
			MSpawnBase.PositionData positionData;
			if (item.spawnAtViewEdge) {
				var pos = item.positioning;
				positionData = main.GetEdgePositionData (pos.positionAngle, pos.lookAngle, pos.lookAngleRange); 
			} else {
				positionData = main.GetPositionData(item.range, item.positioning);
			}
			positionData.rangeAngle += totalAngleOffset;
            item.spawn.Spawn (positionData, OnObjectSpawned);
			if (strategy ==  RandomWave.eSpawnStrategy.QUICK_DELAYS) {
				yield return new WaitForSeconds (UnityEngine.Random.Range (0.2f, 0.6f));
			} else if (strategy ==  RandomWave.eSpawnStrategy.LONG_DELAYS) {
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
	private int CurrentDifficulty() {
		int current = spawningDifficulty;
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


public class DestroyAreaWave : IWaveSpawner{

	[Serializable]
	public class Data {
		public float area = 0f;
		public float time = 20f;
	}

	Data data;
	IEnumerator spawnRoutine;
	float timeLeft;
	float startArea;
	List<PolygonGameObject> objs;
	bool firstTick = true;
	bool areaKilled = false;
	public DestroyAreaWave(Data data) {
		this.data = data;
		timeLeft = data.time;
		objs = Singleton<Main>.inst.gObjects;
		spawnRoutine = CheckSpawnNextRoutine ();
	}

	public void Tick() { 
		if (firstTick) {
			firstTick = false;
			startArea = GetCurrentArea ();
//			Debug.LogError ("current area: " + startArea + "need to destroy: " + data.area);
		}
		if (spawnRoutine != null) {
			spawnRoutine.MoveNext ();
		}
	}

	private float GetCurrentArea() {
		float current = 0;
		for (int i = 0; i < objs.Count ; i++) {
			current += objs [i].polygon.area;
		}
//		Debug.LogError ("current area: " + current);
		return current;
	}

	public bool Done() {
		return (timeLeft <= 0 || areaKilled);
	}

	private void CheckIfAreaDestroyed() {
		if (startArea - GetCurrentArea () > data.area) {
			areaKilled = true;
		}
	}

	
	private IEnumerator CheckSpawnNextRoutine() {
		AIHelper.MyRepeatTimer timer = new AIHelper.MyRepeatTimer (1f, CheckIfAreaDestroyed);
		while (true) {
			float delta = Time.deltaTime;
			timeLeft -= delta;
			timer.Tick (delta);
			yield return null;
		}
	}
}
