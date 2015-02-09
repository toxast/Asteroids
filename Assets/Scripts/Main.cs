using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	[SerializeField] ParticleSystem thrustPrefab;
	[SerializeField] ParticleSystem thrustPrefab2;
	[SerializeField] ParticleSystem thrustBig;
	[SerializeField] ParticleSystem explosion;
	[SerializeField] ParticleSystem gasteroidExplosion;
	[SerializeField] StarsGenerator starsGenerator;
	[SerializeField] TabletInputController tabletController;
	UserSpaceShip spaceship;
	List <PolygonGameObject> enemies = new List<PolygonGameObject>();
	List <BulletBase> bullets = new List<BulletBase>();
	List <BulletBase> enemyBullets = new List<BulletBase>();
	List <TimeDestuctor> destructors = new List<TimeDestuctor>();
	List<ObjectsDestructor> goDestructors = new List<ObjectsDestructor> ();

	PowerUpsCreator powerUpsCreator;
	List<PowerUp> powerUps = new List<PowerUp> ();

	private float DestroyAfterSplitTreshold = 5f;


	[SerializeField] float borderWidth = 40f;
	Rect screenBounds;
	Rect flyZoneBounds;

	[SerializeField] private float starsDensity = 5f;

	[SerializeField] Vector2 sceneSizeInCameras = new Vector2 (3, 3);

	//powerup
	private float slowTimeLeft = 0;
	private float penetrationTimeLeft = 0;

	private event Action moveCameraAction;
	[SerializeField] bool boundsMode = true; 
	//TODO: reposition ship if faraway
	//TODO: stars Clusters for faster check

	void Awake()
	{
		if(boundsMode)
		{
			moveCameraAction += MoveCameraBoundsMode;
		}
		else
		{
			moveCameraAction += MoveCameraWarpMode;
			StartCoroutine(RepositionAll());
			StartCoroutine(WrapStars());
		}
	}


	IEnumerator WrapStars()
	{
		yield return new WaitForSeconds(1f);
		while(true)
		{
			var stars = starsGenerator.stars;
			for (int i = 0; i < stars.Length ; i++)
			{
				Wrap(stars[i]);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	IEnumerator RepositionAll()
	{
		yield return new WaitForSeconds(1f);
		while(true)
		{
			if(spaceship != null)
			{
				Vector2 pos = spaceship.position;
				if(pos.magnitude > screenBounds.width*3)
				{
					spaceship.position -= pos;

					Reposition(enemies, pos, false);
					Reposition(powerUps, pos, false);
					Reposition(bullets, pos, true);
					Reposition(enemyBullets, pos, true);

					var stars = starsGenerator.stars;
					for (int i = 0; i < stars.Length ; i++)
					{
						stars[i].position -= (Vector3)pos;
					}

					MoveCamera();
				}
			}
			yield return new WaitForSeconds(5f);
		}
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
			CreateAsteroid();
		}

		powerUpsCreator = new PowerUpsCreator(5f, 10f);
		powerUpsCreator.PowerUpCreated += HandlePowerUpCreated;
	}

	public IEnumerator Respawn()
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

	public void HandleSpikeAttack(Asteroid spikePart)
	{
		Add2Enemies (spikePart);
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

		if(boundsMode)
		{
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
	}

	private void MoveCameraWarpMode()
	{
		Vector3 pos = spaceship.cacheTransform.position;
		Camera.main.transform.position = pos.SetZ(Camera.main.transform.position.z);
	}

	private void MoveCameraBoundsMode()
	{
		Vector3 pos = spaceship.cacheTransform.position;
		float x = Mathf.Clamp(pos.x, -maxCameraX, maxCameraX);
		float y = Mathf.Clamp(pos.y, -maxCameraY, maxCameraY);
		Camera.main.transform.position = new Vector3(x, y, Camera.main.transform.position.z);
	}


	private void MoveCamera()
	{
		if(spaceship != null)
		{
			moveCameraAction();
		}
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
		BulletBase bullet = BulletCreator.CreateBullet(trfrm, shooter); 

		PutOnFirstNullPlace<BulletBase>(bullets, bullet);
	}

	void OnEnemyFire (ShootPlace shooter, Transform trfrm)
	{
		BulletBase bullet = BulletCreator.CreateBullet(trfrm, shooter); 

		///Missile missile = BulletCreator.CreateMissile(spaceship.gameObject, trfrm, shooter); 
		
		PutOnFirstNullPlace<BulletBase>(enemyBullets, bullet);
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
			if(boundsMode)
			{
				ApplyBoundsForce(spaceship);
			}
			spaceship.Tick(Time.deltaTime);
		}

		TickBullets (bullets, Time.deltaTime);
		TickBullets (enemyBullets, enemyDtime);

		TickObjects (enemies, enemyDtime);
		TickObjects (powerUps, enemyDtime);

		if(boundsMode)
		{
			CheckBounds(bullets, true);
			CheckBounds(enemyBullets, true);
			CheckBounds(enemies, false);
			CheckBounds(powerUps, false);
		}
		else
		{
			Wrap(bullets, true);
			Wrap(enemyBullets, true);
			Wrap(enemies, false);
			Wrap(powerUps, false);
		}

		TickAlphaDestructors (enemyDtime);

		Tick_GO_Destructors (Time.deltaTime);

		//TODO: refactor
		for (int i = enemies.Count - 1; i >= 0; i--) 
		{
			var enemy = enemies[i];

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

						//PhExplosion e = new PhExplosion(bullet.cacheTransform.position, 200, enemies);

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
						EnemyDeath(enemy, i);
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

					spaceship.Hit(bullet.damage);

					Destroy(bullet.gameObject);
					enemyBullets[i] = null; 
				}
			}


			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				var enemy = enemies[i];

				int indxa, indxb;
				if(PolygonCollision.IsCollides(spaceship, enemy, out indxa, out indxb))
				{
					var impulse = PolygonCollision.ApplyCollision(spaceship, enemy, indxa, indxb);
					var dmg = GetCollisionDamage(impulse);

					spaceship.Hit(dmg);
					enemy.Hit(dmg);

					if(enemy.IsKilled())
					{
						EnemyDeath(enemy, i);
						break;
					}
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

	private void EnemyDeath(PolygonGameObject enemy, int i)
	{
		//TODO: refactor
		if(enemy is Gasteroid)
		{
			//TODO: refactor
			SplitAsteroidAndMarkForDestructionAllParts(enemy);
			List<PolygonGameObject> affected = new List<PolygonGameObject>();
			affected.Add(spaceship);
			new PhExplosion(enemy.cacheTransform.position, 6*enemy.mass, affected);
			var e = Instantiate(gasteroidExplosion) as ParticleSystem;
			e.transform.position = enemy.cacheTransform.position - new Vector3(0,0,1);
			e.transform.localScale = new Vector3(enemy.polygon.R, enemy.polygon.R, 1);
			ObjectsDestructor d = new ObjectsDestructor(e.gameObject, e.duration);
			PutOnFirstNullPlace(goDestructors, d); 
		}
		else
		{
			SplitIntoAsteroidsAndMarkForDestuctionSmallParts(enemy);
		}
		
		//TODO: refactor
		{
			EnemySpaceShip esp = enemy as EnemySpaceShip;
			if(esp != null)
			{
				foreach(var t in esp.turrets)
				{
					t.cacheTransform.parent = null;
					t.velocity += esp.velocity;
					t.rotation += UnityEngine.Random.Range(-150f, 150f);
//					bool destroy = false;
//					if(destroy)
//					{
						SplitIntoAsteroidsAndMarkForDestuctionSmallParts(t);
						Destroy(t.gameObject);
//					}
//					else
//					{
//						(t as IGotTarget).SetTarget(null);
//						Add2Enemies(t);
//					}
				}

				var e = Instantiate(explosion) as ParticleSystem;
				e.transform.position = enemy.cacheTransform.position - new Vector3(0,0,1);
				ObjectsDestructor d = new ObjectsDestructor(e.gameObject, e.duration);
				PutOnFirstNullPlace(goDestructors, d); 
			}
		}
		
		enemies.RemoveAt(i);
		Destroy(enemy.gameObject);
	}

	private void ApplyBoundsForce(PolygonGameObject p)
	{
		Vector2 curPos = p.cacheTransform.position;
		//CheckIfShipOutOfBounds()
		if(curPos.x < flyZoneBounds.xMin)
		{
			//TODO: delta
			float dir = (flyZoneBounds.xMin - curPos.x);
			ApplyForce(p, new Vector2(dir,0));
		}
		else if(curPos.x > flyZoneBounds.xMax)
		{
			float dir = (flyZoneBounds.xMax - curPos.x);
			ApplyForce(p, new Vector2(dir,0));
		}
		
		if(curPos.y < flyZoneBounds.yMin)
		{
			float dir = (flyZoneBounds.yMin - curPos.y);
			ApplyForce(p, new Vector2(0,dir));
		}
		else if(curPos.y > flyZoneBounds.yMax)
		{
			float dir = (flyZoneBounds.yMax - curPos.y);
			ApplyForce(p, new Vector2(0,dir));
		}
	}

	private void ApplyForce(PolygonGameObject p, Vector2 dir)
	{
		float pushingForce = 4f;
		Vector3 f = dir.normalized * pushingForce * dir.sqrMagnitude;
		p.velocity += (Time.deltaTime * f) / p.mass ;
	}

	private void SplitIntoAsteroidsAndMarkForDestuctionSmallParts(PolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, DestroyAfterSplitTreshold);
	}

	private void SplitAsteroidAndMarkForDestructionAllParts(PolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, Mathf.Infinity);
	}

	private void SplitAndDestroyThresholdParts(PolygonGameObject obj, float threshold)
	{
		List<Asteroid> parts = Spliter.SplitIntoAsteroids(obj);
		foreach(var part in parts)
		{
			if(part.polygon.area < threshold)
			{
				TimeDestuctor d = new TimeDestuctor(part, 0.7f + UnityEngine.Random.Range(0f, 1f));
				PutOnFirstNullPlace(destructors, d); 
			}
			else
			{
				Add2Enemies(part);
			}
		}
	}

	private void Add2Enemies(PolygonGameObject p)
	{
		enemies.Add (p);
	}

	private float GetCollisionDamage(float impulse)
	{
		var dmg = Mathf.Abs (impulse) * Singleton<GlobalConfig>.inst.DamageFromCollisionsModifier;
		//Debug.LogWarning (dmg);
		return dmg;
	}


	private void TickObjects<T>(List<T> list, float dtime)
		where T: PolygonGameObject
	{
		for (int i = 0; i < list.Count ; i++)
		{
			list[i].Tick(dtime);
		}
	}

	private void CheckBounds<T>(List<T> list, bool nullCheck)
		where T: PolygonGameObject
	{
		if(!nullCheck)
		{
			for (int i = 0; i < list.Count ; i++)
			{
				CheckBounds(list[i]);
			}
		}
		else
		{
			for (int i = 0; i < list.Count ; i++)
			{
				if(list[i] != null)
					CheckBounds(list[i]);
			}
		}
	}


	private void Reposition<T>(List<T> list, Vector2 delta, bool nullCheck)
		where T: PolygonGameObject
	{
		if(!nullCheck)
		{
			for (int i = 0; i < list.Count ; i++)
			{
				list[i].position -= delta;
			}
		}
		else
		{
			for (int i = 0; i < list.Count ; i++)
			{
				if(list[i] != null)
					list[i].position -= delta;
			}
		}
	}

	private void Wrap<T>(List<T> list, bool nullCheck)
		where T: PolygonGameObject
	{
		if(!nullCheck)
		{
			for (int i = 0; i < list.Count ; i++)
			{
				Wrap(list[i].cacheTransform);
			}
		}
		else
		{
			for (int i = 0; i < list.Count ; i++)
			{
				if(list[i] != null)
					Wrap(list[i].cacheTransform);
			}
		}
	}

	private void TickBullets<T>(List<T> list, float dtime)
		where T: BulletBase
	{
		for (int i = 0; i < list.Count ; i++)
		{
			var b = list[i];
			if(b == null)
				continue;

			b.Tick(dtime);
			if(b.Expired())
			{
				Destroy(b.gameObject);
				list[i] = null;
			}
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

	private void Wrap(Transform t)
	{
		Vector2 center = Camera.main.transform.position;
		Vector2 pos = t.position;
		float rHorisontal = screenBounds.width/2;
		float rVertical = screenBounds.height/2;
		
		if(pos.x > center.x + rHorisontal)
		{
			t.position = t.position.SetX(pos.x - 2*rHorisontal);
		}
		else if(pos.x < center.x - rHorisontal)
		{
			t.position = t.position.SetX(pos.x + 2*rHorisontal);
		}
		
		if(pos.y > center.y + rVertical)
		{
			t.position = t.position.SetY(pos.y - 2*rVertical);
		}
		else if(pos.y < center.y - rVertical)
		{
			t.position = t.position.SetY(pos.y + 2*rVertical);
		}
	}


	public void CreateSpaceShip()
	{
		spaceship = ObjectsCreator.CreateSpaceShip ();
		spaceship.SetShield(new ShieldData(10f,2f,2f));
		spaceship.FireEvent += OnFire;
		var gT = Instantiate (thrustPrefab2) as ParticleSystem;
		spaceship.SetThruster (gT, Vector2.zero);
		spaceship.cacheTransform.position = Camera.main.transform.position.SetZ (0);
	}
	
	public void CreateEnemySpaceShip()
	{
		var enemy = ObjectsCreator.CreateEnemySpaceShip ();
		var gT = Instantiate (thrustPrefab) as ParticleSystem;
		enemy.SetController (new EnemySpaceShipController (enemy, bullets, enemy.shooters[0].speed));
		enemy.SetThruster (gT, Vector2.zero);
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
	}
	
	public void CreateEnemySpaceShipBoss()
	{
		var enemy = ObjectsCreator.CreateBossEnemySpaceShip ();
		var gT = Instantiate (thrustBig) as ParticleSystem;
		enemy.SetController (new SimpleAI1(enemy));
		var tpos = enemy.polygon.vertices [6];
		var tpos2 = tpos;
		tpos2.y = -tpos2.y;
		enemy.SetThruster (gT, (tpos + tpos2)*0.5f );
		enemy.FireEvent += OnEnemyFire;
		bool smartAim = true;
		
		//		var towerpos1 = (enemy.polygon.vertices [2] + enemy.polygon.vertices [4])/2f;
		var towerpos1 = enemy.polygon.vertices [6] * 0.6f;
		var dir1 = enemy.polygon.vertices [6];
		{
			Func<Vector3> anglesRestriction1 = () =>
			{
				float angle = enemy.cacheTransform.rotation.eulerAngles.z*Math2d.PIdiv180;
				Vector2 dir = Math2d.RotateVertex(dir1, angle);
				Vector3 result = dir;
				result.z = 90f;
				return result;
			}; 
			var tenemy = ObjectsCreator.CreateSimpleTower(smartAim);
			tenemy.SetAngleRestrictions(anglesRestriction1);
			tenemy.FireEvent += OnEnemyFire;
			enemy.AddTurret (towerpos1, dir1, tenemy);
		}
		
		{
			var towerpos2 = towerpos1;
			towerpos2.y = -towerpos2.y;
			var dir2 = dir1;
			dir2.y = -dir2.y;
			Func<Vector3> anglesRestriction2 = () =>
			{
				float angle = enemy.cacheTransform.rotation.eulerAngles.z*Math2d.PIdiv180;
				Vector2 dir = Math2d.RotateVertex(dir2, angle);
				Vector3 result = dir;
				result.z = 90f;
				return result;
			}; 
			var tenemy = ObjectsCreator.CreateSimpleTower(smartAim);
			tenemy.SetAngleRestrictions(anglesRestriction2);
			tenemy.FireEvent += OnEnemyFire;
			enemy.AddTurret (towerpos2, dir2, tenemy);
		}
		
		InitNewEnemy(enemy);
	}
	
	public void CreateRogueEnemy()
	{
		var enemy = ObjectsCreator.CreateRogueEnemy();
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
	}
	
	public void CreateSawEnemy()
	{
		var enemy = ObjectsCreator.CreateSawEnemy();
		InitNewEnemy(enemy);
	}
	
	public void CreateSpikyAsteroid()
	{
		var enemy = ObjectsCreator.CreateSpikyAsteroid();
		enemy.SpikeAttack += HandleSpikeAttack;
		InitNewEnemy(enemy);
	}
	
	public void CreateSimpleTower()
	{
		bool smartAim = false;
		var enemy = ObjectsCreator.CreateSimpleTower(smartAim);
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
	}
	
	public void CreateTower()
	{
		var enemy = ObjectsCreator.CreateTower();
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
	}
	
	public EvadeEnemy CreateEvadeEnemy()
	{
		EvadeEnemy enemy = ObjectsCreator.CreateEvadeEnemy (bullets);
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
		return enemy;
	}
	
	public TankEnemy CreateTankEnemy()
	{
		TankEnemy enemy = ObjectsCreator.CreateTankEnemy(bullets);
		enemy.FireEvent += OnEnemyFire;
		InitNewEnemy(enemy);
		return enemy;
	}
	
	public Asteroid CreateGasteroid()
	{
		var asteroid = ObjectsCreator.CreateGasteroid ();
		InitNewEnemy(asteroid);
		return asteroid;
	}
	
	public Asteroid CreateAsteroid()
	{
		var asteroid = ObjectsCreator.CreateAsteroid ();
		InitNewEnemy (asteroid);
		return asteroid;
	}
	
	
	private void InitNewEnemy(PolygonGameObject enemy)
	{
		var t = enemy as IGotTarget;
		if(t != null)
			t.SetTarget (spaceship);
		
		SetRandomPosition(enemy);
		Add2Enemies(enemy);
	}
	
	private void SetRandomPosition(PolygonGameObject p)
	{
		float angle = UnityEngine.Random.Range(0f, 359f) * Math2d.PIdiv180;
		float len = UnityEngine.Random.Range(p.polygon.R + 2 * p.polygon.R, screenBounds.yMax);
		p.cacheTransform.position = new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, p.cacheTransform.position.z);
	}


	private void Tick_GO_Destructors(float dtime)
	{
		for (int i = goDestructors.Count - 1; i >= 0; i--) 
		{
			var destructor = goDestructors[i];
			if(destructor != null)
			{
				destructor.Tick(dtime);
				if(destructor.IsTimeExpired())
				{
					goDestructors[i] = null;
					Destroy(destructor.g);
				}
			}
		}
	}

	private void TickAlphaDestructors(float dtime)
	{
		for (int i = destructors.Count - 1; i >= 0; i--) 
		{
			var destructor = destructors[i];
			if(destructor != null)
			{
				destructor.Tick(dtime);
				if(destructor.a == null || destructor.a.gameObject == null)  
				{
					destructors[i] = null;
					continue;
				}
				else if(destructor.IsTimeExpired())
				{
					destructors[i] = null;
					Destroy(destructor.a.gameObject);
				}
			}
		}
	}

	/*
	 * FUTURE UPDATES
	 * shoot place refactors
	 * drops
	 * shoot place levels
	 * missiles!
	 * Lazers!
	 * shield animation
	 * deflect shields
	 * more efficeient stars render
	 * fire enimies
	 * cold enemies
	 * cool ships explosion
	 * gravity enemies
	 * texture on asteroids?
	 * joystick position fixed
	 * bullets and shooters refactoring 
	 * Z pos refactoring
	 * enemy bulets hit asteroids?
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
