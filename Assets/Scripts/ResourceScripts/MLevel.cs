using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLevel : MonoBehaviour, ILevelSpawner
{
	MWaveBase currentWave;
	public List<MWaveBase> waves;
	[NonSerialized] int waveNum = 0;

	void Awake(){
		currentWave = Instantiate(waves [waveNum]);
	}

	public bool Done() {
		return currentWave == null;
	}

	public void Tick()
	{
		if (Done ()) {
			return;
		}
		currentWave.Tick ();
		if(currentWave.Done()) {
			waveNum++;
			currentWave = waveNum < waves.Count ? Instantiate(waves [waveNum]) : null;
		}
	}
}
