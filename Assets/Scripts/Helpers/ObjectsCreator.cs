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
		T spaceship = PolygonCreator.CreatePolygonGOByMassCenter<T> (sdata.verts, sdata.color);
		spaceship.Init(sdata.density);
		spaceship.Init(sdata.physicalParameters);
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

	public static UserSpaceShip CreateSpaceShip(InputController contorller, int indx)
	{
		var spaceship = ObjectsCreator.CreateSpaceShip<UserSpaceShip> (indx);
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
		var turret = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(data.verts, data.color);
		turret.Init (1);
		turret.Init (smartAim, anglesRestriction);
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
		enemy.Init (1);
		enemy.Init ();
		enemy.gameObject.name = "RogueEnemy";
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);

		return enemy;
	}

	public static Asteroid CreateAsteroid(AsteroidData data, AsteroidInitData initData, Material mat)
	{
		float size = Random(initData.size);
		int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Singleton<GlobalConfig>.inst.AsteroidColor, mat);
		asteroid.Init (data.density);
		asteroid.Init (initData.speed, initData.rotation);
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
		asteroid.gameObject.name = "asteroid";
		
		return asteroid;
	}

	public static SawEnemy CreateSawEnemy(SpikyInitData initData)
	{
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SawEnemy asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SawEnemy>(vertices, defaultEnemyColor);
		asteroid.Init (1);
		asteroid.Init (initData.speed, initData.rotation);
		asteroid.Init ();
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		asteroid.gameObject.name = "SawEnemy";

		return asteroid;
	}

	public static SpikyAsteroid CreateSpikyAsteroid(SpikyInitData initData)
	{
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount); //UnityEngine.Random.Range((int)(rInner+1), 9);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, defaultEnemyColor);
		asteroid.Init (1);
		asteroid.Init (initData.speed, initData.rotation);
		asteroid.Init (spikes);
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		asteroid.gameObject.name = "spiked asteroid";

		return asteroid;
	}

	public static Asteroid CreateGasteroid(AsteroidInitData initData)
	{
		float size = UnityEngine.Random.Range(3f, 7f);
		int vcount = UnityEngine.Random.Range(5, 5 + (int)size);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		
		Gasteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Gasteroid>(vertices, Singleton<GlobalConfig>.inst.GasteroidColor);
		asteroid.Init (1);
		asteroid.Init (initData.speed, initData.rotation);
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
		tower.Init (1);
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
		
		tower.Init();

		return tower;
	}

	public static SimpleTower CreateSimpleTower(bool smartAim, System.Func<Vector3> restrict)
	{
		float r = 1f;//UnityEngine.Random.Range(1.0f, 1.2f);
		int sides = 6;

		Vector2[] vertices = PolygonCreator.CreateTowerVertices2 (r, sides);
		
		var enemy = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(vertices, defaultEnemyColor);
		enemy.Init (1);
		enemy.gameObject.name = "tower1";
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		List<Place> gunplaces = new List<Place>
		{
			new Place(new Vector2(2f, 0.0f), new Vector2(1.0f, 0f)),
		};

		InitGuns (enemy, gunplaces, GunsData.SimpleGun2);
		enemy.Init(smartAim, restrict);
		
		return enemy;
	}

	public static EvadeEnemy CreateEvadeEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.Init (1);
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
		enemy.gameObject.name = "evade enemy";
		DeathAnimation.MakeDeathForThatFellaYo (enemy);
		InitGuns (enemy, EvadeEnemy.gunplaces, GunsData.SimpleGun2);

		enemy.Init (bullets);

		return enemy;
	}



	public static EvadeEnemy CreateTankEnemy(List<IBullet> bullets)
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(TankEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
		enemy.Init (1);
		enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
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
		drop.Init (1);
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
