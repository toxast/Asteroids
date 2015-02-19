using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	[SerializeField] Texture2D cursorTexture;

	[SerializeField] ParticleSystem thrustPrefab;
	[SerializeField] ParticleSystem thrustPrefab2;
	[SerializeField] ParticleSystem thrustBig;
	[SerializeField] ParticleSystem explosion;
	[SerializeField] ParticleSystem gasteroidExplosion;
	[SerializeField] List<ParticleSystem> deathExplosions;
	[SerializeField] List<ParticleSystem> finishExplosions;
	[SerializeField] StarsGenerator starsGenerator;
	[SerializeField] TabletInputController tabletController;
	UserSpaceShip spaceship;
	List <IPolygonGameObject> gobjects = new List<IPolygonGameObject>();
	//List <PolygonGameObject> enemies = new List<PolygonGameObject>();
	//List <PolygonGameObject> asteroids = new List<PolygonGameObject>();
	List <polygonGO.Drop> drops = new List<polygonGO.Drop>();
	List <IBullet> bullets = new List<IBullet>();
	//List <BulletBase> enemyBullets = new List<BulletBase>();
	List <TimeDestuctor> destructors = new List<TimeDestuctor>();
	List<ObjectsDestructor> goDestructors = new List<ObjectsDestructor> ();
	Dictionary<DropID, DropData> id2drops = new Dictionary<DropID, DropData> (); 

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
		if (cursorTexture != null)
		{
			Cursor.SetCursor (cursorTexture, new Vector2(cursorTexture.width/2f, cursorTexture.height/2f), CursorMode.Auto);
		}

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

	//TODO: partial objects reposition?
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

					Reposition(gobjects, pos, false);
					//Reposition(enemies, pos, false);
					Reposition(powerUps, pos, false);
					Reposition(bullets, pos, true);
					//Reposition(enemyBullets, pos, true);

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
				foreach (var e in gobjects) 
				{
					e.SetTarget(spaceship);
				}
//				foreach (var e in enemies) 
//				{
//					e.SetTarget(spaceship);
//				}
//				foreach (var e in asteroids) 
//				{
//					e.SetTarget(spaceship);
//				}
			}
		}
	}

	public void HandleSpikeAttack(Asteroid spikePart)
	{
		Add2Objects (spikePart);
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
		//TickBullets (enemyBullets, enemyDtime);

		TickObjects (gobjects, enemyDtime);
//		TickObjects (asteroids, enemyDtime);
//		TickObjects (enemies, enemyDtime);
		TickObjects (powerUps, enemyDtime);
		TickObjects (drops, enemyDtime);

		if(boundsMode)
		{
			CheckBounds(bullets, true);
			CheckBounds (gobjects, false);
			//CheckBounds(enemyBullets, true);
			//CheckBounds(enemies, false);
			//CheckBounds(asteroids, false);
			CheckBounds(powerUps, false);
			CheckBounds(drops, false);
		}
		else
		{
			Wrap(bullets, true);
			Wrap(gobjects, true);
//			Wrap(enemyBullets, true);
//			Wrap(enemies, false);
//			Wrap(asteroids, false);
			Wrap(powerUps, false);
			Wrap(drops, false);
		}

		TickAlphaDestructors (enemyDtime);

		Tick_GO_Destructors (Time.deltaTime);

		CleanBulletsList (bullets);
		BulletsHitObjects (gobjects, bullets);
//		BulletsHitObjects (enemies, bullets);
//		BulletsHitObjects (asteroids, bullets);
//		BulletsHitObjects (asteroids, enemyBullets);

		if(spaceship != null)
		{
			if(spaceship.collector != null)
			{
				spaceship.collector.Pull(spaceship.position, drops, Time.deltaTime);
			}

			BulletsHitObject(spaceship, bullets);

			//BulletsHitObject(spaceship, enemyBullets);

			for (int i = gobjects.Count - 1; i >= 0; i--) 
			{
				ObjectsCollide(spaceship, gobjects[i]);
			}

//			for (int i = enemies.Count - 1; i >= 0; i--) 
//			{
//				ObjectsCollide(spaceship, enemies[i]);
//			}
//
//			for (int i = asteroids.Count - 1; i >= 0; i--) 
//			{
//				ObjectsCollide(spaceship, asteroids[i]);
//			}

			for (int i = drops.Count - 1; i >= 0; i--) 
			{
				int indxa, indxb;
				if(PolygonCollision.IsCollides(spaceship, drops[i], out indxa, out indxb))
				{
					Destroy(drops[i].gameObject);
					drops.RemoveAt(i);
				}
			}
		}

		for (int i = gobjects.Count - 1; i >= 0; i--) 
		{
			for (int k = gobjects.Count - 1; k >= 0; k--) 
			{
				if(i != k)
					ObjectsCollide(gobjects[i], gobjects[k]);
			}
		}

		//experimental
//		for (int i = enemies.Count - 1; i >= 0; i--) 
//		{
//			for (int k = asteroids.Count - 1; k >= 0; k--) 
//			{
//				ObjectsCollide(enemies[i], asteroids[k]);
//			}
//		}

		//TODO: refactor
		for (int i = powerUps.Count - 1; i >= 0; i--) 
		{
			PowerUp powerUp = powerUps[i];
			if(powerUp.lived > 20f)
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

		if(powerUpsCreator != null)
			powerUpsCreator.Tick(Time.deltaTime);

		MoveCamera ();

		if(spaceship != null && spaceship.IsKilled())
		{
			ObjectDeath(spaceship);
			spaceship = null;
		}

		CheckDeadObjects (gobjects);
		//CheckDeadObjects (enemies);
		//CheckDeadObjects (asteroids);
		CheckDeadObjects (drops);
		CheckDeadObjects (bullets, true);
		//CheckDeadObjects (enemyBullets);
	}

	private void CheckDeadObjects<T> (List<T> objs, bool nullCheck = false)
		where T: IPolygonGameObject
	{
		for (int k = objs.Count - 1; k >= 0; k--)
		{
			var obj = objs[k];
			if(obj != null && obj.IsKilled())
			{
				if(obj.deathAnimation != null)
				{
					if(!obj.deathAnimation.started)
					{
						obj.deathAnimation.AnimateDeath(obj);
					}
				}

				if(obj.deathAnimation == null || obj.deathAnimation.finished)
				{
					ObjectDeath(objs[k]);
					if(nullCheck)
					{
						objs[k] = default(T);
					}
					else
					{
						objs.RemoveAt(k);
					}
				}
			}
		}
	}

	private void ObjectDeath(IPolygonGameObject gobject)
	{
		//TODO: refactor
		if(gobject is Gasteroid)
		{
			SplitAsteroidAndMarkForDestructionAllParts(gobject);
			
			//TODO: refactor, do not make explosions on object death?
//			List<PolygonGameObject> affected = new List<PolygonGameObject>();
//			affected.Add(spaceship);
//			affected.AddRange(gobjects);
////			affected.AddRange(asteroids);
////			affected.AddRange(enemies);
//			new PhExplosion(gobject.cacheTransform.position, 8*gobject.mass, affected);
			var e = Instantiate(gasteroidExplosion) as ParticleSystem;
			e.transform.position = gobject.cacheTransform.position - new Vector3(0,0,1);
			e.transform.localScale = new Vector3(gobject.polygon.R, gobject.polygon.R, 1);
			ObjectsDestructor d = new ObjectsDestructor(e.gameObject, e.duration);
			PutOnFirstNullPlace(goDestructors, d); 
		}
		else if(gobject is IBullet)
		{
			var b = (gobject as IBullet);
			if(b.breakOnDeath)
			{
				gobject.velocity /= 2f;
				SplitAsteroidAndMarkForDestructionAllParts(gobject);
			}
		}
		else
		{
			SplitIntoAsteroidsAndMarkForDestuctionSmallParts(gobject);
		}
		
		//TODO: refactor
		{
			EnemySpaceShip esp = gobject as EnemySpaceShip;
			if(esp != null)
			{
				foreach(var t in esp.turrets)
				{
					t.cacheTransform.parent = null;
					t.velocity += esp.velocity;
					t.rotation += UnityEngine.Random.Range(-150f, 150f);
					ObjectDeath(t);
				}
			}
		}

		if(gobject.deathAnimation != null && gobject.deathAnimation.instantiatedExplosions != null)
		{
			foreach (var e in gobject.deathAnimation.instantiatedExplosions) 
			{
				ObjectsDestructor d = new ObjectsDestructor(e.gameObject, e.duration);
				PutOnFirstNullPlace(goDestructors, d); 
			}
		}

		Destroy(gobject.gameObj);
		gobject = null;
	}

	private void ObjectsCollide(IPolygonGameObject a, IPolygonGameObject b)
	{
		if((a.collision & b.layer) == 0)
			return;

		int indxa, indxb;
		if(PolygonCollision.IsCollides(a, b, out indxa, out indxb))
		{
			var impulse = PolygonCollision.ApplyCollision(a, b, indxa, indxb);
			var dmg = GetCollisionDamage(impulse);

			a.Hit(dmg);
			b.Hit(dmg);
		}
	}

	//TODO: ckecks
	private void BulletsHitObjects(List<IPolygonGameObject> objs, List<IBullet> pbullets)
	{
		for (int i = objs.Count - 1; i >= 0; i--) 
		{
			BulletsHitObject(objs[i], pbullets);
		}
	}

	private void CleanBulletsList(List<IBullet> pbullets)
	{
		int lastK_NUlls = 0;
		for (int k = pbullets.Count - 1; k >= 0; k--)
		{
			if(pbullets[k] != null)
			{
				lastK_NUlls = k;
				break;
			}
		}

		if(lastK_NUlls >= 0 && lastK_NUlls < pbullets.Count-1)
		{
			Debug.LogWarning("clear: "  + (pbullets.Count - (lastK_NUlls + 1)));
			pbullets.RemoveRange (lastK_NUlls + 1, pbullets.Count - (lastK_NUlls + 1));
		}
	}

	private void BulletsHitObject(IPolygonGameObject obj,  List<IBullet> pbullets)
	{

		for (int k = pbullets.Count - 1; k >= 0; k--)
		{
			var bullet = pbullets[k];
			if(bullet == null)
				continue;

			if((obj.collision & bullet.layer) == 0)
				continue;

			int indxa, indxb;
			if(PolygonCollision.IsCollides(obj, bullet, out indxa, out indxb))
			{
				PolygonCollision.ApplyCollision(obj, bullet, indxa, indxb);

				if(!bullet.IsKilled())
					obj.Hit(bullet.damage);

				bullet.breakOnDeath = true;
				bullet.Kill();
			}
		}
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

	private void SplitIntoAsteroidsAndMarkForDestuctionSmallParts(IPolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, DestroyAfterSplitTreshold);
	}

	private void SplitAsteroidAndMarkForDestructionAllParts(IPolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, Mathf.Infinity);
	}

	private void SplitAndDestroyThresholdParts(IPolygonGameObject obj, float threshold)
	{
		DropData drop = null;
		if(obj.dropID != null)
		{
			id2drops.TryGetValue(obj.dropID, out drop);
		}

		List<Asteroid> parts = Spliter.SplitIntoAsteroids(obj);
		foreach(var part in parts)
		{
			if(part.polygon.area < threshold)
			{
				//refactor
				if(drop != null)
				{
					CheckDrop(drop, part);
				}

				TimeDestuctor d = new TimeDestuctor(part, 0.7f + UnityEngine.Random.Range(0f, 1f));
				PutOnFirstNullPlace(destructors, d); 
			}
			else
			{
				if(obj.dropID != null)
				{
					part.dropID = obj.dropID;
				}
				Add2Objects(part);
			}
		}
	}

	private void CheckDrop(DropData drop, IPolygonGameObject destroyingPart)
	{
		int lastDrops = (int)((drop.areaLeft/drop.startingArea) * drop.dropsCount);
		drop.areaLeft -= destroyingPart.polygon.area;
		int newDrops = (int)((drop.areaLeft/drop.startingArea) * drop.dropsCount);
		int diff = lastDrops - newDrops;
		if(diff > 0)
		{
			for (int i = 0; i < diff; i++) 
			{
				if(!Math2d.Chance(drop.dropChance))
					continue;

				Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
				var dropObj = ObjectsCreator.CreateDrop();
				dropObj.cacheTransform.position =
					destroyingPart.cacheTransform.position + randomOffset + new Vector3(0,0,2);
				dropObj.rotation = UnityEngine.Random.Range(160f, 240f) * Mathf.Sign(UnityEngine.Random.Range(-1f,1f));
				drops.Add(dropObj);
			}
		}
	}

	private void Add2Objects(PolygonGameObject p)
	{
		if(p.layer == 0)
		{
			if(p is Asteroid)
			{
				p.SetCollisionLayerNum(GlobalConfig.ilayerAsteroids);
				//asteroids.Add(p);
			}
			else
			{
				p.SetCollisionLayerNum(GlobalConfig.ilayerTeamEnemies);
				//enemies.Add (p);
			}
		}
		gobjects.Add (p);
	}

	private float GetCollisionDamage(float impulse)
	{
		var dmg = Mathf.Abs (impulse) * Singleton<GlobalConfig>.inst.DamageFromCollisionsModifier;
		//Debug.LogWarning (dmg);
		return dmg;
	}


	private void TickObjects<T>(List<T> list, float dtime)
		where T: IPolygonGameObject
	{
		for (int i = 0; i < list.Count ; i++)
		{
			list[i].Tick(dtime);
		}
	}

	private void CheckBounds<T>(List<T> list, bool nullCheck)
		where T: IPolygonGameObject
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
		where T: IPolygonGameObject
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
		where T: IPolygonGameObject
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
		where T: IBullet
	{
		for (int i = 0; i < list.Count ; i++)
		{
			var b = list[i];
			if(b == null)
				continue;

			b.Tick(dtime);
			if(b.Expired())
			{
				b.Kill();
			}
		}
	}

	private void CheckBounds(IPolygonGameObject p)
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
		InputController controller = null; 
		#if UNITY_STANDALONE
		controller = new StandaloneInputController();
		#else
		tabletController.Init();
		controller = tabletController;
		#endif
		spaceship = ObjectsCreator.CreateSpaceShip (controller);
		spaceship.SetShield(new ShieldData(10f,2f,2f));
		spaceship.guns.ForEach( g => g.onFire += HandleGunFire);
		//var gT = Instantiate (thrustPrefab2) as ParticleSystem;
		spaceship.SetThruster (thrustPrefab2, Vector2.zero);
		spaceship.cacheTransform.position = Camera.main.transform.position.SetZ (0);
	}

	void HandleEnemyGunFire (IBullet bullet)
	{
		bullet.SetCollisionLayerNum (GlobalConfig.ilayerBulletsEnemies);
		PutOnFirstNullPlace<IBullet>(bullets, bullet);
	}

	void HandleGunFire (IBullet bullet)
	{
		bullet.SetCollisionLayerNum (GlobalConfig.ilayerBulletsUser);
		PutOnFirstNullPlace<IBullet>(bullets, bullet);
	}
	
	public void CreateEnemySpaceShip()
	{
//		var enemy = ObjectsCreator.CreateEnemySpaceShip ();
//		//var gT = Instantiate (thrustPrefab) as ParticleSystem;
//		enemy.SetController (new EnemySpaceShipController (enemy, bullets, enemy.guns[0].bulletSpeed));
//		enemy.SetThruster (thrustPrefab, Vector2.zero);
//		InitNewEnemy(enemy);

		var enemy = ObjectsCreator.CreateSpaceShip<EnemySpaceShip>(3);
		enemy.SetController (new EnemySpaceShipController (enemy, bullets, enemy.guns[0].bulletSpeed));

		InitNewEnemy(enemy);
	}
	
	public EnemySpaceShip CreateEnemySpaceShipBoss()
	{
		var enemy = ObjectsCreator.CreateBossEnemySpaceShip ();
		//var gT = Instantiate (thrustBig) as ParticleSystem;
		enemy.SetController (new SimpleAI1(enemy));
		var tpos = enemy.polygon.vertices [6];
		var tpos2 = tpos;
		tpos2.y = -tpos2.y;
		enemy.SetThruster (thrustBig, (tpos + tpos2)*0.5f );
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
			tenemy.guns.ForEach( g => g.onFire += HandleEnemyGunFire);
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
			tenemy.guns.ForEach( g => g.onFire += HandleEnemyGunFire);
			enemy.AddTurret (towerpos2, dir2, tenemy);
		}
		
		InitNewEnemy(enemy);
		return enemy;
	}
	
	public void CreateRogueEnemy()
	{
		var enemy = ObjectsCreator.CreateRogueEnemy();
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
		InitNewEnemy(enemy);
	}
	
	public void CreateTower()
	{
		var enemy = ObjectsCreator.CreateTower();
		InitNewEnemy(enemy);
	}
	
	public EvadeEnemy CreateEvadeEnemy()
	{
		EvadeEnemy enemy = ObjectsCreator.CreateEvadeEnemy (bullets);
		InitNewEnemy(enemy);
		return enemy;
	}
	
	public EvadeEnemy CreateTankEnemy()
	{
		var enemy = ObjectsCreator.CreateTankEnemy(bullets);
		InitNewEnemy(enemy);
		return enemy;
	}
	
	public Asteroid CreateGasteroid()
	{
		var asteroid = ObjectsCreator.CreateGasteroid ();
		InitNewEnemy(asteroid);
		return asteroid;
	}

	public void CreateFight()
	{
//		var tower = ObjectsCreator.CreateTower();
//		{
//			tower.SetCollisionLayer(GlobalConfig.ilayerTeamUser);
//			tower.guns.ForEach( g => g.onFire += HandleGunFire);
//			SetRandomPosition(tower);
//			Add2Objects(tower);
//		}
//
//		var evade = ObjectsCreator.CreateEvadeEnemy (bullets);
//		{
//			evade.guns.ForEach( g => g.onFire += HandleGunFire);
//			evade.SetCollisionLayer (GlobalConfig.ilayerTeamUser);
//			SetRandomPosition(evade);
//			Add2Objects(evade);
//		}

		var boss = CreateEnemySpaceShipBoss ();

//		var evade = ObjectsCreator.CreateBossEnemySpaceShip (bullets);
//		{
//			evade.guns.ForEach( g => g.onFire += HandleGunFire);
//			evade.SetCollisionLayer (GlobalConfig.ilayerTeamUser);
//			SetRandomPosition(evade);
//			Add2Objects(evade);
//		}

//		var enemy1 = ObjectsCreator.CreateEnemySpaceShip ();
//		{
//			var gT = Instantiate (thrustPrefab) as ParticleSystem;
//			enemy1.SetController (new EnemySpaceShipController (enemy1, bullets, enemy1.guns[0].bulletSpeed));
//			enemy1.SetThruster (gT, Vector2.zero);
//			enemy1.guns.ForEach( g => g.onFire += HandleGunFire);
//			enemy1.SetCollisionLayer (GlobalConfig.ilayerTeamUser);
//			SetRandomPosition(enemy1);
//			Add2Objects(enemy1);
//		}

		for (int i = 0; i < 2; i++) {
			var enemy = ObjectsCreator.CreateEnemySpaceShip ();
			{
				//var gT = Instantiate (thrustPrefab) as ParticleSystem;
				enemy.SetController (new EnemySpaceShipController (enemy, bullets, enemy.guns[0].bulletSpeed));
				enemy.SetThruster (thrustPrefab, Vector2.zero);
				enemy.guns.ForEach( g => g.onFire += HandleGunFire);
				enemy.SetCollisionLayerNum (GlobalConfig.ilayerTeamUser);
				SetRandomPosition(enemy);
				Add2Objects(enemy);
			}
			
			enemy.SetTarget (boss);
			boss.SetTarget (enemy);
				}

	}
	
	public Asteroid CreateAsteroid()
	{
		var asteroid = ObjectsCreator.CreateAsteroid ();

		CreateDropForObject (asteroid);

		InitNewEnemy (asteroid);
		return asteroid;
	}

	private void CreateDropForObject(IPolygonGameObject obj)
	{
		obj.dropID = new DropID ();
		DropData dropData = new DropData
		{
			startingArea = obj.polygon.area,
			areaLeft = obj.polygon.area,
			dropsCount = obj.polygon.area/10f,
			dropChance = 0.8f,
		};
		id2drops [obj.dropID] = dropData;
	}
	
	
	private void InitNewEnemy(PolygonGameObject enemy)
	{
		enemy.guns.ForEach( g => g.onFire += HandleEnemyGunFire);
		enemy.SetTarget (spaceship);
		SetRandomPosition(enemy);
		Add2Objects(enemy);
	}
	
	private void SetRandomPosition(IPolygonGameObject p)
	{
		float angle = UnityEngine.Random.Range(0f, 359f) * Math2d.PIdiv180;
		float len = UnityEngine.Random.Range(p.polygon.R + 2 * p.polygon.R, screenBounds.yMax);
		p.position = new Vector2(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len);
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

	public static bool IsNull(IPolygonGameObject target)
	{
		return target == null || target.Equals (null);
	}

	public IPolygonGameObject GetNewTarget(IPolygonGameObject g)
	{
		var pos = g.position;
		float minDist = float.MaxValue;
		int indx = -1;
		for (int i = 0; i < gobjects.Count; i++)
		{
			var obj = gobjects[i];
			if(((g.collision & obj.layer) != 0))
			{
				var distSqr = (obj.position - pos).sqrMagnitude;
				if(distSqr < minDist)
				{
					minDist = distSqr;
					indx = i;
				}
			}
		}

		if(indx >= 0)
		{
			return gobjects[indx];
		}
		else
		{
			if(!IsNull(spaceship) && (g.collision & spaceship.layer) != 0)
			{
				return spaceship;
			}
			Debug.LogWarning("no target");
			return null;
		}
	}

	/*
	 * FUTURE UPDATES
	 * missile is IBullet && IploygonGO => missile can be Spaceship!
	 * collision mask
	 * death refactor && missiles explosion on death
	 * explision by vertex
	 * gravity misiles
	 * change rotation (thruster attach) missiles?
	 * mine missiles
	 * make enemies fight with each other!
	 * multiple thrusters
	 * textured asteroids
	 * bullets shoot missiles?
	 * shoot place levels
	 * Lazers!
	 * shield hit animation
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
