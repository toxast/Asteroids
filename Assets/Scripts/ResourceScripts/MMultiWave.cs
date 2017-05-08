using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[Serializable]
public class MMultiWave: MWaveBase {
	public List<MWaveBase> wavesData;

	public override IWaveSpawner GetWave() { 
		return new MultiWave(wavesData); 
	}

	public override List<MSpawnBase> GetElements () {
		List<MSpawnBase> elems = new List<MSpawnBase> ();
		wavesData.ForEach(w => elems.AddRange(w.GetElements()));
		return elems;
	}

	public override int GetDiffuculty ()
	{
		int diff = 0;
		wavesData.ForEach (b => diff += b.GetDiffuculty());
		return diff;
	}
}
