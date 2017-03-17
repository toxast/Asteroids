using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[Serializable]
public class MRandomWave : MWaveBase {
	public RandomWave.Data waveData;

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
			waveData = new RandomWave.Data ();
			waveData.objects = new List<WeightedSpawn> ();
			waveData.objects.Add (new WeightedSpawn ());
		}
	}
}




