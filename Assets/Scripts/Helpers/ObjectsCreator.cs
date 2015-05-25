using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsCreator
{

	public static Color defaultEnemyColor = new Color (0.5f, 0.5f, 0.5f);

	public static T CreateSpaceShip<T>(int dataIndex)
		where T:SpaceShip
	{
		var sdata = SpaceshipsResources.Instance.spaceships [dataIndex];
		return CreateSpaceShip<T> (sdata);
	}

	public static T CreateSpaceShip<T>(FullSpaceShipSetupData sdata)
		where T:SpaceShip
	{
		T spaceship = PolygonCreator.CreatePolygonGOByMassCenter<T> (sdata.verts, sdata.color);
		spaceship.InitSpaceShip(sdata.physical, sdata.mobility);
		spaceship.SetThrusters (sdata.thrusters);

		if (sdata.shield != null && sdata.shield.capacity > 0)
			spaceship.SetShield (sdata.shield);

		DeathAnimation.MakeDeathForThatFellaYo (spaceship);
		spaceship.gameObject.name = sdata.name; 
		
		spaceship.guns = new List<Gun> ();
		foreach (var gunplace in sdata.guns) 
		{
			var gun = GunsData.GetGun(gunplace, spaceship);
			spaceship.guns.Add (gun);
		}

		foreach (var item in sdata.turrets) 
		{
			CreateTurret(spaceship, item, SpaceshipsResources.Instance.turrets[item.index]);
		}

		return spaceship;
	}

	public static UserSpaceShip CreateSpaceShip(InputController contorller, FullSpaceShipSetupData data)
	{
		var spaceship = ObjectsCreator.CreateSpaceShip<UserSpaceShip> (data);
		spaceship.SetCollisionLayerNum (CollisionLayers.ilayerUser);
		spaceship.collector = new DropCollector (0.15f, 20f);
		spaceship.SetColor (Color.blue);
		spaceship.gameObject.name = "Spaceship";
		spaceship.SetShield(new ShieldData(10f,2f,2f));
		spaceship.SetController (contorller);
		return spaceship;
	}

	private static void CreateTurret(PolygonGameObject parent, TurretReferenceData pos, TurretSetupData data)
	{
		bool smartAim = true;
		var copyAngle = data.restrictionAngle * 0.5f;
		var copyDir = pos.place.dir;
		System.Func<Vector3> anglesRestriction = () =>
		{
			float angle = parent.cacheTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			Vector2 dir = Math2d.RotateVertex(copyDir, angle);
			Vector3 result = dir;
			result.z = copyAngle;
			return result;
		}; 
		var turret = PolygonCreator.CreatePolygonGOByMassCenter<Turret>(data.verts, data.color);
		turret.InitTurret (new PhysicalData(), smartAim, data.rotationSpeed, anglesRestriction);
		turret.guns = new List<Gun> ();
		foreach (var gunplace in data.guns) 
		{
			var gun = GunsData.GetGun(gunplace, turret);
			turret.guns.Add (gun);
		}
		turret.targetSystem = new TurretTargetSystem(turret, data.rotationSpeed, anglesRestriction);
		parent.AddTurret(pos.place, turret);
	}

	public static RogueEnemy CreateRogueEnemy()
	{ 
		RogueEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<RogueEnemy>(RogueEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.Init ();
		enemy.gameObject.name = "RogueEnemy";
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);

		return enemy;
	}



	public static Asteroid CreateAsteroid(AsteroidSetupData dataPh, AsteroidData data, Material mat)
	{
		float size = Random(dataPh.size);
		int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Singleton<GlobalConfig>.inst.AsteroidColor, mat);

		//todo: include physical into asteroids?
		var ph = new PhysicalData ();
		ph.density = data.density;

		asteroid.InitAsteroid (ph, dataPh.speed, dataPh.rotation); 
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
		asteroid.gameObject.name = "asteroid";
		
		return asteroid;
	}

	public static SawEnemy CreateSawEnemy(SawInitData initData)
	{
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SawEnemy asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SawEnemy>(vertices, defaultEnemyColor);
		asteroid.InitSawEnemy (initData);
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		asteroid.gameObject.name = "SawEnemy";

		return asteroid;
	}

	public static SpikyAsteroid CreateSpikyAsteroid(SpikeShooterInitData initData)
	{
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount); //UnityEngine.Random.Range((int)(rInner+1), 9);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, defaultEnemyColor);
		asteroid.InitSpikyAsteroid (spikes, initData);
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		asteroid.gameObject.name = "spiked asteroid";

		return asteroid;
	}

	public static Gasteroid CreateGasteroid(AsteroidSetupData initData)
	{
		float size = Random (initData.size);
		int vcount = UnityEngine.Random.Range(5, 5 + (int)size);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		
		Gasteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Gasteroid>(vertices, Singleton<GlobalConfig>.inst.GasteroidColor);
		asteroid.InitAsteroid (new PhysicalData(), initData.speed, initData.rotation);
		asteroid.destructionType = PolygonGameObject.DestructionType.eComplete;
		DeathAnimation.MakeDeathForThatFellaYo (asteroid, true);
		asteroid.deathAnimation.finalExplosionPowerKoeff = 1.3f;
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
		asteroid.gameObject.name = "gasteroid";
		
		return asteroid;
	}

	private static float Random(RandomFloat r)
	{
		return UnityEngine.Random.Range(r.min, r.max);
	}

	private static int Random(RandomInt r)
	{
		return UnityEngine.Random.Range(r.min, r.max);
	}

	public static TowerEnemy CreateTower()
	{
		float r = UnityEngine.Random.Range(7f, 12f);
		int sides = UnityEngine.Random.Range(5, 8);
		
		int[] cannons;
		Vector2[] vertices = PolygonCreator.CreateTowerPolygonVertices (r, r/7f, sides, out cannons);
		
		var tower = PolygonCreator.CreatePolygonGOByMassCenter<TowerEnemy>(vertices, Singleton<GlobalConfig>.inst.towerEnemiesColor);
		tower.InitTowerEnemy ();
		tower.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
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
		
		return tower;
	}

	public static SimpleTower CreateSimpleTower(TowerSetupData data)
	{
		bool smartAim = true;
		System.Func<Vector3> anglesRestriction = () =>
		{
			Vector2 dir = new Vector2(1,0);
			Vector3 result = dir;
			result.z = 180;
			return result;
		}; 
		var turret = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(data.verts, data.color);
		turret.InitSimpleTower (data.physical, smartAim, data.rotationSpeed);
		turret.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		turret.guns = new List<Gun> ();
		foreach (var gunplace in data.guns) 
		{
			var gun = GunsData.GetGun(gunplace, turret);
			turret.guns.Add (gun);
		}
		turret.targetSystem = new TurretTargetSystem(turret, data.rotationSpeed, anglesRestriction);
		DeathAnimation.MakeDeathForThatFellaYo (turret);

		if (data.shield != null && data.shield.capacity > 0)
			turret.SetShield (data.shield);

		foreach (var item in data.turrets) 
		{
			CreateTurret(turret, item, SpaceshipsResources.Instance.turrets[item.index]);
		}

		return turret;
	}

	public static EvadeEnemy CreateEvadeEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.InitEvadeEnemy (new PhysicalData(), bullets);
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		enemy.gameObject.name = "evade enemy";
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		InitGuns (enemy, EvadeEnemy.gunplaces, GunsData.SimpleGun2);

		return enemy;
	}



	public static EvadeEnemy CreateTankEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(TankEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.InitEvadeEnemy (new PhysicalData(), bullets);
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		List<Place> gunplaces = new List<Place>
		{
			new Place(new Vector2(1.5f, 0.75f), new Vector2(1.0f, 0f)),
			new Place(new Vector2(1.5f, -0.75f), new Vector2(1.0f, 0f)),
		};

		InitGuns (enemy, gunplaces, GunsData.TankGun);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		enemy.gameObject.name = "tank enemy";
		
		return enemy;
	}



	private static List<Material> _asteroidsMaterials;
	public static Material GetAsteroidMaterial(int n)
	{
		if(_asteroidsMaterials == null)
		{
			_asteroidsMaterials = new List<Material>();
			for (int i = 0; i < AsteroidsResources.Instance.asteroidsData.Count; i++) 
			{
				var mat = GameObject.Instantiate (PolygonCreator.texturedMaterial) as Material;
				mat.color = AsteroidsResources.Instance.asteroidsData[i].color;
				_asteroidsMaterials.Add(mat);
			}
		}
		return _asteroidsMaterials[n];
	}

	public static polygonGO.Drop CreateDrop(DropData data)
	{
		float size = 0.7f;//Random.Range(1f, 2f);
		int vcount = 5;//Random.Range(5, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(size, vcount);
		
		var drop = PolygonCreator.CreatePolygonGOByMassCenter<polygonGO.Drop>(vertices, data.asteroidData.color);
		drop.InitPolygonGameObject (new PhysicalData());
		drop.SetCollisionLayerNum (CollisionLayers.ilayerMisc);
		drop.data = data;
		drop.gameObject.name = "drop";
		drop.lifetime = 10f;
		
		return drop;
	}

	private static void InitGuns(PolygonGameObject enemy, List<Place> gunplaces, System.Func<Place, IPolygonGameObject, Gun> gunsGetter)
	{
		foreach (var gunplace in gunplaces) 
		{
			var gun = gunsGetter(gunplace, enemy);
			enemy.guns.Add (gun);
		}
	}

}
