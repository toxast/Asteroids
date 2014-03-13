using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	SpaceShip spaceship;
	List <Asteroid> asteroids = new List<Asteroid>();
	List <Bullet> bullets = new List<Bullet>();
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

		evades.Add(CreateEvadeEnemy());


		int spikies = 0;//UnityEngine.Random.Range(1, 4);
		for (int i = 0; i < spikies; i++) 
		{
			CreateSpikyAsteroid();
		}

		int asteroidsNum = 0;//UnityEngine.Random.Range(2, 5);
		for (int i = 0; i < asteroidsNum; i++) 
		{
			Asteroid asteroid = CreateAsteroid();
			float angle = (Random.Range(0f,359f) * Mathf.PI) / 180f;
			float len = UnityEngine.Random.Range(spaceship.polygon.R + 2 * asteroid.polygon.R, bounds.yMax);
			asteroid.cacheTransform.position =  new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, 0);
			asteroids.Add(asteroid);
		}

		//powerUpsCreator = new PowerUpsCreator(5f, 10f);
		//powerUpsCreator.PowerUpCreated += HandlePowerUpCreated;

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
		
		float angle = (Random.Range(0f,359f) * Mathf.PI) / 180f;
		float len = UnityEngine.Random.Range(spaceship.polygon.R + 2 * asteroid.polygon.R, bounds.yMax);
		asteroid.cacheTransform.position =  new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, 0);
		asteroids.Add(asteroid);
	}

	public void HandleSpikeAttack(Asteroid spikePart)
	{
		asteroids.Add (spikePart);
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
		float angle = (Random.Range(0f,359f) * Mathf.PI) / 180f;
		float len = UnityEngine.Random.Range(0f , bounds.height/2f);
		float z = UnityEngine.Random.Range (0.1f, 1f);
		powerUp.cacheTransform.position =  new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, z);
		powerUps.Add(powerUp);
	}

	void OnFire ()
	{
		Bullet bullet = CreateBullet();

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

	private Bullet CreateBullet()
	{
		float size = 0.3f;
		Vector2[] bulletVertices = new Vector2[]
		{
			new Vector2(size,size),
			new Vector2(size,-size),
			new Vector2(-size,-size),
			new Vector2(-size,size),
		};
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(bulletVertices, Color.red);
		bullet.Init(spaceship.cacheTransform.right);
		bullet.cacheTransform.position = spaceship.cacheTransform.position + spaceship.cacheTransform.right;
		bullet.gameObject.name = "bullet";

		return bullet;
	}


	float asteroidsDtime;
	void Update()
	{
		//TODO: refactor Powerup
		if (slowTimeLeft > 0) 
		{
			slowTimeLeft -=  Time.deltaTime;
			asteroidsDtime = Time.deltaTime/2f;
		}
		else
		{
			asteroidsDtime = Time.deltaTime;
		}

		spaceship.Tick(Time.deltaTime);
		CheckBounds(spaceship.cacheTransform, spaceship.polygon.R);

		for (int i = 0; i < asteroids.Count ; i++)
		{
			asteroids[i].Tick(asteroidsDtime);
			CheckBounds(asteroids[i].cacheTransform, asteroids[i].polygon.R);
		}

		for (int i = 0; i < bullets.Count; i++)
		{
			if(bullets[i] == null)
				continue;

			bullets[i].Tick(Time.deltaTime);
			CheckBounds(bullets[i].cacheTransform, bullets[i].polygon.R);
		}

		//TODO: refactor
		Asteroid asteroid;
		Bullet bullet;
		for (int i = 0; i < asteroids.Count ; i++)
		{
			asteroid = asteroids[i];
			for (int k = 0; k < bullets.Count; k++)
			{
				bullet = bullets[k];
				if(bullet == null)
					continue;

				if(IsCollides(asteroid, bullet))
				{
					Destroy(bullet.gameObject);
					bullets[k] = null; 

					asteroid.Hit(bullet.damage);
					if(asteroid.IsKilled())
					{
						if(asteroid.polygon.R > DestroyTreshold)
						{
							List<Asteroid> parts = SplitAsteroid(asteroid);
							asteroids.AddRange(parts);
						}
						 
						Destroy(asteroid.gameObject);
						asteroids.RemoveAt(i);
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
			if(IsCollides(spaceship, powerUp))
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

	private bool IsCollides(PolygonGameObject a, PolygonGameObject b)
	{
		Vector3 distance = a.cacheTransform.position - b.cacheTransform.position;
		Vector2 distance2d = new Vector2(distance.x, distance.y);
		return (distance2d.magnitude < a.polygon.R + b.polygon.R && Math2d.IsCollides (a, b));
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

	private List<Asteroid> SplitAsteroid(Asteroid asteriod)
	{
		List<Vector2[]> polys = asteriod.Split();
		List<Asteroid> parts = new List<Asteroid>();

		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
			parts.Add(asteriod);
			return parts;
		}

		foreach(var vertices in polys)
		{
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, Color.black);
			asteroidPart.Init();
			asteroidPart.cacheTransform.Translate(asteriod.cacheTransform.position);
			asteroidPart.cacheTransform.RotateAround(asteriod.cacheTransform.position, -Vector3.back, asteriod.cacheTransform.rotation.eulerAngles.z);
			asteroidPart.gameObject.name = "asteroid part";

			Vector3 direction = asteroidPart.cacheTransform.position - asteriod.cacheTransform.position;
			asteroidPart.velocity = direction*2 + asteriod.velocity; 
			asteroidPart.velocity.z = 0;

			parts.Add(asteroidPart);
		}
		return parts;
	}

	private EvadeEnemy CreateEvadeEnemy()
	{
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, Color.black);
		enemy.SetBulletsList(bullets);
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
