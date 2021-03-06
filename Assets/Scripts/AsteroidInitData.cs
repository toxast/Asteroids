﻿using UnityEngine;
using System.Collections;


[System.Serializable]
public class PhysicalData : IClonable<PhysicalData>
{
	public float density = 1f;
	public float health = -1;
	public float healthModifier = 1f; //not used if health is >= 0
	public float collisionDefence = 0f;
	public float collisionAttackModifier = 1f;

	public PhysicalData() : this(1, 1, 0, 1) {}

	public PhysicalData(float density, float healthModifier, float collisionDefence, float collisionAttackModifier, float health = -1)
	{
		this.density = density;
		this.healthModifier = healthModifier;
		this.health = health;
		this.collisionDefence = collisionDefence;
		this.collisionAttackModifier = collisionAttackModifier;
	}

	public PhysicalData Clone()
	{
		PhysicalData c = new PhysicalData (density, healthModifier, collisionDefence, collisionAttackModifier, health);
		return c;
	}
}

[System.Serializable]
public class RandomFloat
{
	public float min = 1;
	public float max = 2;

    public float RandomValue { get{ return UnityEngine.Random.Range (min, max); }}

	public float Middle { get{ return 0.5f*(min+max); }}

	public RandomFloat(){}

	public RandomFloat( float min, float max){
		this.min = min;
		this.max = max;
	}
}

[System.Serializable]
public class RandomInt
{
	public int min = 1;
	public int max = 2;

    public int RandomValue { get{ return UnityEngine.Random.Range (min, max + 1); }}
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
public class AccuracyData: IClonable<AccuracyData>
{
	public float startingAccuracy = 0f;
	public float thresholdDistance = 10f;
	public bool isDynamic = false;
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
