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
	public float density = 1f;
	public float healthModifier = 1f;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
	public RandomFloat spikeSize;
	public RandomInt spikesCount;
}

[System.Serializable]
public class SpikeShooterInitData : SpikyInitData
{
	public float spikeVelocity = 45f;
}

[System.Serializable]
public class SawInitData : SpikyInitData
{
	public float chargeDuration = 3f;
	public float chargeSpeed = 50f;
	public float chargeRotation = 300f;
	public float prepareTime = 2f;
}
