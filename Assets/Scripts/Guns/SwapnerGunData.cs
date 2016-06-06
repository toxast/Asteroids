using UnityEngine;
using System.Collections;

[System.Serializable]
public class SwapnerGunData : IClonable<SwapnerGunData>, IGun
{
	public string name;
	public GunData baseData;
	public int spaceshipIndex;
	public int maxSpawn = 1;
	public int startSpawn = 1;
	public float startSpawnInterval = 2f;


	public string iname{ get {return baseData.name;}}
	public int iprice{ get {return baseData.price;}}
	public GunSetupData.eGuns itype{ get {return GunSetupData.eGuns.SPAWNER;}}
	
	public SwapnerGunData Clone()
	{
		SwapnerGunData r = new SwapnerGunData ();
		r.baseData = baseData.Clone ();
		r.spaceshipIndex = spaceshipIndex;
		r.maxSpawn = maxSpawn; 
		r.startSpawn = startSpawn;
		r.startSpawnInterval = startSpawnInterval; 
		return r;
	}
}
