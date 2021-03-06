﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class CollisionLayers
{
	//TODO: separate file
	public const int ilayerUser = 0;
	public const int ilayerTeamUser = 1;
	public const int ilayerTeamEnemies = 2;
	public const int ilayerBulletsUser = 3;
	public const int ilayerBulletsEnemies = 4;
	public const int ilayerAsteroids = 5;
	public const int ilayerMisc = 6;
	public const int ilayerNoCollision = 7;


	public static List<int> enemyLayers = new List<int> {
		CollisionLayers.ilayerTeamEnemies,
		CollisionLayers.ilayerBulletsEnemies,
		CollisionLayers.ilayerAsteroids
	};

	public static List<int> friendlyLayers = new List<int> {
		CollisionLayers.ilayerTeamUser,
		CollisionLayers.ilayerUser,
		CollisionLayers.ilayerBulletsUser,
	};

	public enum eLayerNum : int
	{
		SAME = -1, //for default or no override behaviour
		USER = ilayerUser,
		TEAM_USER = ilayerTeamUser,
		TEAM_ENEMIES = ilayerTeamEnemies,
		BULLETS_USER = ilayerBulletsUser,
		BULLETS_ENEMIES = ilayerBulletsEnemies,
		ASTEROIDS = ilayerAsteroids,
		MISC = ilayerMisc,
		NO_COLISION = ilayerNoCollision,
	}

	[System.Flags]
	public enum eLayer : int
	{
		USER = 1 << ilayerUser,
		TEAM_USER = 1 << ilayerTeamUser,
		TEAM_ENEMIES = 1 << ilayerTeamEnemies,
		BULLETS_USER = 1 << ilayerBulletsUser,
		BULLETS_ENEMIES = 1 << ilayerBulletsEnemies,
		ASTEROIDS = 1 << ilayerAsteroids,
		MISC = 1 << ilayerMisc,
		NO_COLISION = 1 << ilayerNoCollision,
	}

	[Serializable]
	public class TimeMultipliersData
	{
		public float duration = 10f;

		public float user = 1;
		public float userTeam = 1;
		public float userBullets = 1;

		public float enemiesTeam = 1;
		public float enemiesBullets = 1;

		//layers should not intersect
		public const int userLayer = (int)eLayer.USER;
		public const int miscLayer = (int)eLayer.MISC;
		public const int userTeamLayer = (int)eLayer.TEAM_USER;
		public const int userBulletsLayer = (int)eLayer.BULLETS_USER;

		//layers should not intersect
		public const int enemiesTeamLayer = (int)eLayer.TEAM_ENEMIES | (int)eLayer.ASTEROIDS;
		public const int enemiesBulletsLayer = (int)eLayer.BULLETS_ENEMIES;
	}


	static public int GetBulletLayerNum(int parentLayer)
	{
		if(parentLayer == (int)CollisionLayers.eLayer.USER || parentLayer == (int)CollisionLayers.eLayer.TEAM_USER)
		{
			return CollisionLayers.ilayerBulletsUser;
		}
		else if(parentLayer == (int)CollisionLayers.eLayer.TEAM_ENEMIES)
		{
			return CollisionLayers.ilayerBulletsEnemies;
		}
		else
		{
			Debug.LogError("wtf layer");
			return 0;
		}
	}

	static public PolygonGameObject SpawnObjectFriendlyToParent(PolygonGameObject parent, MSingleSpawn spawn){
		return spawn.Create(GetLayerFriendlyToParent(parent, (int)spawn.iGameSpawnLayer));
	}

	static public int GetLayerFriendlyToParent(PolygonGameObject parent, int defaultLayerNum){
		int parentLogicNum = parent.logicNum;
		int spawnLayerNum = defaultLayerNum;

		if (enemyLayers.Contains (parentLogicNum) && enemyLayers.Contains (spawnLayerNum)) {
			return spawnLayerNum;
		} else if (friendlyLayers.Contains (parentLogicNum) && friendlyLayers.Contains (spawnLayerNum)) {
			return spawnLayerNum;
		} else {
			if (enemyLayers.Contains (parentLogicNum)) {
				return ilayerTeamEnemies;
			} else if (friendlyLayers.Contains (parentLogicNum)) {
				return ilayerTeamUser;
			} else {
				Debug.LogError ("wtf is that spawn: " + parent.name + " " + parentLogicNum + " " + spawnLayerNum);
				return spawnLayerNum;
			}
		}
	}

	static public int GetSpawnedLayer(int parentLayer)
	{
		if(parentLayer == (int)CollisionLayers.eLayer.USER || parentLayer == (int)CollisionLayers.eLayer.TEAM_USER)
		{
			return CollisionLayers.ilayerTeamUser;
		}
		else if(parentLayer == (int)CollisionLayers.eLayer.TEAM_ENEMIES)
		{
			return CollisionLayers.ilayerTeamEnemies;
		}
		else
		{
			Debug.LogError("wtf layer");
			return 0;
		}
	}

	static public int GetCollisionLayer(int parentLayer)
	{
		if(parentLayer == (int)CollisionLayers.eLayer.USER || parentLayer == (int)CollisionLayers.eLayer.TEAM_USER)
		{
			return CollisionLayers.ilayerTeamUser;
		}
		else if(parentLayer == (int)CollisionLayers.eLayer.TEAM_ENEMIES)
		{
			return CollisionLayers.ilayerTeamEnemies;
		}
		else
		{
			Debug.LogError("wtf layer");
			return 0;
		}
	}
	
	//first is one with most priority
	static public int GetEnemyLayers(int layer)
	{
		int enemyLayer = 0;;
		if ((layer & (1 << CollisionLayers.ilayerBulletsUser | 1 << CollisionLayers.ilayerTeamUser | 1 << CollisionLayers.ilayerUser)) != 0) {
			enemyLayer = 1 << CollisionLayers.ilayerTeamEnemies | 1 << CollisionLayers.ilayerAsteroids;
		} else if ((layer & (1 << CollisionLayers.ilayerBulletsEnemies | 1 << CollisionLayers.ilayerTeamEnemies)) != 0) {
			enemyLayer = 1 << CollisionLayers.ilayerTeamUser | 1 << CollisionLayers.ilayerUser;
		} else if ((layer & (1 << CollisionLayers.ilayerAsteroids)) != 0) {
			enemyLayer = 1 << CollisionLayers.ilayerUser;
		} else {
			Debug.LogError ("no enemies with layer: " + layer);
		}
		
		return enemyLayer;
	}

	
	
	//	private const int layerUser = 1 << ilayerUser;
	//	private const int layerTeamUser = 1 << ilayerTeamUser;
	//	private const int layerTeamEnemies = 1 << ilayerTeamEnemies;
	//	private const int layerBulletsUser = 1 << ilayerBulletsUser;
	//	private const int layerBulletsEnemies = 1 << ilayerBulletsEnemies;
	//	private const int layerAsteroids = 1 << ilayerAsteroids;
	//	private const int layerMisc = 1 << ilayerMisc;
	
	public static List<int> fullCollisions;
	public static List<int> halfMatrixCollisions = new List<int>
	{
		//layerUser
		(int)(eLayer.TEAM_ENEMIES | eLayer.BULLETS_ENEMIES | eLayer.ASTEROIDS | eLayer.MISC), 
		//layerTeamUser
		(int)(eLayer.TEAM_ENEMIES | eLayer.BULLETS_ENEMIES), 	
		//layerTeamEnemies
		(int)eLayer.BULLETS_USER, //+ layerAsteroids, 
		//layerBulletsUser
		(int)eLayer.ASTEROIDS,
		//layerBulletsEnemies
		(int)eLayer.ASTEROIDS,
		//layerAsteroids
		0,
		//layerMisc
		0,
		//no collision
		0,
	};

	public static int GetGravityBulletCollisions(int bulletLayer) {
		if (bulletLayer == ilayerBulletsUser) {
			return (int)(eLayer.TEAM_ENEMIES | eLayer.BULLETS_ENEMIES | eLayer.ASTEROIDS); 
		} else if(bulletLayer == ilayerBulletsEnemies) {
			return (int)(eLayer.TEAM_USER| eLayer.BULLETS_USER); 
		} else {
			Debug.LogError ("unknown gravity Bullet layer " + bulletLayer);
			return 0;
		}
	}
	 

	public static int GetLayerCollisions(int layerNum)
	{
		if(fullCollisions == null)
		{
			fullCollisions = new List<int>();
			for (int i = 0; i < halfMatrixCollisions.Count; i++) 
			{
				int collisionMask = 0;
				int layer = 1 << i;
				
				for (int k = 0; k < halfMatrixCollisions.Count; k++)
				{
					if(k != i)
					{
						if((halfMatrixCollisions[k] & layer) != 0)
						{
							collisionMask |= 1 << k;
						}
					}
					else
					{
						collisionMask |= halfMatrixCollisions [k];
					}
				}
				fullCollisions.Add(collisionMask);
				//Debug.LogWarning(GetIntBinaryString(collisionMask));
			}
		}
		
		return fullCollisions[layerNum];
	}
	
	static string GetIntBinaryString(int n)
	{
		char[] b = new char[32];
		int pos = 31;
		int i = 0;
		
		while (i < 32)
		{
			if ((n & (1 << i)) != 0)
			{
				b[pos] = '1';
			}
			else
			{
				b[pos] = '0';
			}
			pos--;
			i++;
		}
		return new string(b);
	}
}
