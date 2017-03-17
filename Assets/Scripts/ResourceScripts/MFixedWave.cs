using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[Serializable]
public class MFixedWave: MWaveBase {
	public FixedWave.Data waveData;

	public override IWaveSpawner GetWave() { 
		return new FixedWave(waveData); 
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
			waveData = new FixedWave.Data ();
			waveData.objects = new List<SpawnPos> ();
			waveData.objects.Add (new SpawnPos ());
		}
	}
}