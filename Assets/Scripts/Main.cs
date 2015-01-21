using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	[SerializeField] Image joystick;
	[SerializeField] FireButton fireButton;
	[SerializeField] FireButton accelerateButton;
	[SerializeField] ParticleSystem thrustPrefab;
	[SerializeField] StarsGenerator starsGenerator;
	SpaceShip spaceship;
	List <PolygonGameObject> enemies = new List<PolygonGameObject>();
	List <Bullet> bullets = new List<Bullet>();
	List <PolygonGameObject> enemyBullets = new List<PolygonGameObject>();
	List <TimeDestuctor> parts2kill = new List<TimeDestuctor>();

	PowerUpsCreator powerUpsCreator;
	List<PowerUp> powerUps = new List<PowerUp> ();

	private float DestroyTreshold = 8f;
	private float DestroyAfterSplitTreshold = 5f;
	public static Color defaultEnemyColor = new Color (0.5f, 0.5f, 0.5f);

	[SerializeField] float borderWidth = 40f;
	Rect screenBounds;
	Rect flyZoneBounds;

	[SerializeField] private float starsDensity = 5f;

	[SerializeField] Vector2 sceneSizeInCameras = new Vector2 (3, 3);

	//powerup
	private float slowTimeLeft = 0;
	private float penetrationTimeLeft = 0;

	//TODO: new GUI
	void OnGUI()
	{
		int width = 100;
		int hieight = 40;
		int margine = 20;
		int y = 20;

		if(GUI.Button(new Rect(10, y, width+20, hieight), "asteroid"))
		{
			CreateAsteroid();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "saw"))
		{
			CreateSawEnemy();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "spiky"))
		{
			CreateSpikyAsteroid();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "tank"))
		{
			CreateTankEnemy();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "scout"))
		{
			CreateEvadeEnemy();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "rogue"))
		{
			CreateRogueEnemy();
		}
		y += hieight + margine;

		if(GUI.Button(new Rect(10, y, width, hieight), "respawn"))
		{
			StartCoroutine(Respawn());
		}
		y += hieight + margine;


		if(GUI.Button(new Rect(Screen.width-100, 10, width+20, hieight), "quit"))
		{
			Application.Quit();
		}
	}

	void Awkae()
	{
#if !UNITY_STANDALONE
		fireButton.gameObject.SetActive(true);
		accelerateButton.gameObject.SetActive(true);
#endif
	}

	void Start()
	{
		CalculateBounds(sceneSizeInCameras.x, sceneSizeInCameras.y);

		starsGenerator.Generate ((int)(starsDensity*(screenBounds.width * screenBounds.height)/2000f) , screenBounds, 30f);

		CreateSpaceShip();

		int rogues = UnityEngine.Random.Range(0, 1);
		int saws = UnityEngine.Random.Range(0, 1);
		int evades = UnityEngine.Random.Range(0, 1);
		int tanks = UnityEngine.Random.Range(0, 1);
		int spikies = UnityEngine.Random.Range(0, 1);
		int asteroidsNum = UnityEngine.Random.Range(0, 1);

		for (int i = 0; i < rogues; i++) 
		{
			CreateRogueEnemy();
		}

		for (int i = 0; i < saws; i++) 
		{
			CreateSawEnemy();
		}

		for (int i = 0; i < evades; i++) 
		{
			CreateEvadeEnemy();
		}

		for (int i = 0; i < tanks; i++) 
		{
			CreateTankEnemy();
		}

		for (int i = 0; i < spikies; i++) 
		{
			CreateSpikyAsteroid();
		}

		for (int i = 0; i < asteroidsNum; i++) 
		{
			Asteroid asteroid = CreateAsteroid();
		}

		powerUpsCreator = new PowerUpsCreator(5f, 10f);
		powerUpsCreator.PowerUpCreated += HandlePowerUpCreated;
	}


	private IEnumerator Respawn()
	{
		if (spaceship == null)
		{
			CreateSpaceShip();
			
			yield return new WaitForSeconds (1f);
			
			if (spaceship != null)
			{
				foreach (var e in enemies) 
				{
					var te = e as IGotTarget;
					if(te != null)
						te.SetTarget(spaceship);
				}
			}
		}
	}

	private void SetRandomPosition(PolygonGameObject p)
	{
		float angle = Random.Range(0f,359f) * Math2d.PIdiv180;
		float len = UnityEngine.Random.Range(p.polygon.R + 2 * p.polygon.R, screenBounds.yMax);
		p.cacheTransform.position = new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, p.cacheTransform.position.z);
	}

	private void CreateSawEnemy()
	{
		float rInner = UnityEngine.Random.Range(2f, 3f);
		float spikeLength = UnityEngine.Random.Range (0.8f, 1.5f);
		float rOuter = rInner + spikeLength;
		int spikesCount = UnityEngine.Random.Range((int)(rInner+5), (int)(rInner+10));
		
		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);
		
		SawEnemy asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SawEnemy>(vertices, defaultEnemyColor);
		asteroid.gameObject.name = "SawEnemy";
		
		asteroid.Init(spaceship);
		
		SetRandomPosition(asteroid);
		enemies.Add(asteroid);
	}

	private void CreateSpikyAsteroid()
	{
		float rInner = UnityEngine.Random.Range(2f, 4f);
		float spikeLength = UnityEngine.Random.Range (3f, 4f);
		float rOuter = rInner + spikeLength;
		int spikesCount = UnityEngine.Random.Range((int)(rInner+1), 9);

		int[] spikes;
		Vector2[] vertices = PolygonCreator.CreateSpikyPolygonVertices (rOuter, rInner, spikesCount, out spikes);

		SpikyAsteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(vertices, defaultEnemyColor);
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

	float maxCameraX;
	float maxCameraY;
	private void CalculateBounds(float screensNumHeight, float screensNumWidth)
	{
		float camHeight = 2f * Camera.main.camera.orthographicSize;
		float camWidth = camHeight * (Screen.width / (float)Screen.height);
		float height = screensNumHeight * camHeight;  
		float width = screensNumWidth * camWidth;
		screenBounds = new Rect(-width/2f, -height/2f, width, height);
		flyZoneBounds = screenBounds;
		flyZoneBounds.width -= 2f * borderWidth;
		flyZoneBounds.height -= 2f * borderWidth;
		flyZoneBounds.center = Vector2.zero;

		maxCameraX = (screenBounds.width - camWidth) / 2f;
		maxCameraY = (screenBounds.height - camHeight) / 2f;

		var borderObj = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(
			PolygonCreator.GetRectShape(20,20), Color.gray);

		var m = borderObj.mesh;
		var indx = new int[]{0,1,2,3,0};
		m.vertices = new Vector3[]
		{
			new Vector3(flyZoneBounds.xMin, flyZoneBounds.yMin),
			new Vector3(flyZoneBounds.xMax, flyZoneBounds.yMin),
			new Vector3(flyZoneBounds.xMax, flyZoneBounds.yMax),
			new Vector3(flyZoneBounds.xMin, flyZoneBounds.yMax),
		};
		m.SetIndices(indx, MeshTopology.LineStrip, 0);
		m.RecalculateBounds();

	}

	private void MoveCamera()
	{
		if(spaceship != null)
		{
			Vector3 pos = spaceship.cacheTransform.position;
			float x = Mathf.Clamp(pos.x, -maxCameraX, maxCameraX);
			float y = Mathf.Clamp(pos.y, -maxCameraY, maxCameraY);
			Camera.main.transform.position = new Vector3(x, y, Camera.main.transform.position.z);
		}
	}

	private void CreateSpaceShip()
	{
		spaceship = PolygonCreator.CreatePolygonGOByMassCenter<SpaceShip>(SpaceshipsData.fastSpaceshipVertices, Color.blue);
		spaceship.FireEvent += OnFire;
		spaceship.gameObject.name = "Spaceship";
#if UNITY_STANDALONE
		spaceship.SetController (new StandaloneInputController(flyZoneBounds));
#else
		TODO
		spaceship.SetTabletControls (fireButton, accelerateButton);
#endif

		var gT = Instantiate (thrustPrefab) as ParticleSystem;
		spaceship.SetThruster (gT);

		List<ShootPlace> shooters = new List<ShootPlace>();


		ShootPlace place2 =  ShootPlace.GetSpaceshipShootPlace();
				place2.color = Color.red;
				place2.fireInterval = 0.25f;
				place2.position = new Vector2(1f, 0f);

//		ShootPlace place2 =  ShootPlace.GetSpaceshipShootPlace();
//		place2.color = Color.white;
//		place2.fireInterval = 0.4f;
//		place2.position = new Vector2(1f, 1.3f);
//
//		ShootPlace place3 =  ShootPlace.GetSpaceshipShootPlace();
//		place3.fireInterval = 0.4f;
//		place3.color = Color.white;
//		place3.position = new Vector2(1f, -1.3f);
//
		shooters.Add(place2);
//		shooters.Add(place3);
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

		///Missile missile = BulletCreator.CreateMissile(spaceship.gameObject, trfrm, shooter); 
		
		PutOnFirstNullPlace<PolygonGameObject>(enemyBullets, bullet);
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

		if(spaceship != null)
		{
			spaceship.Tick(Time.deltaTime);

			//CheckBounds(spaceship);
		}

		TickAndCheckBoundsNullCheck (bullets, Time.deltaTime);

		TickAndCheckBounds (enemies, enemyDtime);

		TickAndCheckBoundsNullCheck (enemyBullets, enemyDtime);

		TickAndCheckBounds (powerUps, enemyDtime);

		for (int i = parts2kill.Count - 1; i >= 0; i--) 
		{
			var part = parts2kill[i];
			part.Tick(enemyDtime);
			if(part.a == null || part.a.gameObject == null)  
			{
				parts2kill.RemoveAt(i);
				continue;
			}
			else if(part.IsTimeExpired())
			{
				bool removed = enemies.Remove(part.a);
				Destroy(part.a.gameObject);
				parts2kill.RemoveAt(i);
				if(!removed)
				{
					Debug.LogError("part not removed, count:" + parts2kill.Count);
				}
			}
		}

		//TODO: refactor
		for (int i = enemies.Count - 1; i >= 0; i--) 
		{
			var enemy = enemies[i];

			if(enemy.markedForDeath)
				continue;

			for (int k = bullets.Count - 1; k >= 0; k--)
			{
				var bullet = bullets[k];
				if(bullet == null)
					continue;

				int indxa, indxb;
				if(PolygonCollision.IsCollides(enemy, bullet, out indxa, out indxb))
				{
					if(penetrationTimeLeft <= 0f)
					{
						var impulse = PolygonCollision.ApplyCollision(enemy, bullet, indxa, indxb);

						SplitIntoAsteroidsAndMarkForDestuctionSmallParts(bullet);
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
//						if(enemy.polygon.area > DestroyTreshold || !(enemy is Asteroid))//TODO: refactor
//						{
						SplitIntoAsteroidsAndMarkForDestuctionSmallParts(enemy);
						//}
						 
						enemies.RemoveAt(i);
						Destroy(enemy.gameObject);
						break;
					}
				}
			}
		}


		if(spaceship != null)
		{
			for (int i = 0; i < enemyBullets.Count; i++) 
			{
				var bullet = enemyBullets[i];
				if(bullet == null)
					continue;

				int indxa, indxb;
				if(PolygonCollision.IsCollides(spaceship, bullet, out indxa, out indxb))
				{
					var impulse = PolygonCollision.ApplyCollision(spaceship, bullet, indxa, indxb);

					SplitIntoAsteroidsAndMarkForDestuctionSmallParts(bullet);

					//TODO: bullet damage
					//spaceship.Hit(Mathf.Abs(impulse) / spaceship.mass);

					Destroy(bullet.gameObject);
					enemyBullets[i] = null; 
					Debug.Log("bam");
				}
			}


			for (int i = 0; i < enemies.Count; i++) 
			{
				var enemy = enemies[i];

				if(enemy.markedForDeath)
					continue;

				int indxa, indxb;
				if(PolygonCollision.IsCollides(spaceship, enemy, out indxa, out indxb))
				{
					var impulse = PolygonCollision.ApplyCollision(spaceship, enemy, indxa, indxb);

					//spaceship.Hit(Mathf.Abs(impulse) / spaceship.mass);
				}
			}
		}

		//TODO: refactor
		for (int i = powerUps.Count - 1; i >= 0; i--) 
		{
			PowerUp powerUp = powerUps[i];
			if(powerUp.lived > 10f)
			{
				powerUps.RemoveAt(i);
				Destroy(powerUp.gameObject);
			}
			else if(spaceship != null && PolygonCollision.IsCollides(spaceship, powerUp))
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

		if(spaceship != null && spaceship.IsKilled())
		{
			SplitIntoAsteroidsAndMarkForDestuctionSmallParts(spaceship);
			Destroy(spaceship.gameObject);
			spaceship = null;
		}

		if(powerUpsCreator != null)
			powerUpsCreator.Tick(Time.deltaTime);

		MoveCamera ();
	}

	private void SplitIntoAsteroidsAndMarkForDestuctionSmallParts(PolygonGameObject obj)
	{
		List<Asteroid> parts = SplitIntoAsteroids(obj);
		foreach(var part in parts)
		{
			enemies.Add(part);
			
			if(part.polygon.area < DestroyAfterSplitTreshold)
			{
				TimeDestuctor d = new TimeDestuctor(part, 0.7f + UnityEngine.Random.Range(0f, 1f));
				parts2kill.Add(d);
			}
		}
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

		if(pos.x - R > screenBounds.xMax)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetX(screenBounds.xMin - R);
		}
		else if(pos.x + R < screenBounds.xMin)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetX(screenBounds.xMax + R);
		}

		if(pos.y + R < screenBounds.yMin)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetY(screenBounds.yMax + R);
		}
		else if(pos.y - R > screenBounds.yMax)
		{
			p.cacheTransform.position = p.cacheTransform.position.SetY(screenBounds.yMin - R);
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
			Asteroid asteroidPart = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, polygonGo.mesh.colors[0]);

//			if(PolygonCreator.CheckIfVerySmallOrSpiky(asteroidPart.polygon))
//			{
//				Destroy(asteroidPart.gameObject);
//			}
//			else
//			{
				asteroidPart.Init();
				asteroidPart.cacheTransform.Translate(polygonGo.cacheTransform.position);
				asteroidPart.cacheTransform.RotateAround(polygonGo.cacheTransform.position, -Vector3.back, polygonGo.cacheTransform.rotation.eulerAngles.z);
				asteroidPart.gameObject.name = "asteroid part";

				parts.Add(asteroidPart);
			//}
		}

		CalculateObjectPartVelocity(parts, polygonGo);

		return parts;
	}

	//TODO:refactor
	private void CalculateObjectPartVelocity(List<Asteroid> parts, PolygonGameObject mainPart)
	{
		Vector2 mainVelocity = mainPart.velocity;
		float mainRotation = mainPart.rotation* Math2d.PIdiv180; 

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

		float sumVelocityWeights = 0;
		List<float> velocityWeights = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			velocityWeights.Add(Mathf.Sqrt(parts[i].mass));
			sumVelocityWeights += velocityWeights[i];
		}

		float sumRotationWeights = 0;
		List<float> rotationWeights = new List<float> (parts.Count);
		for (int i = 0; i < parts.Count; i++) 
		{
			rotationWeights.Add(Mathf.Sqrt(1f/parts[i].polygon.R));
			sumRotationWeights += rotationWeights[i];
		}

		float rotationSign = Mathf.Sign (mainRotation);
		for (int i = 0; i < parts.Count; i++) 
		{
			Asteroid part = parts[i];
			float pieceBlowEnergy = blowEnergy * (velocityWeights[i] / sumVelocityWeights); 
			float pieceInertiaEnergy = inertiaEnergy * (velocityWeights[i] / sumVelocityWeights); 

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
		EvadeEnemy enemy = PolygonCreator.CreatePolygonGOByMassCenter<EvadeEnemy>(EvadeEnemy.vertices, defaultEnemyColor);

		ShootPlace place = ShootPlace.GetSpaceshipShootPlace();
		place.fireInterval *= 3;
		Math2d.ScaleVertices(place.vertices, 1f);
		enemy.FireEvent += OnEnemyFire;
		enemy.gameObject.name = "evade enemy";

		enemy.Init(spaceship, bullets, place);
		SetRandomPosition(enemy);
		enemies.Add(enemy);

		return enemy;
	}

	private RogueEnemy CreateRogueEnemy()
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
		enemy.FireEvent += OnEnemyFire;

		enemy.Init(spaceship, places);

		SetRandomPosition(enemy);
		enemies.Add(enemy);
		return enemy;
	}

	private TankEnemy CreateTankEnemy()
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
		enemy.FireEvent += OnEnemyFire;

		enemy.Init(spaceship, bullets, places);

		enemy.gameObject.name = "tank enemy";

		SetRandomPosition(enemy);
		enemies.Add(enemy);

		return enemy;
	}

	private Asteroid CreateAsteroid()
	{
		float size = Random.Range(3f, 8f);
		int vcount = Random.Range(5, 5 + (int)size*3);
		Vector2[] vertices = PolygonCreator.CreatePolygonVertices(size, size/2f, vcount);
		
		Asteroid asteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(vertices, defaultEnemyColor);
		asteroid.Init ();
		asteroid.gameObject.name = "asteroid";

		SetRandomPosition(asteroid);
		enemies.Add(asteroid);

		return asteroid;
	}

	/*
	 * FUTURE UPDATES
	 * speed restrictions seconds after collision
	 * more efficeient stars render
	 * Cut inside angles on asteroid destroy
	 * joystick position fixed
	 * trust animation
	 * bullets and shooters refactoring 
	 * Z pos refactoring
	 * enemy bulets hit asteroids?
	 * shields
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
