using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MLevel : MonoBehaviour
{
	[SerializeField] LevelData data;

	public LevelSpawner GetLevel(){
		return new LevelSpawner (data);
	}

	[Serializable]
	public class LevelData{ 
		public List<MWaveBase> waves;
	}

}


public class LevelSpawner : ILevelSpawner
{
	MLevel.LevelData data;
	IWaveSpawner currentWave;
	Stack<IWaveSpawner> deferredWaves = new Stack<IWaveSpawner>(); //paused waves
	int waveNum = 0;

	public LevelSpawner(MLevel.LevelData data) {
		this.data = data;
		currentWave = data.waves [0].GetWave();
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
			if (deferredWaves.Count > 0) {
				currentWave = deferredWaves.Pop();
			} else {
				waveNum++;
				currentWave = waveNum < data.waves.Count ? data.waves [waveNum].GetWave () : null;
			}
		}
	}


	public void ForceInsertWave(IWaveSpawner wave){
		deferredWaves.Push(currentWave);
		currentWave = wave;
	}
}