using UnityEngine;
using System.Collections;

[System.Serializable]
public class RandomFloat
{
	public float min = 1;
	public float max = 2;
}

[System.Serializable]
public class RandomInt
{
	public int min = 1;
	public int max = 2;
}

[System.Serializable]
public class AsteroidSetupData 
{
	public int densityIndx;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
}

[System.Serializable]
public class SpikyInitData
{
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
	public RandomFloat spikeSize;
	public RandomInt spikesCount;
}
