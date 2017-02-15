using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MWaveBase : MonoBehaviour, ILevelSpawner
{
	public virtual bool Done()
	{
		return false;
	}

	public virtual void Tick()
	{ 
	}
}
