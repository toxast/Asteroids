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
public class SpawnObject 
{
	public ObjectType type = ObjectType.eAsteroid;
	public int indx;
	public int count = 1;
	public float spawnInterval = 0;

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
		default:
			break;
		}

		if(obj == null)
		{
			Debug.LogWarning("null " + type + " " + indx);
		}
		return obj;
	}
}

[Serializable]
public class SpawnWave
{
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
	public List<SpawnWave> list;
}

[Serializable]
public class LevelSpawner
{
	public SpawnWaves waves;
	[NonSerialized] int currentQueue = -1;

	public LevelSpawner(SpawnWaves waves)
	{
		this.waves = waves;
		currentQueue = -1;
	}

	public bool Done()
	{
		return (currentQueue == waves.list.Count - 1) && waves.list [currentQueue].Done ();
	}

	public void Tick()
	{
		if (Done ())
			return;

		if(currentQueue < 0 || waves.list[currentQueue].Done())
		{
			currentQueue++;
			if(currentQueue >= waves.list.Count)
				return;

			if(!waves.list[currentQueue].startedSpawn)
				waves.list[currentQueue].Spawn();
		}
	}
}