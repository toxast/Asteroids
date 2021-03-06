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

	public override int GetDiffuculty ()
	{
		int diff = 0;
		waveData.objects.ForEach (b => diff += b.difficulty);
		return diff;
	}

	public override List<MSpawnBase> GetElements ()
	{
		return waveData.objects.ConvertAll (e => e.spawn);
	}

	[Space (30)]
	[SerializeField] bool createDefWaveEditor = false;
    [SerializeField] float totalDifficulty;

    void OnValidate(){
        totalDifficulty = 0;
        for (int i = 0; i < waveData.objects.Count; i++) {
			if (waveData.objects [i].spawn == null) {
				Debug.LogError ("null at " + i + " " + this.gameObject.name);
			}
			totalDifficulty += waveData.objects[i].difficulty * waveData.objects[i].count;
        }

		if (createDefWaveEditor) {
			createDefWaveEditor = false;
			waveData = new FixedWave.Data ();
			waveData.objects = new List<FixedWave.CountSpawnPos>();
			waveData.objects.Add (new FixedWave.CountSpawnPos ());
		}
	}
}



