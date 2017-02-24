using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class SpawnPositioning
{
	public float positionAngle = 0f; // 0 - right, 90 - up, 180 - left, 270 - down from the user ship
	public float positionAngleRange = 0; //randomize positionAngle by this degrees
	public float lookAngle = 0f; // 0 - towards user, 180 - turn back 2 user
	public float lookAngleRange = 0; // randomize lookAngle by this degrees
}
	
public interface ILevelSpawner
{
	bool Done ();
	void Tick ();
}

public class EmptyTestSceneSpawner : ILevelSpawner
{
	public bool Done()
	{
		return false;
	}

	public void Tick()
	{
	}
}
