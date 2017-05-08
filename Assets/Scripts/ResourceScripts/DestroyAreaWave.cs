using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

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
