using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalConfig : MonoBehaviour{

	[SerializeField] public float GlobalHealthModifier = 1f;
	[SerializeField] public float SpaceshipHealthModifier = 1f;
	[SerializeField] public float TankEnemyHealthModifier = 1f;
	[SerializeField] public float SawEnemyHealthModifier = 1f;
	[SerializeField] public float TowerEnemyHealthModifier = 1f;
	[SerializeField] public float RogueEnemyHealthModifier = 1f;


	[SerializeField] public float DamageFromCollisionsModifier = 0.3f;


	[SerializeField] public Color GasteroidColor = Color.white;
	[SerializeField] public Color spaceshipEnemiesColor = Color.white;
	[SerializeField] public Color towerEnemiesColor = Color.white;

	[SerializeField] public ParticleSystem fireEffect;
	[SerializeField] public ParticleSystem fireEffect2;
	[SerializeField] public ParticleSystem thrusterEffect;

	[SerializeField] public List<ParticleSystem> smallDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> smallFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> mediumFinalDeathExplosionEffects;
	[SerializeField] public List<ParticleSystem> largeFinalDeathExplosionEffects;


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
		(int)(eLayer.TEAM_ENEMIES | eLayer.BULLETS_ENEMIES | eLayer.ASTEROIDS), 	
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
