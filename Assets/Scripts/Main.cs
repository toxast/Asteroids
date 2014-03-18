using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	SpaceShip spaceship;
	List <PolygonGameObject> enemies = new List<PolygonGameObject>();
	List <Bullet> bullets = new List<Bullet>();
	List <Bullet> enemyBullets = new List<Bullet>();
	List <EvadeEnemy> evades = new List<EvadeEnemy>(); 

	PowerUpsCreator powerUpsCreator;
	List<PowerUp> powerUps = new List<PowerUp> ();

	private float DestroyTreshold = 2.5f;

	public Bullet bulletPrefab;

	Rect bounds;


	//powerup
	private float slowTimeLeft = 0;

	void Start()
	{
		CalculateBounds();

		CreateSpaceShip();


		int evades = 1;//UnityEngine.Random.Range(1, 4);
		for (int i = 0; i < evades; i++) 
		{
			EvadeEnemy enemy = CreateEvadeEnemy();
			SetRandomPosition(enemy);
			enemies.Add(enemy);
		}

		int spikies = 1;//UnityEngine.Random.Range(1, 4);
		for (int i = 0; i < spikies; i++) 
		{
			CreateSpikyAsteroid();
		}

		int asteroidsNum = 2;//UnityEngine.Random.Range(2, 5);
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
		float angle = (Random.Range(0f,359f) * Mathf.PI) / 180f;
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
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(SpaceshipsData.fastSpaceshipVertices, Color.blue);
		spaceship.FireEvent += OnFire;
		spaceship.gameObject.name = "Spaceship";
	}
	
	void HandlePowerUpCreated (PowerUp powerUp)
	{
		SetRandomPosition(powerUp);
		powerUps.Add(powerUp);
	}

	void OnFire ()
	{
		Bullet bullet = CreateBullet(spaceship.cacheTransform.position + spaceship.cacheTransform.right, spaceship.cacheTransform.right, Color.red);

		for (int i = 0; i < bullets.Count; i++) 
		{
			if(bullets[i] == null)
			{
				bullets[i] = bullet;
				return;
			}
		}

		bullets.Add(bullet);
	}

	//TODO: refactor
	void OnEnemyFire (EvadeEnemy enemy)
	{
		Bullet bullet = CreateBullet(enemy.cacheTransform.position, enemy.cacheTransform.right, Color.magenta);
		
		for (int i = 0; i < enemyBullets.Count; i++) 
		{
			if(enemyBullets[i] == null)
			{
				enemyBullets[i] = bullet;
				return;
			}
		}
		
		enemyBullets.Add(bullet);
	}

	private Bullet CreateBullet(Vector2 position, Vector2 direction, Color color)
	{
		float size = 0.3f;
		Vector2[] bulletVertices = new Vector2[]
		{
			new Vector2(size,size),
			new Vector2(size,-size),
			new Vector2(-size,-size),
			new Vector2(-size,size),
		};
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(bulletVertices, color);
		bullet.Init(direction);
		bullet.cacheTransform.position = position;
		bullet.gameObject.name = "bullet";

		return bullet;
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

		spaceship.Tick(Time.deltaTime);
		CheckBounds(spaceship.cacheTransform, spaceship.polygon.R);

		for (int i = 0; i < enemies.Count ; i++)
		{
			enemies[i].Tick(enemyDtime);
			CheckBounds(enemies[i].cacheTransform, enemies[i].polygon.R);
		}

		for (int i = 0; i < bullets.Count; i++)
		{
			if(bullets[i] == null)
				continue;

			bullets[i].Tick(Time.deltaTime);
			CheckBounds(bullets[i].cacheTransform, bullets[i].polygon.R);
		}

		//TODO: refactor
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			if(enemyBullets[i] == null)
				continue;
			
			enemyBullets[i].Tick(enemyDtime);
			CheckBounds(enemyBullets[i].cacheTransform, enemyBullets[i].polygon.R);
		}

		//TODO: refactor
		PolygonGameObject asteroid;
		Bullet bullet;
		for (int i = 0; i < enemies.Count ; i++)
		{
			asteroid = enemies[i];
			for (int k = 0; k < bullets.Count; k++)
			{
				bullet = bullets[k];
				if(bullet == null)
					continue;

				if(PolygonCollision.IsCollides(asteroid, bullet))
				{
					Destroy(bullet.gameObject);
					bullets[k] = null; 

					asteroid.Hit(bullet.damage);
					if(asteroid.IsKilled())
					{
						if(asteroid.polygon.R > DestroyTreshold || asteroid is EvadeEnemy)//TODO: refactor
						{
							List<PolygonGameObject> parts = SplitIntoAsteroids(asteroid);
							enemies.AddRange(parts);
						}
						 
						Destroy(asteroid.gameObject);
						enemies.RemoveAt(i);
					}

					if(asteroid.IsKilled())
					{
						break;
					}
				}
			}
		}

		for (int i = powerUps.Count - 1; i >= 0; i--) 
		{
			PowerUp powerUp = powerUps[i];
			if(PolygonCollision.IsCollides(spaceship, powerUp))
			{
				EffectType effect = powerUp.effect;

				powerUps.RemoveAt(i);
				Destroy(powerUp.gameObject);

				if(effect == EffectType.SlowAsteroids)
				{
					slowTimeLeft = 10f;
				}
				else if(effect == EffectType.IncreasedShootingSpeed)
				{
					spaceship.ChangeFiringSpeed(3f, 10f);
				}
			}
		}

		if(powerUpsCreator != null)
			powerUpsCreator.Tick(Time.deltaTime);

	}


	private void CheckBounds(Transform pTransform, float R)
	{
		Vector3 pos = pTransform.position;
		if(pos.x - R > bounds.xMax)
		{
			pTransform.position = pTransform.position.SetX(bounds.xMin - R);
		}
		else if(pos.x + R < bounds.xMin)
		{
			pTransform.position = pTransform.position.SetX(bounds.xMax + R);
		}

		if(pos.y + R < bounds.yMin)
		{
			pTransform.position = pTransform.position.SetY(bounds.yMax + R);
		}
		else if(pos.y - R > bounds.yMax)
		{
			pTransform.position = pTransform.position.SetY(bounds.yMin - R);
		}
	}

	private List<PolygonGameObject> SplitIntoAsteroids(PolygonGameObject polygonGo)
	{
		List<Vector2[]> polys = polygonGo.Split();
		List<PolygonGameObject> parts = new List<PolygonGameObject>();

		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
			parts.Add(polygonGo);
			return parts;
		}

		foreach(var vertices in polys)
		{
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Color.black);
			asteroidPart.Init();
			asteroidPart.cacheTransform.Translate(polygonGo.cacheTransform.position);
			asteroidPart.cacheTransform.RotateAround(polygonGo.cacheTransform.position, -Vector3.back, polygonGo.cacheTransform.rotation.eulerAngles.z);
			asteroidPart.gameObject.name = "asteroid part";

			CalculateObjectPartVelocity(asteroidPart, polygonGo);

			parts.Add(asteroidPart);
		}
		return parts;
	}

	private void CalculateObjectPartVelocity(Asteroid asteroidPart, PolygonGameObject mainPart)
	{
		Vector2 direction = asteroidPart.cacheTransform.position - mainPart.cacheTransform.position;
		asteroidPart.velocity = direction*2;
		IGotVelocity velocityObj = mainPart as IGotVelocity;
		if(velocityObj != null)
		{
			asteroidPart.velocity += (Vector3)velocityObj.Velocity;
		}
		asteroidPart.velocity.z = 0; //TODO: z system
	}

	private EvadeEnemy CreateEvadeEnemy()
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Color.black);
		enemy.SetBulletsList(bullets);
		enemy.SetTarget(spaceship);
		enemy.FireEvent += OnEnemyFire;
		enemy.gameObject.name = "evade enemy";
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

	
}
