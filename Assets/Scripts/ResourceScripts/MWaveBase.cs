using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class MWaveBase : MonoBehaviour
{
	public virtual IWaveSpawner GetWave() { return null; }
	public abstract List<MSpawnBase> GetElements ();
}
