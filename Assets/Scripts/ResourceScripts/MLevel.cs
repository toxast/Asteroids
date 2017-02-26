using System;
using System.Collections;
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
	int waveNum = 0;

	public LevelSpawner(MLevel.LevelData data) {
		this.data = data;
		currentWave = data.waves [0].GetWave();
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
			waveNum++;
			currentWave = waveNum < data.waves.Count ? data.waves [waveNum].GetWave () : null;
		}
	}
}