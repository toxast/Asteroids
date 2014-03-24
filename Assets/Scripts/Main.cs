using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	SpaceShip spaceship;
	List <PolygonGameObject> enemies = new List<PolygonGameObject>();
	List <Bullet> bullets = new List<Bullet>();
	List <Bullet> enemyBullets = new List<Bullet>();
	//List <EvadeEnemy> evades = new List<EvadeEnemy>();


	PowerUpsCreator powerUpsCreator;
	List<PowerUp> powerUps = new List<PowerUp> ();

	private float DestroyTreshold = 8f;

	Rect bounds;

	//powerup
	private float slowTimeLeft = 0;
	private float penetrationTimeLeft = 0;

	void Start()
	{
		CalculateBounds();

		CreateSpaceShip();


		int evades = 5;//UnityEngine.Random.Range(0, 2);
		for (int i = 0; i < evades; i++) 
		{
			EvadeEnemy enemy = CreateEvadeEnemy();
			SetRandomPosition(enemy);
			enemies.Add(enemy);
		}

		int tanks = 1;//UnityEngine.Random.Range(0, 2);
		for (int i = 0; i < tanks; i++) 
		{
			TankEnemy enemy = CreateTankEnemy();
			SetRandomPosition(enemy);
			enemies.Add(enemy);
		}

		int spikies = 0;//UnityEngine.Random.Range(0, 3);
		for (int i = 0; i < spikies; i++) 
		{
			CreateSpikyAsteroid();
		}

		int asteroidsNum = 0;//UnityEngine.Random.Range(2, 9);
		for (int i = 0; i < asteroidsNum; i++) 
		{
			Asteroid asteroid = CreateAsteroid();
			SetRandomPosition(asteroid);
			enemies.Add(asteroid);
		}

		powerUpsCreator = new PowerUpsCreator(5f, 10f);
		powerUpsCreator.PowerUpCreated += HandlePowerUpCreated;

	}

	private void SetRandomPosition(PolygonGameObject p)
	{
		float angle = Random.Range(0f,359f) * Math2d.PIdiv180;
		float len = UnityEngine.Random.Range(p.polygon.R + 2 * p.polygon.R, bounds.yMax);
		p.cacheTransform.position = new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, p.cacheTransform.position.z);
	}

	private void CreateSpikyAsteroid()
	{
		float rInner = UnityEngine.Random.Range(2f, 4f);
		float spikeLength = UnityEngine.Random.Range (2f, 3f);
		float rOuter = rInner + spikeLength;
		int spikesCount = UnityEngine.Random.Range((int)(rInner+1), 9);

		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);

		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, Color.black);
		asteroid.gameObject.name = "spiked asteroid";
		
		asteroid.Init(spaceship.cacheTransform, spikes);
		asteroid.SpikeAttack += HandleSpikeAttack;
		
		SetRandomPosition(asteroid);
		enemies.Add(asteroid);
	}

	public void HandleSpikeAttack(Asteroid spikePart)
	{
		enemies.Add (spikePart);
	}

	private void CalculateBounds()
	{
		float height = 2*Camera.main.camera.orthographicSize;  
		float width = height * Screen.width / Screen.height;
		bounds = new Rect(-width/2f, -height/2f, width, height);
	}

	private void CreateSpaceShip()
	{
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(SpaceshipsData.tankSpaceshipVertices, Color.blue);
		spaceship.FireEvent += OnFire;
		spaceship.gameObject.name = "Spaceship";
		List<ShootPlace> shooters = new List<ShootPlace>();

		ShootPlace place2 =  ShootPlace.GetSpaceshipShootPlace();
		place2.color = Color.white;
		place2.fireInterval = 0.4f;
		place2.position = new Vector2(1f, 1.3f);

		ShootPlace place3 =  ShootPlace.GetSpaceshipShootPlace();
		place3.fireInterval = 0.4f;
		place3.color = Color.white;
		place3.position = new Vector2(1f, -1.3f);

		shooters.Add(place2);
		shooters.Add(place3);
		spaceship.SetShootPlaces(shooters );
	}
	
	void HandlePowerUpCreated (PowerUp powerUp)
	{
		SetRandomPosition(powerUp);
		powerUps.Add(powerUp);
	}

	private void PutOnFirstNullPlace<T>(List<T> list, T obj)
	{
		for (int i = 0; i < list.Count; i++) 
		{
			if(list[i] == null)
			{
				list[i] = obj;
				return;
			}
		}
		
		list.Add(obj);
	}

	void OnFire (ShootPlace shooter, Transform trfrm)
	{
		Bullet bullet = BulletCreator.CreateBullet(trfrm, shooter); 

		PutOnFirstNullPlace<Bullet>(bullets, bullet);
	}

	void OnEnemyFire (ShootPlace shooter, Transform trfrm)
	{
		Bullet bullet = BulletCreator.CreateBullet(trfrm, shooter); 
		
		PutOnFirstNullPlace<Bullet>(enemyBullets, bullet);
	}

	float enemyDtime;
	void Update()
	{
		//TODO: refactor Powerup
		if (slowTimeLeft > 0) 
		{
			slowTimeLeft -=  Time.deltaTime;
			enemyDtime = Time.deltaTime/2f;
		}
		else
		{
			enemyDtime = Time.deltaTime;
		}

		if(penetrationTimeLeft > 0)
		{
			penetrationTimeLeft -= Time.deltaTime;
		}

		spaceship.Tick(Time.deltaTime);
		CheckBounds(spaceship);

		TickAndCheckBoundsNullCheck (bullets, Time.deltaTime);

		TickAndCheckBounds (enemies, enemyDtime);

		TickAndCheckBoundsNullCheck (enemyBullets, enemyDtime);


		//TODO: refactor
		PolygonGameObject enemy;
		Bullet bullet;
		for (int i = enemies.Count - 1; i >= 0; i--) 
		{
			enemy = enemies[i];
			for (int k = bullets.Count - 1; k >= 0; k--)
			{
				bullet = bullets[k];
				if(bullet == null)
					continue;

				if(PolygonCollision.IsCollides(enemy, bullet))
				{
					if(penetrationTimeLeft <= 0f)
					{
						Destroy(bullet.gameObject);
						bullets[k] = null; 

						enemy.Hit(bullet.damage);
					}
					else
					{
						enemy.Hit(bullet.damage*10*Time.deltaTime);
					}
					
					if(enemy.IsKilled())
					{
						if(enemy.polygon.area > DestroyTreshold || enemy is EvadeEnemy)//TODO: refactor
						{
							List<Asteroid> parts = SplitIntoAsteroids(enemy);
							foreach(var part in parts)
							{
								enemies.Add(part);
							}
						}
						 
						enemies.RemoveAt(i);
						Destroy(enemy.gameObject);
						break;
					}
				}
			}
		}

		//TODO: refactor
		for (int i = powerUps.Count - 1; i >= 0; i--) 
		{
			PowerUp powerUp = powerUps[i];
			if(PolygonCollision.IsCollides(spaceship, powerUp))
			{
				EffectType effect = powerUp.effect;

				powerUps.RemoveAt(i);
				Destroy(powerUp.gameObject);

				switch (effect) 
				{
				case EffectType.IncreasedShootingSpeed:
					spaceship.ChangeFiringSpeed(3f, 10f);
				break;

				case EffectType.PenetrationBullet:
					penetrationTimeLeft = 10f;
				break;

				case EffectType.SlowAsteroids:
					slowTimeLeft = 10f;
				break;
				}	
			}
		}

		if(powerUpsCreator != null)
			powerUpsCreator.Tick(Time.deltaTime);

	}

	private void TickAndCheckBounds<T>(List<T> list, float dtime)
		where T: PolygonGameObject
	{
		for (int i = 0; i < list.Count ; i++)
		{
			list[i].Tick(dtime);
			CheckBounds(list[i]);
		}
	}

	private void TickAndCheckBoundsNullCheck<T>(List<T> list, float dtime)
		where T: PolygonGameObject
	{
		for (int i = 0; i < list.Count ; i++)
		{
			if(list[i] == null)
				continue;
			
			list[i].Tick(dtime);
			CheckBounds(list[i]);
		}
	}

	private void CheckBounds(PolygonGameObject p)
	{
		Vector3 pos = p.cacheTransform.position;
		float R = p.polygon.R;

		if(pos.x - R > bounds.xMax)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetX(bounds.xMin - R);
		}
		else if(pos.x + R < bounds.xMin)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetX(bounds.xMax + R);
		}

		if(pos.y + R < bounds.yMin)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetY(bounds.yMax + R);
		}
		else if(pos.y - R > bounds.yMax)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetY(bounds.yMin - R);
		}
	}

	private List<Asteroid> SplitIntoAsteroids(PolygonGameObject polygonGo)
	{
		List<Vector2[]> polys = polygonGo.Split();
		List<Asteroid> parts = new List<Asteroid>();

		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
		}

		foreach(var vertices in polys)
		{
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Color.black);

			if(PolygonCreator.CheckIfVerySmallOrSpiky(asteroidPart.polygon))
			{
				Destroy(asteroidPart.gameObject);
			}
			else
			{
				asteroidPart.Init();
				asteroidPart.cacheTransform.Translate(polygonGo.cacheTransform.position);
				asteroidPart.cacheTransform.RotateAround(polygonGo.cacheTransform.position, -Vector3.back, polygonGo.cacheTransform.rotation.eulerAngles.z);
				asteroidPart.gameObject.name = "asteroid part";

				parts.Add(asteroidPart);
			}
		}

		CalculateObjectPartVelocity(parts, polygonGo);

		return parts;
	}

	//TODO:refactor
	private void CalculateObjectPartVelocity(List<Asteroid> parts, PolygonGameObject mainPart)
	{
		IGotVelocity velocityObj = mainPart as IGotVelocity;
		Vector2 mainVelocity = Vector2.zero;
		if(velocityObj != null)
		{
			mainVelocity = velocityObj.Velocity;
		}

		float mainRotation = 0; 
		IGotRotation rotationObj = mainPart as IGotRotation;
		if(rotationObj != null)
		{
			mainRotation = rotationObj.Rotation * Math2d.PIdiv180;
		}

		float mainPartEnergy = 0.5f * mainPart.mass * mainVelocity.sqrMagnitude;
		float mainPartRotationEnergy = 0.5f * mainPart.inertiaMoment * (mainRotation*mainRotation);

		float kInertiaToBlow = 0.1f;
		float kVelocityEnergyToRotation = 0.1f;
		float kRotationEnergyToVelocity = 0.3f;

		mainPartRotationEnergy += mainPartEnergy * kVelocityEnergyToRotation;
		mainPartEnergy = mainPartEnergy * (1 - kVelocityEnergyToRotation);

		float blowEnergy = Mathf.Sqrt(mainPart.mass) +  mainPartEnergy * kInertiaToBlow;
		float inertiaEnergy = mainPartEnergy*(1 - kInertiaToBlow);

		List<Vector2> distances = new List<Vector2> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			distances.Add(parts[i].cacheTransform.position - mainPart.cacheTransform.position);
		}

		float sqrtSummMass = 0;
		List<float> sqrtMasses = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			sqrtMasses.Add(Mathf.Sqrt(parts[i].mass));
			sqrtSummMass += sqrtMasses[i];
		}

		float sumRotationWeights = 0;
		List<float> rotationWeights = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			rotationWeights.Add(Mathf.Sqrt(distances[i].magnitude/parts[i].polygon.R));
			sumRotationWeights += rotationWeights[i];
		}

		float rotationSign = Mathf.Sign (mainRotation);
		for (int i = 0; i < parts.Count; i++) 
		{
			Asteroid part = parts[i];
			float pieceBlowEnergy = blowEnergy * (sqrtMasses[i] / sqrtSummMass); 
			float pieceInertiaEnergy = inertiaEnergy * (sqrtMasses[i] / sqrtSummMass); 

			Vector2 direction = distances[i];
			part.velocity = direction.normalized * Mathf.Sqrt(2f * pieceBlowEnergy / part.mass );
			if(mainVelocity != Vector2.zero)
			{
				part.velocity += (Vector3)mainVelocity.normalized * Mathf.Sqrt( 2f * pieceInertiaEnergy / part.mass );
			}

			float pieceRotationEnergy = mainPartRotationEnergy * (rotationWeights[i]/ sumRotationWeights);
			float velocityEnegryFromRotation = kRotationEnergyToVelocity * pieceRotationEnergy;
			pieceRotationEnergy = pieceRotationEnergy * (1 - kRotationEnergyToVelocity);
			
			part.rotation = rotationSign * Mathf.Sqrt( 2f * pieceRotationEnergy / part.inertiaMoment ) / Math2d.PIdiv180;

			Vector2 perpendecular = new Vector2(direction.y, -direction.x);
			part.velocity += (Vector3)perpendecular.normalized * rotationSign * Mathf.Sqrt( 2f * velocityEnegryFromRotation / part.mass );

			part.velocity.z = 0;//TODO: z system*/
		}
	}

	private EvadeEnemy CreateEvadeEnemy()
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Color.black);
		enemy.SetBulletsList(bullets);
		enemy.SetTarget(spaceship);
		ShootPlace place = ShootPlace.GetSpaceshipShootPlace();
		place.fireInterval *= 3;
		Math2d.ScaleVertices(place.vertices, 1f);
		enemy.SetShooter(place);
		enemy.FireEvent += OnEnemyFire;
		enemy.gameObject.name = "evade enemy";
		return enemy;
	}

	private TankEnemy CreateTankEnemy()
	{
		TankEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<TankEnemy>(TankEnemy.vertices, Color.black);
		enemy.SetBulletsList(bullets);
		enemy.SetTarget(spaceship);

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
		enemy.SetShooter(places);
		enemy.FireEvent += OnEnemyFire;
		enemy.gameObject.name = "tank enemy";
		return enemy;
	}

	private Asteroid CreateAsteroid()
	{
		float size = Random.Range(2f, 8f);
		int vcount = Random.Range(3, 3 + (int)size*2);
		Vector2[] vertices = PolygonCreator.CreatePolygonVertices(size, size/2f, vcount);
		
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Color.black);
		asteroid.Init ();
		asteroid.gameObject.name = "asteroid";

		return asteroid;
	}

	/*
	 * FUTURE UPDATES
	 * bullets and shooters refactoring 
	 * Z pos refactoring
	 * enemy bulets hit asteroids?
	 * penetration bullet power up
	 * tank enemy
	 * shields
	 * rogue enemy, invisibility
	 * magnet enemy
	 * achievements and ship unlocks for them (luke - survive astroid field, reach 100% acc in more than x shots)
	 * dissolve bullet and shader
	 * rockets
	 * bad effects on power-up destroy. Monster spawn, tough emeny, rocket launch, spawn of new very PoweR up! 
	 * bosses (arcanoid?)
	 * drops from enemies
	 * 
	 * 
	 * 
	 */
}
