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

    public float RandomValue { get{ return UnityEngine.Random.Range (min, max); }}
}

[System.Serializable]
public class RandomInt
{
	public int min = 1;
	public int max = 2;

    public int RandomValue { get{ return UnityEngine.Random.Range (min, max); }}
}

[System.Serializable]
public class AsteroidSetupData 
{
	public string name;
	public int densityIndx;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
}

[System.Serializable]
public class SpikyInitData
{
	public string name;
	public Color color = new Color (0.5f, 0.5f, 0.5f);
	public int reward = 0;
	public PhysicalData physical;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
	public RandomFloat spikeSize;
	public RandomInt spikesCount;
}

[System.Serializable]
public class AccuracyData: IClonable<AccuracyData>
{
	public float startingAccuracy = 0f;
	public float thresholdDistance = 10f;
	public bool isDynamic = true;
	public float checkDtime = 1f;
	public float add = 0.15f;
	public float sub = 0.4f;
	public Vector2 bounds = new Vector2 (0, 1); 

	public AccuracyData Clone()
	{
		AccuracyData c = new AccuracyData ();
		c.startingAccuracy = startingAccuracy;
		c.thresholdDistance = thresholdDistance;
		c.isDynamic = isDynamic;
		c.checkDtime = checkDtime;
		c.add = add;
		c.sub = sub;
		c.bounds = bounds;
		return c;
	}
}

[System.Serializable]
public class SpikeShooterInitData : SpikyInitData
{
	public float spikeVelocity = 45f;
	public float growSpeed = 0.1f;
	//public float checkForTargetdTime = 0.2f;
}

[System.Serializable]
public class SawInitData : SpikyInitData
{
	public float prepareTime = 2f;

	public float chargeSpeed = 50f;
	public float chargeRotation = 300f;
	public float chargeDuration = 3f;

	public float slowingDuration = 2f;
}
