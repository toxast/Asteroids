using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsCreator
{

	public static Color defaultEnemyColor = new Color (0.5f, 0.5f, 0.5f);

	public static UserSpaceShip CreateSpaceShip(Rect flyZoneBounds)
	{
		var spaceship = PolygonCreator.CreatePolygonGOByMassCenter<UserSpaceShip>(SpaceshipsData.fastSpaceshipVertices, Color.blue);
		
		spaceship.gameObject.name = "Spaceship";
		#if UNITY_STANDALONE
		spaceship.SetController (new StandaloneInputController(flyZoneBounds));
		
		//		tabletController.Init(flyZoneBounds);
		//		spaceship.SetController(tabletController);
		#else
		TODO
			spaceship.SetTabletControls (fireButton, accelerateButton);
		#endif
		
		List<ShootPlace> shooters = new List<ShootPlace>();
		
		ShootPlace place2 =  ShootPlace.GetSpaceshipShootPlace();
		place2.color = Color.red;
		place2.fireInterval = 0.25f;
		place2.position = new Vector2(2f, 0f);
		shooters.Add(place2);
		spaceship.SetShootPlaces(shooters);
		
		SpaceshipData data = new SpaceshipData{
			thrust = 45f,
			maxSpeed = 20f,
			turnSpeed = 220f,
			brake = 15f,
			passiveBrake = 2f,
		}; 
		spaceship.Init(data);
		
		return spaceship;
	}
	
	public static EnemySpaceShip CreateEnemySpaceShip()
	{
		EnemySpaceShip enemySpaceship = PolygonCreator.CreatePolygonGOByMassCenter<EnemySpaceShip> (SpaceshipsData.fastSpaceshipVertices, Color.white);
		
		enemySpaceship.gameObject.name = "enemy spaceship";
		
		List<ShootPlace> shooters = new List<ShootPlace> ();
		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace ();
		place2.color = Color.yellow;
		place2.fireInterval = 0.5f;
		place2.position = new Vector2 (2f, 0f);
		shooters.Add (place2);
		enemySpaceship.SetShootPlaces (shooters);

		SpaceshipData data = new SpaceshipData{
			thrust = 20f,
			maxSpeed = 15f,
			turnSpeed = 150f,
			brake = 8f,
			passiveBrake = 4f,
		}; 
		enemySpaceship.Init(data);

		return enemySpaceship;
	}

	public static EnemySpaceShip CreateBossEnemySpaceShip()
	{
		var vertices = PolygonCreator.GetCompleteVertexes (SpaceshipsData.halfBossVertices, 2).ToArray();
		EnemySpaceShip enemySpaceship = PolygonCreator.CreatePolygonGOByMassCenter<EnemySpaceShip> (vertices, Color.white);
		
		enemySpaceship.gameObject.name = "boss";
		
		List<ShootPlace> shooters = new List<ShootPlace> ();
		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace ();
		place2.color = Color.yellow;
		place2.fireInterval = 0.5f;
		place2.position = new Vector2 (2f, 0f);
		shooters.Add (place2);
		enemySpaceship.SetShootPlaces (shooters);
		
		SpaceshipData data = new SpaceshipData{
			thrust = 20f,
			maxSpeed = 5f,
			turnSpeed = 40f,
			brake = 8f,
			passiveBrake = 4f,
		}; 
		enemySpaceship.Init(data);
		
		return enemySpaceship;
	}


	public static RogueEnemy CreateRogueEnemy()
	{ 
		RogueEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<RogueEnemy>(RogueEnemy.vertices, defaultEnemyColor);
		enemy.gameObject.name = "RogueEnemy";
		
		float size = 1f;
		ShootPlace place1 = ShootPlace.GetSpaceshipShootPlace();
		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace();
		Math2d.ScaleVertices(place1.vertices, size);
		Math2d.ScaleVertices(place2.vertices, size);
		place1.position = new Vector2(1.5f, 0.75f) * size;
		place2.position = new Vector2(1.5f, -0.75f) * size;
		List<ShootPlace> places = new List<ShootPlace>();
		places.Add(place1);
		places.Add(place2);
		enemy.Init (places);

		return enemy;
	}

	public static SawEnemy CreateSawEnemy()
	{
		float rInner = UnityEngine.Random.Range(2f, 3f);
		float spikeLength = UnityEngine.Random.Range (0.8f, 1.5f);
		float rOuter = rInner + spikeLength;
		int spikesCount = UnityEngine.Random.Range((int)(rInner+5), (int)(rInner+10));
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SawEnemy asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SawEnemy>(vertices, defaultEnemyColor);
		asteroid.gameObject.name = "SawEnemy";
		asteroid.Init();

		return asteroid;
	}

	public static SpikyAsteroid CreateSpikyAsteroid()
	{
		float rInner = UnityEngine.Random.Range(2f, 4f);
		float spikeLength = UnityEngine.Random.Range (3f, 4f);
		float rOuter = rInner + spikeLength;
		int spikesCount = UnityEngine.Random.Range((int)(rInner+1), 9);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, defaultEnemyColor);
		asteroid.gameObject.name = "spiked asteroid";
		asteroid.Init(spikes);

		return asteroid;
	}

	public static TowerEnemy CreateTower()
	{
		float r = UnityEngine.Random.Range(3f, 4f);
		int sides = UnityEngine.Random.Range(3, 6);
		
		int[] cannons;
		Vector2[] vertices = PolygonCreator.CreateTowerPolygonVertices (r, r/5f, sides, out cannons);
		
		var tower = PolygonCreator.CreatePolygonGOByMassCenter<TowerEnemy>(vertices, defaultEnemyColor);
		tower.gameObject.name = "tower";
		
		List<ShootPlace> shooters = new List<ShootPlace> ();
		for (int i = 0; i < cannons.Length; i++) 
		{
			ShootPlace place = ShootPlace.GetSpaceshipShootPlace();
			place.fireInterval *= 3;
			place.position = vertices[cannons[i]];
			place.direction = place.position.normalized;
			float angle = Math2d.AngleRAD2 (new Vector2 (1, 0), place.position);
			place.vertices = Math2d.RotateVerticesRAD(place.vertices, angle);
			shooters.Add(place);
		}
		
		tower.Init(shooters);

		return tower;
	}

	public static SimpleTower CreateSimpleTower()
	{
		float r = 1f;//UnityEngine.Random.Range(1.0f, 1.2f);
		int sides = 6;
		
		Vector2[] vertices = PolygonCreator.CreateTowerVertices2 (r, sides);
		
		var tower = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(vertices, defaultEnemyColor);
		tower.gameObject.name = "tower1";
		
		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace ();
		place2.color = Color.yellow;
		place2.fireInterval = 0.5f;
		place2.position = new Vector2 (2f, 0f);
		tower.Init(place2, false);
		
		return tower;
	}

	public static EvadeEnemy CreateEvadeEnemy(List<BulletBase> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, defaultEnemyColor);
		
		ShootPlace place = ShootPlace.GetSpaceshipShootPlace();
		place.fireInterval *= 3;
		Math2d.ScaleVertices(place.vertices, 1f);
		enemy.gameObject.name = "evade enemy";
		
		enemy.Init(bullets, place);

		return enemy;
	}

	public static TankEnemy CreateTankEnemy(List<BulletBase> bullets)
	{
		TankEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<TankEnemy>(TankEnemy.vertices, defaultEnemyColor);
		
		float size = 2f;
		
		ShootPlace place1 = ShootPlace.GetSpaceshipShootPlace();
		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace();
		Math2d.ScaleVertices(place1.vertices, size);
		Math2d.ScaleVertices(place2.vertices, size);
		place1.position = new Vector2(1.5f, 0.75f) * size;
		place2.position = new Vector2(1.5f, -0.75f) * size;
		List<ShootPlace> places = new List<ShootPlace>();
		places.Add(place1);
		places.Add(place2);
		enemy.gameObject.name = "tank enemy";
		
		enemy.Init(bullets, places);
		
		return enemy;
	}

	public static Asteroid CreateGasteroid()
	{
		float size = Random.Range(3f, 5f);
		int vcount = Random.Range(5, 5 + (int)size);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		
		Gasteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Gasteroid>(vertices, Singleton<GlobalConfig>.inst.GasteroidColor);
		asteroid.Init ();
		asteroid.gameObject.name = "gasteroid";

		return asteroid;
	}
	
	public static Asteroid CreateAsteroid()
	{
		float size = Random.Range(3f, 8f);
		int vcount = Random.Range(5, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, defaultEnemyColor);
		asteroid.Init ();
		asteroid.gameObject.name = "asteroid";
		
		return asteroid;
	}

}
