using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : ILevelSpawner
{
	MLevel.Data data;
	IWaveSpawner currentWave;
	Stack<IWaveSpawner> deferredWaves = new Stack<IWaveSpawner>(); //paused waves
	int waveNum = 0;

	DateTime start;
	DateTime lastStartWave;

	public Action OnWaveFinished;
	public int WavesCount{get{return data.waves.Count;}}

	public LevelSpawner(MLevel.Data data) {
		this.data = data;
		currentWave = data.waves [0].GetWave();
		lastStartWave = DateTime.Now;
		start = DateTime.Now;
	}

    public void ForceWaveNum(int pwave) {
        waveNum = pwave;
        currentWave = data.waves[pwave].GetWave();
		lastStartWave = DateTime.Now;
		Debug.LogError ("ForceWaveNum " + pwave);
    }

	public List<MSpawnBase> GetElements (){
		var allelements = new HashSet<MSpawnBase> ();
		foreach (var item in data.waves) {
			foreach (var spawn in item.GetElements ()) {
				allelements.Add (spawn);
			}
		}
		Debug.LogWarning (allelements.Count + "different spawns");
		return allelements.ToList ();
	}

	public bool Done() {
		return currentWave == null;
	}

	public void Tick()
	{
		if (currentWave == null) {
			return;
		}
		currentWave.Tick ();
		if(currentWave.Done()) {
			if (OnWaveFinished != null) {
				OnWaveFinished ();
			}
			Debug.LogError ("finished wave " + waveNum + " in " + (DateTime.Now - lastStartWave).TotalSeconds);
			lastStartWave = DateTime.Now;
			if (deferredWaves.Count > 0) {
				currentWave = deferredWaves.Pop();
			} else {
				waveNum++;
				currentWave = waveNum < data.waves.Count ? data.waves [waveNum].GetWave () : null;
			}

			if (currentWave == null) {
				Debug.LogError ("finished all in " + (DateTime.Now - start).TotalSeconds);
			}
		}
	}

	public void ForceInsertWave(IWaveSpawner wave){
		deferredWaves.Push(currentWave);
		currentWave = wave;
		lastStartWave = DateTime.Now;
	}
}