using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

[Serializable]
public class MRepeatWave: MWaveBase {
	[SerializeField] public MWaveBase wave;
	[SerializeField] public int count = 1;
	
	public override IWaveSpawner GetWave ()
	{
		return base.GetWave ();
	}

	public override List<MSpawnBase> GetElements ()
	{
		return wave.GetElements ();
	}

	public override int GetDiffuculty ()
	{
		return wave.GetDiffuculty () * count;
	}
}
