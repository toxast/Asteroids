using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MWaveBase : MonoBehaviour
{
	public virtual IWaveSpawner GetWave() { return null; }
}
