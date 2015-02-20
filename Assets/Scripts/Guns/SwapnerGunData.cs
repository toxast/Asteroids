using UnityEngine;
using System.Collections;

[System.Serializable]
public class SwapnerGunData : IClonable<SwapnerGunData>
{
	public GunData baseData;
	public int spaceshipIndex;
	public int maxSpawn = 1;

	public SwapnerGunData Clone()
	{
		SwapnerGunData r = new SwapnerGunData ();
		r.baseData = baseData.Clone ();
		r.spaceshipIndex = spaceshipIndex;
		r.maxSpawn = maxSpawn; 
		return r;
	}
}
