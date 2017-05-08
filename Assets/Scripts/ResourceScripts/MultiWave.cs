using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class MultiWave : IWaveSpawner {
	List<IWaveSpawner> waves;
	public MultiWave(List<MWaveBase> wavesData) {
		waves = wavesData.ConvertAll (w => w.GetWave ());
	}
	#region IWaveSpawner implementation
	public bool Done () {
		return waves.TrueForAll (w => w.Done ());
	}
	public void Tick () {
		waves.ForEach (w => w.Tick ());
	}
	#endregion
}
