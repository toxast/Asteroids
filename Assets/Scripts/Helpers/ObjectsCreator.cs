using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsCreator
{

	public static Color defaultEnemyColor = new Color (0.5f, 0.5f, 0.5f);

	public static T CreateSpaceship<T>(MSpaceshipData sdata, int layerNum)
		where T:SpaceShip
	{
		var spaceship = MCreateSpaceShip<T>(sdata, layerNum);
		var bullets = Singleton<Main>.inst.bullets;
		var controller = new CommonController (spaceship, bullets, spaceship.guns [0], sdata.accuracy);
        spaceship.SetController (controller);

		return spaceship;
	}

	public static T CreateInvisibleSpaceship<T>(MInvisibleSpaceshipData sdata, int layerNum)
		where T:SpaceShip
	{
		var spaceship = MCreateSpaceShip<T>(sdata, layerNum);
		var bullets = Singleton<Main>.inst.bullets;
        var controller = new InvisibleSpaceshipController(spaceship, bullets, spaceship.guns[0], sdata.accuracy, sdata.invisibleData);
		spaceship.SetController(controller);
		return spaceship;
	}

	public static T CreateEarthSpaceship<T>(MEarthSpaceshipData sdata, int layerNum)
		where T:SpaceShip
	{
		var spaceship = MCreateSpaceShip<T>(sdata, layerNum);
		var controller = new EarthSpaceshipController(spaceship, Singleton<Main>.inst.gObjects, sdata);
		spaceship.SetController(controller);
		return spaceship;
	}

    public static T CreateChargerSpaceship<T>(MChargerSpaseshipData sdata, int layerNum)
        where T : SpaceShip {
        var spaceship = MCreateSpaceShip<T>(sdata, layerNum);
        var bullets = Singleton<Main>.inst.bullets;
        var controller = new ChargerController(spaceship, bullets, sdata.accuracy, sdata.chargerData);
        spaceship.SetController(controller);
        return spaceship;
    }

    public static T CreateSuisideBombSpaceship<T>(MSuicideBombSpaceshipData sdata, int layerNum)
       where T : SpaceShip {
        var spaceship = MCreateSpaceShip<T>(sdata, layerNum);
        var bullets = Singleton<Main>.inst.bullets;
        var controller = new MSuicideBombController(spaceship, bullets, sdata);
        spaceship.SetController(controller);
        return spaceship;
    }

    public static UserSpaceShip CreateSpaceShip(InputController contorller, MSpaceshipData data)
	{
		var spaceship = ObjectsCreator.MCreateSpaceShip<UserSpaceShip> (data, CollisionLayers.ilayerUser);
		spaceship.collector = new DropCollector (0.15f, 20f);
		spaceship.SetColor (Color.blue);
        spaceship.gameObject.name = "User_Spaceship " + data.name;
		spaceship.SetController (contorller);
		return spaceship;
	}

	private static T MCreateSpaceShip<T>(MSpaceshipData sdata, int layer)
		where T:SpaceShip
	{
		T spaceship = PolygonCreator.CreatePolygonGOByMassCenter<T> (sdata.verts, sdata.color);
		spaceship.reward = sdata.reward;
		spaceship.InitSpaceShip(sdata.physical, sdata.mobility);
		spaceship.SetThrusters (sdata.thrusters);

        if (sdata.shield != null && sdata.shield.capacity > 0)
			spaceship.SetShield (sdata.shield);

        var deathData = sdata.deathData;
        spaceship.destructionType = deathData.destructionType;
        spaceship.overrideExplosionDamage = deathData.overrideExplosionDamage;
        spaceship.overrideExplosionRange = deathData.overrideExplosionRange;
        if (deathData.createExplosionOnDeath) {
            DeathAnimation.MakeDeathForThatFellaYo(spaceship, deathData.instantExplosion);
        }

		spaceship.gameObject.name = sdata.name; 

		var guns = new List<Gun> ();
		foreach (var gunplace in sdata.guns) 
		{
			var gun = GunsData.GetGun(gunplace, spaceship);
			guns.Add (gun);
		}
		spaceship.SetGuns (guns, sdata.linkedGuns);

		foreach (var item in sdata.turrets) 
		{
			CreateTurret(spaceship, item);
		}
		spaceship.SetCollisionLayerNum (layer);

		return spaceship;
	}

	private static void CreateTurret(PolygonGameObject parent, MTurretReferenceData pos)
	{
		MTurretData data = pos.turret;
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
		turret.InitTurret (new PhysicalData(), data.rotationSpeed, anglesRestriction);
		var guns = new List<Gun> ();
		foreach (var gunplace in data.guns) 
		{
			var gun = GunsData.GetGun(gunplace, turret);
			guns.Add (gun);
		}
		turret.SetGuns (guns, data.linkedGuns);

		turret.targetSystem = new TurretTargetSystem(turret, data.rotationSpeed, anglesRestriction, data.repeatTargetCheck);
		parent.AddTurret(pos.place, turret);
	}

	
	public static Asteroid CreateAsteroid(MAsteroidData mdata)
	{
		float size = Random(mdata.size);
		int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Singleton<GlobalConfig>.inst.AsteroidColor, ObjectsCreator.GetAsteroidMaterial(mdata.commonData.asteroidMaterialIndex));

		//todo: include physical into asteroids?
		var ph = new PhysicalData ();
		ph.density = mdata.commonData.density;
		ph.healthModifier = mdata.commonData.density;

		asteroid.InitAsteroid (ph, mdata.speed, mdata.rotation); 
		asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
        asteroid.gameObject.name = mdata.name;

		return asteroid;
	}

    public static Comet CreateComet(MCometData mdata) {
        float size = Random(mdata.size);
        int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size * 3);
        Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size / 2f, vcount);
        Comet asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Comet>(vertices, mdata.color);
        asteroid.InitAsteroid(mdata.physical, mdata.speed, mdata.rotation);
        asteroid.InitComet(mdata.powerupData, mdata.lifeTime);
        var ps = GameObject.Instantiate<ParticleSystem>(mdata.particles, asteroid.transform);
        ps.startColor = mdata.particleSystemColor;
        ps.transform.localPosition = new Vector3(0, 0, -1);
        asteroid.SetCollisionLayerNum(CollisionLayers.ilayerTeamEnemies);
        asteroid.gameObject.name = mdata.name;
        asteroid.destructionType = PolygonGameObject.DestructionType.eComplete;
        return asteroid;
    }

    public static Gasteroid CreateGasteroid(MGasteroidData initData)
    {
        float size = Random (initData.size);
        int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size*3);
        Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);

        Gasteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Gasteroid>(vertices, Singleton<GlobalConfig>.inst.GasteroidColor);
        asteroid.InitAsteroid (initData.physical, initData.speed, initData.rotation);
        asteroid.destructionType = PolygonGameObject.DestructionType.eComplete;
        DeathAnimation.MakeDeathForThatFellaYo (asteroid, true, 1.3f);
        asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
        asteroid.gameObject.name = initData.name;

        return asteroid;
    }



	public static SawEnemy CreateSawEnemy(MSawData data, int layer)
	{
		var initData = data.mdata;
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SawEnemy asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SawEnemy>(vertices, initData.color);
		asteroid.InitSawEnemy (initData);
		asteroid.SetCollisionLayerNum (layer);
        asteroid.gameObject.name = data.name;

		return asteroid;
	}

	public static SpikyAsteroid CreateSpikyAsteroid(MSpikyData data, int layer)
	{
		var initData = data.mdata;
		float rInner = Random(initData.size);
		float spikeLength = Random(initData.spikeSize);
		float rOuter = rInner + spikeLength;
		int spikesCount = Random (initData.spikesCount); //UnityEngine.Random.Range((int)(rInner+1), 9);
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, initData.color);
		asteroid.InitSpikyAsteroid (spikes, initData);
		asteroid.SetCollisionLayerNum (layer);
        asteroid.gameObject.name = data.name;

		return asteroid;
	}

    public static TowerEnemy CreateStationTower(MStationTowerData data, int layer)
	{
        float r = data.size.RandomValue;  
        int sides = data.sidesCount.RandomValue;
		
		int[] cannons;
		Vector2[] vertices = PolygonCreator.CreateTowerPolygonVertices (r, r/7f, sides, out cannons);
		
        var tower = PolygonCreator.CreatePolygonGOByMassCenter<TowerEnemy> (vertices, data.color);//Singleton<GlobalConfig>.inst.towerEnemiesColor);
        tower.Init (data);
		tower.SetCollisionLayerNum (layer);
        tower.gameObject.name = data.name;
		DeathAnimation.MakeDeathForThatFellaYo (tower);
		List<Place> gunplaces = new List<Place> ();
		for (int i = 0; i < cannons.Length; i++) 
		{
			Place place = new Place();
			place.pos = vertices[cannons[i]];
			place.dir = place.pos.normalized;
			gunplaces.Add(place);
		}

        InitGuns (tower, gunplaces, data.gun);
		
		return tower;
	}

	public static SimpleTower CreateSimpleTower(MTowerData data, int layer)
	{
		System.Func<Vector3> anglesRestriction = () =>
		{
			Vector2 dir = new Vector2(1,0);
			Vector3 result = dir;
			result.z = 180;
			return result;
		}; 
		var turret = PolygonCreator.CreatePolygonGOByMassCenter<SimpleTower>(data.verts, data.color);
		turret.InitSimpleTower (data.physical, data.rotationSpeed, data.accuracy, data.shootAngle);
		turret.reward = data.reward;
		turret.SetCollisionLayerNum (layer);
		var guns = new List<Gun> ();
		foreach (var gunplace in data.guns) 
		{
			var gun = GunsData.GetGun(gunplace, turret);
			guns.Add (gun);
		}
		turret.SetGuns (guns, data.linkedGuns);
		turret.targetSystem = new TurretTargetSystem(turret, data.rotationSpeed, anglesRestriction, data.repeatTargetCheck);
		DeathAnimation.MakeDeathForThatFellaYo (turret);

		if (data.shield != null && data.shield.capacity > 0)
			turret.SetShield (data.shield);

		foreach (var item in data.turrets) 
		{
			CreateTurret(turret, item);
		}

		return turret;
	}

	private static List<Material> _asteroidsMaterials;
	public static Material GetAsteroidMaterial(int n)
	{
		if(_asteroidsMaterials == null)
		{
			_asteroidsMaterials = new List<Material>();
			for (int i = 0; i < MAsteroidsResources.Instance.asteroidsData.Count; i++) 
			{
				var mat = GameObject.Instantiate (PolygonCreator.texturedMaterial) as Material;
				mat.color = MAsteroidsResources.Instance.asteroidsCommonData[i].color;
				_asteroidsMaterials.Add(mat);
			}
		}
		return _asteroidsMaterials[n];
	}

	public static polygonGO.Drop CreateDrop(Color color, int value)
	{
		float size = 0.7f;
		int vcount = 5;
		Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(size, vcount);

        var drop = PolygonCreator.CreatePolygonGOByMassCenter<polygonGO.Drop>(vertices, color);
		drop.InitPolygonGameObject (new PhysicalData());
		drop.SetCollisionLayerNum (CollisionLayers.ilayerMisc);
		drop.value = value;
		drop.gameObject.name = "drop";
		drop.lifetime = 10f;
		
		return drop;
	}

    public static PowerUp CreatePowerUpDrop(PowerupData data) {
        float size = 1f;
        int vcount = 7;
        Vector2[] vertices = PolygonCreator.CreatePerfectPolygonVertices(size, vcount);
        var drop = PolygonCreator.CreatePolygonGOByMassCenter<PowerUp>(vertices, data.color);
        drop.InitPolygonGameObject(new PhysicalData());
        drop.InitPowerUp(data.effect);
        var ps = GameObject.Instantiate<ParticleSystem> (data.particles, drop.transform);
        ps.startColor = data.particleSystemColor;
        ps.transform.localPosition = new Vector3(0, 0, 1);
        drop.SetCollisionLayerNum(CollisionLayers.ilayerMisc);
        drop.gameObject.name = "DropPowerUp " + data.effect.ToString();
        drop.lifetime = data.lifeTime;
        return drop;
    }

	private static void InitGuns(PolygonGameObject enemy, List<Place> gunplaces, MGunBaseData gunData)
	{
		foreach (var gunplace in gunplaces) 
		{
			var gun = gunData.GetGun (gunplace, enemy);
			enemy.guns.Add (gun);
		}
	}

    private static float Random(RandomFloat r)
    {
        return UnityEngine.Random.Range(r.min, r.max);
    }

    private static int Random(RandomInt r)
    {
        return UnityEngine.Random.Range(r.min, r.max);
    }

    #region deprecated
    //this guys are not used currently
    public static EvadeEnemy CreateEvadeEnemy(List<PolygonGameObject> bullets)
    {
        EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
        enemy.InitEvadeEnemy (new PhysicalData(), bullets);
        enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
        enemy.gameObject.name = "evade enemy";
        DeathAnimation.MakeDeathForThatFellaYo (enemy);
        InitGuns (enemy, EvadeEnemy.gunplaces, GunsData.SimpleGun2());

        return enemy;
    }

    //this guys are not used currently
    public static EvadeEnemy CreateTankEnemy(List<PolygonGameObject> bullets)
    {
        EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(TankEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
        enemy.InitEvadeEnemy (new PhysicalData(), bullets);
        enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
        List<Place> gunplaces = new List<Place>
        {
            new Place(new Vector2(1.5f, 0.75f), new Vector2(1.0f, 0f)),
            new Place(new Vector2(1.5f, -0.75f), new Vector2(1.0f, 0f)),
        };

        InitGuns (enemy, gunplaces, GunsData.TankGun());
        DeathAnimation.MakeDeathForThatFellaYo (enemy);
        enemy.gameObject.name = "tank enemy";

        return enemy;
    }

    //this guys are not used currently
    public static RogueEnemy CreateRogueEnemy()
    { 
        RogueEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<RogueEnemy>(RogueEnemy.vertices, Singleton<GlobalConfig>.inst.spaceshipEnemiesColor);
        enemy.Init ();
        enemy.gameObject.name = "RogueEnemy";
        enemy.SetCollisionLayerNum (CollisionLayers.ilayerTeamEnemies);
        DeathAnimation.MakeDeathForThatFellaYo (enemy);

        return enemy;
    }

    //deprecated
    //  public static Asteroid CreateAsteroid(AsteroidSetupData dataPh, AsteroidData data, Material mat)
    //  {
    //      float size = Random(dataPh.size);
    //      int vcount = UnityEngine.Random.Range(5 + (int)size, 5 + (int)size*3);
    //      Vector2[] vertices = PolygonCreator.CreateAsteroidVertices(size, size/2f, vcount);
    //      Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Singleton<GlobalConfig>.inst.AsteroidColor, mat);
    //
    //      //todo: include physical into asteroids?
    //      var ph = new PhysicalData ();
    //      ph.density = data.density;
    //      ph.healthModifier = data.density;
    //
    //      asteroid.InitAsteroid (ph, dataPh.speed, dataPh.rotation); 
    //      asteroid.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
    //      asteroid.gameObject.name = "asteroid";
    //      
    //      return asteroid;
    //  }
    #endregion
}
