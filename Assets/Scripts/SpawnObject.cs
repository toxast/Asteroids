using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
	public bool overrideAmount = false;
	public int keepEnemiesAmount;
	public int count = 1;
	public ObjectType type = ObjectType.eAsteroid;
	public int indx;
	public float spawnInterval = 0;
	public float teleportDuration = 1.5f;
	public float teleportRingSize = 10f;
	public Color teleportationColor = new Color (1, 174f / 255f, 0);
	public Vector2 spawnRange = new Vector2(40,80);
	public SpawnPositioning positioning;


	public PolygonGameObject Spawn(Vector2 pos, float lookAngle)
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

			obj.cacheTransform.position = pos;
			obj.cacheTransform.rotation = Quaternion.Euler (0, 0, lookAngle);

//			Singleton<Main>.inst.SetRandomPosition(obj, spawnRange, positioning);
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
	[NonSerialized] private int keepEnemiesAmount;
//	public List<SpawnObject> secondary;
	public int doneWhenLeft = 0;

	[NonSerialized] private int shouldSpawn = 100;
	[NonSerialized] private int spawningNow = 0;
	[NonSerialized] private int spawnedCount = 0;
	[NonSerialized] public bool startedSpawn = false;
	[NonSerialized] public bool finishedSpawn = false;
	[NonSerialized] private List<PolygonGameObject> spawned = new List<PolygonGameObject>();

	public bool Done()
	{
		if (!finishedSpawn)
			return false;

		var left = Left ();
		return left <= doneWhenLeft;
	}

	private int Left()
	{
		int left = spawningNow;
		for (int i = spawned.Count - 1; i >= 0; i--) 
		{
			if(!Main.IsNull(spawned[i]))
			{
				left++;
			}
			else
			{
				spawned.RemoveAt(i);
			}
		} 
		return left;
	}

	public void Spawn()
	{
		if(!startedSpawn)
		{
			shouldSpawn = 0;
			foreach (var item in objects) {
				shouldSpawn += item.count;
			}
			Singleton<Main>.inst.StartCoroutine(SpawnRoutine());
		}
	}

	private IEnumerator SpawnRoutine()
	{
		startedSpawn = true;
//		foreach (var item in objects) 
//		{
//			for (int i = 0; i < item.count; i++) 
//			{
//				if(item.spawnInterval > 0)
//				{
//					yield return new WaitForSeconds(item.spawnInterval);
//				}
//				spawned.Add(item.Spawn());
//			}
//		}

		if(objects.Any())
		{
			int curList = 0;
			int objCounter = 0;
			keepEnemiesAmount = objects[0].keepEnemiesAmount;

			while(curList < objects.Count)
			{
				while(true)
				{
					if(curList >= objects.Count)
						break;

					var item = objects[curList];

					if(item.overrideAmount)
						keepEnemiesAmount = item.keepEnemiesAmount;

					var left = Left();
					if(left >= keepEnemiesAmount)
						break;

					if(objCounter >= item.count)
					{
						curList++;
						objCounter = 0;
						continue;
					}

					objCounter ++;
					Vector2 pos;
					float lookAngle;
					Singleton<Main>.inst.GetRandomPosition(item.spawnRange, item.positioning, out pos, out lookAngle);
					if(item.spawnInterval > 0)
					{
						yield return new WaitForSeconds(item.spawnInterval);
					}
					if(item.teleportDuration > 0)
					{
						spawningNow++;
						Singleton<Main>.inst.StartCoroutine(SpawnObjectWithTeleportAnimation(item, pos, lookAngle));
					}
					else
					{
						spawned.Add(item.Spawn(pos, lookAngle));
						spawnedCount ++;
					}
				}
				yield return new WaitForSeconds(1f);
			}
		}

		while(spawnedCount < shouldSpawn)
		{
			yield return new WaitForSeconds(0.5f);
		}

		finishedSpawn = true;
		yield break;
	}

	private IEnumerator SpawnObjectWithTeleportAnimation(SpawnObject item, Vector2 pos, float lookAngle)
	{
		var anim = Singleton<Main>.inst.CreateTeleportationRing(pos);

		if (item.type != ObjectType.eAsteroid)
			anim.startColor = item.teleportationColor;
		else
			anim.startColor = Color.gray;

		anim.startSize = item.teleportRingSize;
		yield return new WaitForSeconds(item.teleportDuration);
		anim.Stop ();
		spawned.Add(item.Spawn(pos, lookAngle));
		spawnedCount ++;
		Singleton<Main>.inst.PutObjectOnDestructionQueue (anim.gameObject, 5f);
		spawningNow--;
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