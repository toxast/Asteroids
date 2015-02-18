using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsCreator
{

	public static Color defaultEnemyColor = new Color (0.5f, 0.5f, 0.5f);

	public static UserSpaceShip CreateSpaceShip()
	{
		var vr = Math2d.ScaleVertices2 (SpaceshipsData.fastSpaceshipVertices, 1f);
		var spaceship = PolygonCreator.CreatePolygonGOByMassCenter<UserSpaceShip>(vr, Color.blue);

		spaceship.SetCollisionLayer (GlobalConfig.ilayerUser);
		spaceship.collector = new DropCollector (0.15f, 20f);

		spaceship.gameObject.name = "Spaceship";
		#if UNITY_STANDALONE
		spaceship.SetController (new StandaloneInputController());
		
		//		tabletController.Init(flyZoneBounds);
		//		spaceship.SetController(tabletController);
		#else
		TODO
			spaceship.SetTabletControls (fireButton, accelerateButton);
		#endif

		InitGuns (spaceship, SpaceshipsData.alien5gunplaces, GunsData.SimpleGun);

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
		//var vr = Math2d.Chance (0.5f) ? Math2d.ScaleVertices2 (SpaceshipsData.alien6, 1.5f) : SpaceshipsData.alien3;
		var vr = Math2d.ScaleVertices2 (SpaceshipsData.butterflySpaceship, 1.1f);
		//var vr = Math2d.ScaleVertices2 (vrt, 1.5f);
		EnemySpaceShip spaceship = PolygonCreator.CreatePolygonGOByMassCenter<EnemySpaceShip> (vr, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		spaceship.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (spaceship);
		spaceship.gameObject.name = "enemy spaceship";

		InitGuns (spaceship, SpaceshipsData.alien9gunplaces, GunsData.SimpleGun2);

		SpaceshipData data = new SpaceshipData{
			thrust = 20f,
			maxSpeed = 15f,
			turnSpeed = 150f,
			brake = 8f,
			passiveBrake = 4f,
		}; 
		spaceship.Init(data);

		return spaceship;
	}

	public static EnemySpaceShip CreateFriendShip2()
	{
		//var vr = Math2d.Chance (0.5f) ? Math2d.ScaleVertices2 (SpaceshipsData.alien6, 1.5f) : SpaceshipsData.alien3;
		var vr = Math2d.ScaleVertices2 (SpaceshipsData.alien12, 1.1f);
		//var vr = Math2d.ScaleVertices2 (vrt, 1.5f);
		EnemySpaceShip spaceship = PolygonCreator.CreatePolygonGOByMassCenter<EnemySpaceShip> (vr, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		spaceship.SetCollisionLayer (GlobalConfig.ilayerTeamUser);
		DeathAnimation.MakeDeathForThatFellaYo (spaceship);
		spaceship.gameObject.name = "friend spaceship";
		
		InitGuns (spaceship, SpaceshipsData.alien9gunplaces, GunsData.SimpleGun2);
		
		SpaceshipData data = new SpaceshipData{
			thrust = 20f,
			maxSpeed = 15f,
			turnSpeed = 150f,
			brake = 8f,
			passiveBrake = 4f,
		}; 
		spaceship.Init(data);
		
		return spaceship;
	}

	public static EnemySpaceShip CreateBossEnemySpaceShip()
	{
		var vertices = PolygonCreator.GetCompleteVertexes (SpaceshipsData.halfBossVertices, 2).ToArray();
		EnemySpaceShip spaceship = PolygonCreator.CreatePolygonGOByMassCenter<EnemySpaceShip> (vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		spaceship.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		spaceship.gameObject.name = "boss";
		DeathAnimation.MakeDeathForThatFellaYo (spaceship);
		InitGuns (spaceship, SpaceshipsData.bossGunplaces, GunsData.SimpleGun2);

		SpaceshipData data = new SpaceshipData{
			thrust = 15f,
			maxSpeed = 5f,
			turnSpeed = 30f,
			brake = 8f,
			passiveBrake = 4f,
		}; 
		spaceship.Init(data);
		
		return spaceship;
	}


	public static RogueEnemy CreateRogueEnemy()
	{ 
		RogueEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<RogueEnemy>(RogueEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.gameObject.name = "RogueEnemy";
		enemy.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
//		float size = 1f;
//		ShootPlace place1 = ShootPlace.GetSpaceshipShootPlace();
//		ShootPlace place2 = ShootPlace.GetSpaceshipShootPlace();
//		Math2d.ScaleVertices(place1.vertices, size);
//		Math2d.ScaleVertices(place2.vertices, size);
//		place1.position = new Vector2(1.5f, 0.75f) * size;
//		place2.position = new Vector2(1.5f, -0.75f) * size;
//		List<ShootPlace> places = new List<ShootPlace>();
//		places.Add(place1);
//		places.Add(place2);
		enemy.Init ();

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
		asteroid.SetCollisionLayer (GlobalConfig.ilayerAsteroids);
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
		asteroid.SetCollisionLayer (GlobalConfig.ilayerAsteroids);
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
		tower.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		tower.gameObject.name = "tower";
		DeathAnimation.MakeDeathForThatFellaYo (tower);
		List<Place> gunplaces = new List<Place> ();
		for (int i = 0; i < cannons.Length; i++) 
		{
			Place place = new Place();
			place.pos = vertices[cannons[i]];
			place.dir = place.pos.normalized;
			//float angle = Math2d.AngleRAD2 (new Vector2 (1, 0), place.pos);
			//place.vertices = Math2d.RotateVerticesRAD(place.vertices, angle);
			gunplaces.Add(place);
		}

		InitGuns (tower, gunplaces, GunsData.RocketLauncher);
		
		tower.Init();

		return tower;
	}

	public static SimpleTower CreateSimpleTower(bool smartAim)
	{
		float r = 1f;//UnityEngine.Random.Range(1.0f, 1.2f);
		int sides = 6;

		Vector2[] vertices = PolygonCreator.CreateTowerVertices2 (r, sides);
		
		var enemy = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(vertices, defaultEnemyColor);
		enemy.gameObject.name = "tower1";
		enemy.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		List<Place> gunplaces = new List<Place>
		{
			new Place(new Vector2(2f, 0.0f), new Vector2(1.0f, 0f)),
		};

		InitGuns (enemy, gunplaces, GunsData.SimpleGun2);

		enemy.Init(smartAim);
		
		return enemy;
	}

	public static EvadeEnemy CreateEvadeEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		enemy.gameObject.name = "evade enemy";
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		InitGuns (enemy, EvadeEnemy.gunplaces, GunsData.SimpleGun2);

		enemy.Init (bullets);

		return enemy;
	}



	public static EvadeEnemy CreateTankEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(TankEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.SetCollisionLayer (GlobalConfig.ilayerTeamEnemies);
		List<Place> gunplaces = new List<Place>
		{
			new Place(new Vector2(1.5f, 0.75f), new Vector2(1.0f, 0f)),
			new Place(new Vector2(1.5f, -0.75f), new Vector2(1.0f, 0f)),
		};

		InitGuns (enemy, gunplaces, GunsData.TankGun);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		enemy.gameObject.name = "tank enemy";
		
		enemy.Init(bullets);
		
		return enemy;
	}

	public static Asteroid CreateGasteroid()
	{
		float size = Random.Range(3f, 5f);
		int vcount = Random.Range(5, 5 + (int)size);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		
		Gasteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Gasteroid>(vertices, Singleton<GlobalConfig>.inst.GasteroidColor);
		asteroid.SetCollisionLayer (GlobalConfig.ilayerAsteroids);
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
		asteroid.SetCollisionLayer (GlobalConfig.ilayerAsteroids);
		asteroid.Init ();
		asteroid.gameObject.name = "asteroid";
		
		return asteroid;
	}

	public static polygonGO.Drop CreateDrop()
	{
		float size = 0.7f;//Random.Range(1f, 2f);
		int vcount = 5;//Random.Range(5, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreatePrefectPolygonVertices(size, vcount);
		
		var drop = PolygonCreator.CreatePolygonGOByMassCenter<polygonGO.Drop>(vertices, Color.cyan);
		drop.SetCollisionLayer (GlobalConfig.ilayerMisc);
		drop.gameObject.name = "drop";
		drop.lifetime = 10f;
		
		return drop;
	}


	private static void InitGuns(PolygonGameObject enemy, List<Place> gunplaces, System.Func<Place, Transform, Gun> gunsGetter)
	{
		foreach (var gunplace in gunplaces) 
		{
			var gun = gunsGetter(gunplace, enemy.cacheTransform);
			enemy.guns.Add (gun);
		}
	}

}
