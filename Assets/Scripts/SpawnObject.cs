using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ObjectType
{
	eNone = 0,
	eAsteroid = 1,
	eSpaceship = 2,
	eSpiky = 3,
	eSaw = 4,
	eTurretTower = 5,
	eGasteroid = 6,
	ePowerup = 10,
}
[Serializable]
public class SpawnPositioning
{
	public float angle2Ship = 0f; // 0 - towards user, 180 - turn back 2 user
	public float angle2ShipRange = 180f; // randomize angle2Ship by this degrees
	public float angleFromShip = 0f; // 0 - right, 90 - up, 180 - left, 270 - down from the user ship
	public float angleFromShipRange = 180f; //randomize angleFromShip by this degrees
}

[Serializable]
public class SpawnObject 
{
	public string name = "spawns";
	public ObjectType type = ObjectType.eAsteroid;
	public int indx;
	public int count = 1;
	public float spawnInterval = 0;
	public Vector2 spawnRange = new Vector2(40,80);
	public SpawnPositioning positioning;


	public PolygonGameObject Spawn()
	{
		PolygonGameObject obj = null;
		var main = Singleton<Main>.inst;
		//TODO: spawn and save for count
		switch (type) 
		{
		case ObjectType.eAsteroid:
			obj = main.CreateAsteroid(indx);
			break;
		case ObjectType.eSpaceship:
			obj = main.CreateEnemySpaceShip(indx);
			break;
		case ObjectType.eSpiky:
			obj = main.CreateSpikyAsteroid(indx);
			break;
		case ObjectType.eSaw:
			obj = main.CreateSawEnemy(indx);
			break;
		case ObjectType.eTurretTower:
			obj = main.CreateSimpleTower(indx);
			break;
			//gasteroid
			//blockpost

		default:
			break;
		}

		if(obj == null)
		{
			Debug.LogWarning("null " + type + " " + indx);
		}
		else
		{
			if(obj.layerNum != CollisionLayers.ilayerAsteroids)
				obj.targetSystem = new TargetSystem (obj);

			Singleton<Main>.inst.SetRandomPosition(obj, spawnRange, positioning);
			Singleton<Main>.inst.Add2Objects(obj);
		}


		return obj;
	}
}

[Serializable]
public class SpawnWave
{
	public string name = "wave";
	public List<SpawnObject> objects;
	public int doneWhenLeft = 0;

	[NonSerialized] public bool startedSpawn = false;
	[NonSerialized] public bool finishedSpawn = false;
	[NonSerialized] private List<PolygonGameObject> spawned = new List<PolygonGameObject>();

	public bool Done()
	{
		if (!finishedSpawn)
			return false;

		int left = 0;
		for (int i = 0; i < spawned.Count; i++) 
		{
			if(!Main.IsNull(spawned[i]))
			{
				left++;
			}
		}

		return left <= doneWhenLeft;
	}

	public void Spawn()
	{
		if(!startedSpawn)
		{
			Singleton<Main>.inst.StartCoroutine(SpawnRoutine());
		}
	}

	private IEnumerator SpawnRoutine()
	{
		startedSpawn = true;
		foreach (var item in objects) 
		{
			for (int i = 0; i < item.count; i++) 
			{
				spawned.Add(item.Spawn());
				if(item.spawnInterval > 0)
				{
					yield return new WaitForSeconds(item.spawnInterval);
				}
			}
		}
		finishedSpawn = true;
		yield break;
	}
}

[Serializable]
public class SpawnWaves
{
	public string name = "level";
	public List<SpawnWave> list;
}

[Serializable]
public class LevelSpawner
{
	public SpawnWaves waves;
	[NonSerialized] int currentQueue = 0;

	public LevelSpawner(SpawnWaves waves, int waveNum = 0)
	{
		this.waves = waves;
		currentQueue = waveNum;
	}

	public bool Done()
	{
		return (currentQueue == waves.list.Count - 1) && waves.list [currentQueue].Done ();
	}

	public void Tick()
	{
		if (Done())
			return;

		var wave = waves.list [currentQueue];

		if(!wave.startedSpawn)
			wave.Spawn();

		if(wave.Done())
		{
			currentQueue++;
			if(currentQueue >= waves.list.Count)
				return;
		}
	}
}