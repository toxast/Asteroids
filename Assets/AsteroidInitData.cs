using UnityEngine;
using System.Collections;


[System.Serializable]
public class PhysicalData : IClonable<PhysicalData>
{
	public float density = 1f;
	public float healthModifier = 1f;
	public float collisionDefence = 0f;
	public float collisionAttackModifier = 1f;

	public PhysicalData() : this(1, 1, 0, 1) {}

	public PhysicalData(float density, float healthModifier, float collisionDefence, float collisionAttackModifier)
	{
		this.density = density;
		this.healthModifier = healthModifier;
		this.collisionDefence = collisionDefence;
		this.collisionAttackModifier = collisionAttackModifier;
	}

	public PhysicalData Clone()
	{
		PhysicalData c = new PhysicalData (density, healthModifier, collisionDefence, collisionAttackModifier);
		return c;
	}
}

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
	public PhysicalData physical;
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
