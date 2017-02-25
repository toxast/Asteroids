using UnityEngine;
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
