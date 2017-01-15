using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	[SerializeField] Texture2D cursorTexture;

	[SerializeField] ParticleSystem dropAnimationPrefab;
	[SerializeField] public ParticleSystem teleportationRingPrefab;
	[SerializeField] StarsGenerator starsGenerator;
	[SerializeField] TabletInputController tabletController;
	[SerializeField] oldGUI oldGUI;
 	UserSpaceShip spaceship;
	List <PolygonGameObject> gobjects = new List<PolygonGameObject>();
    List<polygonGO.DropBase> drops = new List<polygonGO.DropBase>();
	public List<PolygonGameObject> bullets = new List<PolygonGameObject>();
	List <TimeDestuctor> destructors = new List<TimeDestuctor>();
	List<ObjectsDestructor> goDestructors = new List<ObjectsDestructor> ();
	Dictionary<DropID, DropData> id2drops = new Dictionary<DropID, DropData> (); 

	PowerUpsCreator powerUpsCreator;
	List<PowerUp> powerUps = new List<PowerUp> ();

	private float DestroyAfterSplitTreshold = 5f;
	private float addMoneyKff = 1.2f;

	[SerializeField] float borderWidth = 40f;
	Rect screenBounds;
	Rect flyZoneBounds;

	public List <PolygonGameObject> gObjects{get {return gobjects;}}
	public List <PolygonGameObject> pBullets{get {return bullets;}}

	[SerializeField] private float starsDensity = 5f;

	[SerializeField] Vector2 sceneSizeInCameras = new Vector2 (3, 3);

	//powerup
	//private float slowTimeLeft = 0;
	private float penetrationTimeLeft = 0;

	private event Action moveCameraAction;
	[SerializeField] bool boundsMode = true; 
	//TODO: stars Clusters for faster check

	[SerializeField] Camera mainCamera;
	Transform cameraTransform;
	[SerializeField] Camera minimapCamera;

	Coroutine repositionCoroutine;
	Coroutine wrapStarsCoroutine;
	Coroutine levelEndRoutine;

	void Awake()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		cameraTransform = mainCamera.transform;
		minimapCamera = GameObject.FindGameObjectWithTag ("MinimapCamera").GetComponent<Camera>();

		if (boundsMode) {
			moveCameraAction += MoveCameraBoundsMode;
		} else {
			moveCameraAction += MoveCameraWarpMode;
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

	static public Vector2 AddShipSpeed2TheBullet(PolygonGameObject ship)
	{
		return ship.velocity; //* 0.3f;
	}

	public void CreatePhysicalExplosion(Vector2 pos, float r, float dmgMax, int collision = -1)
	{
        new PhExplosion(pos, r, dmgMax, dmgMax, gobjects, collision);
	}

	ILevelSpawner spawner;
	public void StartTheGame(MSpaceshipData spaceshipData, int level = 0, int waveNum = 0)
	{
		#if UNITY_STANDALONE
		if (cursorTexture != null)
		{
			Cursor.SetCursor (cursorTexture, new Vector2(cursorTexture.width/2f, cursorTexture.height/2f), CursorMode.Auto);
		}
		#endif

		gameIsOn = true;

		CalculateBounds(sceneSizeInCameras.x, sceneSizeInCameras.y);
		
		starsGenerator.Generate ((int)(starsDensity*(screenBounds.width * screenBounds.height)/2000f) , screenBounds, 30f);
		
		CreateSpaceShip (spaceshipData);
		spaceship.destroyed += HandleUserDestroyed;

		if (level < 0)
			spawner = new EmptyTestSceneSpawner();
		else
			spawner = new LevelSpawner (LevelsResources.Instance.levels [level], waveNum);

		repositionCoroutine = StartCoroutine(RepositionAll());
		wrapStarsCoroutine = StartCoroutine(WrapStars());

//		powerUpsCreator = new PowerUpsCreator(5f, 10f);
//		powerUpsCreator.PowerUpCreated += HandlePowerUpCreated;
	}

	public event Action gameOver;
	public event Action levelCleared;
	private void HandleUserDestroyed()
	{
//		gameOver ();
	}

	public void Clear()
	{
		#if UNITY_STANDALONE
		if (cursorTexture != null)
		{
			Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		}
		#endif

		if (repositionCoroutine != null) {
			StopCoroutine (repositionCoroutine);
			repositionCoroutine = null;
		}

		if (wrapStarsCoroutine != null) {
			StopCoroutine (wrapStarsCoroutine);
			wrapStarsCoroutine = null;
		}

		if (levelEndRoutine != null) {
			StopCoroutine (levelEndRoutine);
			levelEndRoutine = null;
		}

		spawner = null;

		if (!IsNull (spaceship))
		{
			Destroy(spaceship.gameObject);
			spaceship = null;
		}

		foreach (var drop in drops) {
			drop.OnGameEnd();
			Destroy(drop.gameObject);
		}
		drops.Clear ();

		foreach (var obj in gobjects) {
			if(!IsNull(obj))
				Destroy(obj.gameObj);
		}
		gobjects.Clear ();

		foreach (var obj in bullets) {
			if(!IsNull(obj))
				Destroy(obj.gameObj);
		}
		bullets.Clear ();

		foreach (var obj in destructors) {
			if(obj!= null && !IsNull(obj.a))
				Destroy(obj.a.gameObject);
		}
		destructors.Clear ();

		foreach (var obj in goDestructors) {
			if(obj != null && obj.g != null && obj.g.transform != null)
				Destroy(obj.g.gameObject);
		}
		goDestructors.Clear ();
		id2drops.Clear ();

		gameIsOn = false;
	}

	//TODO: partial objects reposition?
	IEnumerator RepositionAll()
	{
		yield return new WaitForSeconds(1f);
		while(true)
		{
			Vector2 pos = cameraTransform.position;
			if(pos.magnitude > screenBounds.width*3)
			{
				Reposition(drops, pos, false);
				Reposition(gobjects, pos, false);
				Reposition(powerUps, pos, false);
				Reposition(bullets, pos, true);

				var stars = starsGenerator.stars;
				for (int i = 0; i < stars.Length ; i++)
				{
					stars[i].position -= (Vector3)pos;
				}

				MoveCamera();
			}
			yield return new WaitForSeconds(2f);
		}
	}


	IEnumerator CheckLevelEndRoutine()
	{
		while(true)
		{
			bool finished = true;
			foreach(var obj in gobjects)
			{
				if(obj.layerNum == CollisionLayers.ilayerTeamEnemies)
				{
					finished = false;
					break;
				}
			}

			if(finished)
			{
				levelCleared();
				yield break;
			}

			yield return new WaitForSeconds(1.5f);
		}
	}

	public IEnumerator Respawn()
	{
//		if (spaceship != null)
//		{
//			spaceship.Hit (10000000);
//			spaceship = null;
//			yield return new WaitForSeconds(0.5f);
//		}
//
//		if (spaceship == null)
//		{
//			CreateSpaceShip();
//		}
		yield break;
	}

	public void HandleSpikeAttack(Asteroid spikePart)
	{
		spikePart.destroyOnBoundsTeleport = true;
		spikePart.SetCollisionLayerNum (CollisionLayers.ilayerAsteroids);
		Add2Objects (spikePart);
	}

	float maxCameraX;
	float maxCameraY;
	private void CalculateBounds(float screensNumHeight, float screensNumWidth)
	{
		float camHeight = 2f * mainCamera.GetComponent<Camera>().orthographicSize;
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
		Vector3 pos = spaceship.position;
		mainCamera.transform.position = pos.SetZ(mainCamera.transform.position.z);
		minimapCamera.transform.position = mainCamera.transform.position.SetZ(minimapCamera.transform.position.z);
	}

	private void MoveCameraBoundsMode()
	{
		Vector3 pos = spaceship.position;
		float x = Mathf.Clamp(pos.x, -maxCameraX, maxCameraX);
		float y = Mathf.Clamp(pos.y, -maxCameraY, maxCameraY);
		mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
		minimapCamera.transform.position = mainCamera.transform.position.SetZ(minimapCamera.transform.position.z);
	}

	private void MoveCamera()
	{
		if(!IsNull(spaceship))
		{
			moveCameraAction();
		}
	}

	void HandlePowerUpCreated (PowerUp powerUp)
	{
//		SetRandomPosition(powerUp, new Vector2(50, 100), new SpawnPositioning());
//		powerUps.Add(powerUp);
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
//	bool doTick = true;
	bool gameIsOn = false;
	void Update()
	{
        if (!gameIsOn) {
            return;
        }


        if (IsNull(spaceship)) {
            spaceship = null;
        }


        if (spawner != null) {
            if (!spawner.Done()) {
                spawner.Tick();
            } else {
                if (levelEndRoutine == null) {
                    levelEndRoutine = StartCoroutine(CheckLevelEndRoutine());
                    spawner = null;
                }
            }
        }

		float dtime = Time.deltaTime;

//		if (slowTimeLeft > 0) 
//		{
//			slowTimeLeft -=  dtime;
//			enemyDtime = dtime/2f;
//		}
//		else
//		{
			enemyDtime = dtime;
//		}


		if(penetrationTimeLeft > 0)
		{
			penetrationTimeLeft -= dtime;
		}

		if(spaceship != null)
		{
			if(boundsMode)
			{
				ApplyBoundsForce(spaceship);
			}
		}

		TickBullets (bullets, dtime);
		TickObjects (gobjects, enemyDtime);
		TickObjects (drops, enemyDtime);

		CalculateGlobalPolygons (bullets);
		CalculateGlobalPolygons (gobjects);
		CalculateGlobalPolygons (drops);

		if(boundsMode)
		{
			CheckBounds(bullets, true);
			CheckBounds (gobjects, false); //TODO: spaceship in gobjects!!!
			CheckBounds(drops, false);
		}
		else
		{
			Wrap(bullets, true);
			Wrap(gobjects, true);
			Wrap(drops, false);
		}

		TickAlphaDestructors (enemyDtime);

		Tick_GO_Destructors (dtime);

		CleanBulletsList (bullets);
		BulletsHitObjects (gobjects, bullets);

		if(spaceship != null)
		{
			if(spaceship.collector != null)
			{
				spaceship.collector.Pull(spaceship.position, drops, dtime);
			}

			for (int i = drops.Count - 1; i >= 0; i--) 
			{
				int indxa, indxb;
				var drop = drops[i];
				if(PolygonCollision.IsCollides(spaceship, drop, out indxa, out indxb))
				{
                    drop.OnUserInteracted();
					Destroy(drop.gameObject);
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
/*
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
//				case EffectType.IncreasedShootingSpeed:
//					spaceship.ChangeFiringSpeed(3f, 10f);
//				break;

				case EffectType.PenetrationBullet:
					penetrationTimeLeft = 10f;
				break;

				case EffectType.SlowAsteroids:
					//slowTimeLeft = 10f;
				break;
				}	
			}
		}
        */
//		if(powerUpsCreator != null)
//			powerUpsCreator.Tick(Time.deltaTime);

		MoveCamera ();

		CheckDeadObjects (gobjects);
		CheckDeadObjects (drops);
		CheckDeadObjects (bullets, true);
	}

    //TODO: unlock ability to score persantage of non-collected drops (Call it "Dust Collector")
    //TODO: unlock ability to increase addMoneyKff (Call it "Businessman")
    public void AddMoneyOnDropInterated(int value) {
        GameResources.AddMoney((int)(value * addMoneyKff));
    }

    public void ApplyPowerUP(EffectType effect) {
        switch (effect) {
            //				case EffectType.IncreasedShootingSpeed:
            //					spaceship.ChangeFiringSpeed(3f, 10f);
            //				break;

            case EffectType.PenetrationBullet:
                penetrationTimeLeft = 10f;
                break;

            case EffectType.SlowAsteroids:
                //slowTimeLeft = 10f;
                break;
        }	
    }


	private void CalculateGlobalPolygons<T> (List<T> objs)
		where T: PolygonGameObject
	{
		for (int k = objs.Count - 1; k >= 0; k--)
		{
			var obj = objs[k];
			if(obj != null)
			{
				obj.globalPolygon = PolygonCollision.GetPolygonInGlobalCoordinates(obj);
			}
		}
	}

	private void CheckDeadObjects<T> (List<T> objs, bool nullCheck = false)
		where T: PolygonGameObject
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
						obj.deathAnimation.AnimateDeath();
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

	private void ObjectDeath(PolygonGameObject gobject)
	{
		switch (gobject.destructionType) {
		case PolygonGameObject.DestructionType.eNormal:
			SplitIntoAsteroidsAndMarkForDestuctionSmallParts(gobject);
			break;
		case PolygonGameObject.DestructionType.eComplete:
			SplitAndMarkForDestructionAllParts(gobject);
			break;
		case PolygonGameObject.DestructionType.eDisappear:
		case PolygonGameObject.DestructionType.eSptilOnlyOnHit:
			break;
		default:
			SplitIntoAsteroidsAndMarkForDestuctionSmallParts(gobject);
			break;
		}

		//TODO: refactor
		{

			foreach(var t in gobject.turrets)
			{
				t.cacheTransform.parent = null;
				t.velocity += gobject.velocity;
				t.rotation += UnityEngine.Random.Range(-150f, 150f);
				ObjectDeath(t);
			}
		}

		if(gobject.deathAnimation != null)
		{
		   	if(gobject.deathAnimation.instantiatedExplosions != null)
			{
				foreach (var e in gobject.deathAnimation.instantiatedExplosions) 
				{
					PutObjectOnDestructionQueue(e.gameObject, e.duration);
				}
			}

			float radius = gobject.deathAnimation.GetFinalExplosionRadius();
			float damage = gobject.overrideExplosionDamage >= 0 ? gobject.overrideExplosionDamage : 2f * Mathf.Pow(radius, 0.65f);

			Debug.LogError("explosion " + gobject.gameObj.name + " " + radius + " " + damage);
			int collision = -1;
//			if(gobject is SpaceShip)
//			{
//				collision = gobject.collision;
//			}
			CreatePhysicalExplosion(gobject.position, radius, damage, collision);//radius = 6f*Mathf.Sqrt(obj.polygon.area) = 6a , 1.5aa = x 
			//1.5f*gobject.polygon.area
		}

        DestroyPolygonGameObject(gobject);
		gobject = null;
	}

    private void DestroyPolygonGameObject(PolygonGameObject gobject) {
		gobject.OnDestroying ();
        Destroy(gobject.gameObj);
    }

    public void PutObjectOnDestructionQueue(GameObject obj, float duration) {
        ObjectsDestructor d = new ObjectsDestructor(obj, duration);
        PutOnFirstNullPlace(goDestructors, d);
    }

    private void ObjectsCollide(PolygonGameObject a, PolygonGameObject b) {
        if ((a.collision & b.layer) == 0)
            return;

        int indxa, indxb;
        if (PolygonCollision.IsCollides(a, b, out indxa, out indxb)) {
            var impulse = PolygonCollision.ApplyCollision(a, b, indxa, indxb);
            a.Hit(GetCollisionDamage(impulse, a, b));
            b.Hit(GetCollisionDamage(impulse, b, a));
        }
    }

    //TODO: ckecks
    private void BulletsHitObjects(List<PolygonGameObject> objs, List<PolygonGameObject> pbullets) {
        for (int i = objs.Count - 1; i >= 0; i--) {
            BulletsHitObject(objs[i], pbullets);
        }
    }

    private void CleanBulletsList(List<PolygonGameObject> pbullets) {
        if (pbullets.Count == 0)
            return;

        int fisrtNull = 0;
        for (int k = pbullets.Count - 1; k >= 0; k--) {
            if (pbullets[k] != null) {
                fisrtNull = k + 1;
                break;
            }
        }

        if (fisrtNull >= 0 && fisrtNull < pbullets.Count) {
            pbullets.RemoveRange(fisrtNull, pbullets.Count - fisrtNull);
        }
    }

    private void BulletsHitObject(PolygonGameObject obj, List<PolygonGameObject> pbullets) {

        for (int k = pbullets.Count - 1; k >= 0; k--) {
            var bullet = pbullets[k];
            if (bullet == null)
                continue;

            if ((obj.collision & bullet.layer) == 0)
                continue;

            int indxa, indxb;
            if (PolygonCollision.IsCollides(obj, bullet, out indxa, out indxb)) {
                var impulse = PolygonCollision.ApplyCollision(obj, bullet, indxa, indxb);

                if (!bullet.IsKilled()) {
                    obj.Hit(bullet.damageOnCollision + GetCollisionDamage(impulse, obj, bullet));
                }

                if (bullet.destructionType == PolygonGameObject.DestructionType.eSptilOnlyOnHit) {
                    bullet.destructionType = PolygonGameObject.DestructionType.eComplete;
                }
                bullet.Kill();
            }
        }
    }

	private void ApplyBoundsForce(PolygonGameObject p)
	{
		Vector2 curPos = p.position;
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
		var f = dir.normalized * pushingForce * dir.sqrMagnitude;
		p.velocity += (Time.deltaTime * f) / p.mass ;
	}

	private void SplitIntoAsteroidsAndMarkForDestuctionSmallParts(PolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, DestroyAfterSplitTreshold);
	}

	private void SplitAndMarkForDestructionAllParts(PolygonGameObject obj)
	{
		SplitAndDestroyThresholdParts (obj, Mathf.Infinity);
	}

	private void SplitAndDestroyThresholdParts(PolygonGameObject obj, float threshold)
	{
		DropData drop = null;
		if(obj.dropID != null)
		{
			id2drops.TryGetValue(obj.dropID, out drop);
		}

		CheckReward (obj);

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

				AddToAlphaDetructor (part, 0.7f + UnityEngine.Random.Range (0f, 1f));
			}
			else
			{
				if(obj.dropID != null)
				{
					part.dropID = obj.dropID;
				}
				else
				{
					part.destroyOnBoundsTeleport = true;
//					part.destructionType = PolygonGameObject.DestructionType.eJustDestroy;
				}
				Add2Objects(part);
			}
		}
	}

	private void CheckReward(PolygonGameObject go)
	{
		if (go.reward <= 0)
			return;

		int rewardLeft = go.reward;

		var	datas = MAsteroidsResources.Instance.asteroidsCommonData;
		int dataIndex = datas.Count - 1;
		int maxLoops = 3;

		while(rewardLeft > 0)
		{
			var val = datas[dataIndex].value;
			if(rewardLeft >= val && (maxLoops <= 0 || Math2d.Chance(0.7f)))
			{
				rewardLeft -= val;
				CreateDrop(datas[dataIndex], go.cacheTransform.position, go.polygon.R*0.6f);
			}
			else
			{
				dataIndex--;
				if(dataIndex < 0)
				{
					dataIndex = datas.Count - 1;
					maxLoops --;
				}
			}
		}
	}

	public void AddToAlphaDetructor(PolygonGameObject p, float time) {
		TimeDestuctor d = new TimeDestuctor(p, time);
		PutOnFirstNullPlace(destructors, d); 
	}

	private void CheckDrop(DropData drop, PolygonGameObject destroyingPart)
	{
		int lastDrops = (int)((drop.areaLeft/drop.startingArea) * drop.dropsCount);
		drop.areaLeft -= destroyingPart.polygon.area;
		int newDrops = (int)((drop.areaLeft/drop.startingArea) * drop.dropsCount);
		int diff = lastDrops - newDrops;
		if(diff > 0)
		{
			for (int i = 0; i < diff; i++) 
			{
//				if(!Math2d.Chance(drop.dropChance))
//					continue;

				CreateDrop(drop.asteroidData, destroyingPart.cacheTransform.position, destroyingPart.polygon.R*0.6f);
			}
		}
	}

	public ParticleSystem CreateTeleportationRing(Vector2 pos)
	{
		var anim = Instantiate(teleportationRingPrefab) as ParticleSystem;
		anim.transform.position = (Vector3)pos - new Vector3(0,0,1);
		return anim;
	}

	private void CreateDrop(MAsteroidCommonData drop, Vector3 position, float R)
	{
		Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-R, R), UnityEngine.Random.Range(-R, R), 0);
		var dropObj = ObjectsCreator.CreateDrop(drop.color, drop.value);
		var anim = Instantiate(dropAnimationPrefab) as ParticleSystem;
		anim.startColor = drop.color;
		anim.transform.parent = dropObj.transform;
		anim.transform.localPosition = new Vector3(0, 0, UnityEngine.Random.Range(1f, 1.1f));
		dropObj.cacheTransform.position =
			position + randomOffset + new Vector3(0,0,2);
		dropObj.rotation = UnityEngine.Random.Range(160f, 240f) * Mathf.Sign(UnityEngine.Random.Range(-1f,1f));
		drops.Add(dropObj);
	}

    public void CreatePowerUp(PowerupData data, Vector3 position) {
        var powerup = ObjectsCreator.CreatePowerUpDrop(data);
        powerup.cacheTransform.position = position + new Vector3(0, 0, 2);
        //powerup.rotation = UnityEngine.Random.Range(160f, 240f) * Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
        drops.Add(powerup);
    }

	public void Add2Objects(PolygonGameObject p)
	{
		p.globalPolygon = PolygonCollision.GetPolygonInGlobalCoordinates (p);
		CreateMinimapIndicatorForObject (p);
		gobjects.Add (p);
	}

	static public float GetCollisionDamage(float impulse, PolygonGameObject a,  PolygonGameObject from)
	{
		var dmg = Mathf.Abs (impulse) * Singleton<GlobalConfig>.inst.DamageFromCollisionsModifier;
		return (1f - a.collisionDefence) * from.collisionAttackModifier * dmg;
	}

	private void TickObjects<T>(List<T> list, float dtime)
		where T: PolygonGameObject
	{
		for (int i = 0; i < list.Count ; i++)
		{
            var go = list[i];
            go.Tick(dtime);
            if (go.Expired()) {
                go.Kill();
            }
		}
	}

    private void CheckBounds<T>(List<T> list, bool nullCheck)
        where T : PolygonGameObject {
        if (!nullCheck) {
            for (int i = 0; i < list.Count; i++) {
                var wrapped = CheckBounds(list[i]);
                if (wrapped && list[i].destroyOnBoundsTeleport) {
                    list[i].Kill();
                    list[i].destructionType = PolygonGameObject.DestructionType.eDisappear;
                }
            }
        } else {
            for (int i = 0; i < list.Count; i++) {
                if (list[i] != null) {
                    var wrapped = CheckBounds(list[i]);
                    if (wrapped && list[i].destroyOnBoundsTeleport) {
                        list[i].Kill();
                        list[i].destructionType = PolygonGameObject.DestructionType.eDisappear;
                    }
                }
            }
        }
    }


    private void Reposition<T>(List<T> list, Vector2 delta, bool nullCheck)
        where T : PolygonGameObject {
        if (!nullCheck) {
            for (int i = 0; i < list.Count; i++) {
                list[i].position -= delta;
            }
        } else {
            for (int i = 0; i < list.Count; i++) {
                if (list[i] != null)
                    list[i].position -= delta;
            }
        }
    }

    private void Wrap<T>(List<T> list, bool nullCheck)
        where T : PolygonGameObject {
        if (!nullCheck) {
            for (int i = 0; i < list.Count; i++) {
                var wrapped = Wrap(list[i].cacheTransform);
                if (wrapped && list[i].destroyOnBoundsTeleport) {
                    list[i].Kill();
                    list[i].destructionType = PolygonGameObject.DestructionType.eDisappear;
                }
            }
        } else {
            for (int i = 0; i < list.Count; i++) {
                if (list[i] != null) {
                    var wrapped = Wrap(list[i].cacheTransform);
                    if (wrapped && list[i].destroyOnBoundsTeleport) {
                        list[i].Kill();
                        list[i].destructionType = PolygonGameObject.DestructionType.eDisappear;
                    }
                }
            }
        }
    }

    private void TickBullets<T>(List<T> list, float dtime)
        where T : PolygonGameObject {
        for (int i = 0; i < list.Count; i++) {
            var b = list[i];
            if (b == null) {
                continue;
            }

            b.Tick(dtime);
            if (b.Expired()) {
                b.Kill();
            }
        }
    }

	private bool CheckBounds(PolygonGameObject p)
	{
		Vector3 pos = p.position;
		float R = p.polygon.R;
		bool repositioned = false;

		if(pos.x - R > screenBounds.xMax)
		{
			p.position = p.position.SetX(screenBounds.xMin - R);
			repositioned = true;
		}
		else if(pos.x + R < screenBounds.xMin)
		{
			p.position = p.position.SetX(screenBounds.xMax + R);
			repositioned = true;
		}

		if(pos.y + R < screenBounds.yMin)
		{
			p.position = p.position.SetY(screenBounds.yMax + R);
			repositioned = true;
		}
		else if(pos.y - R > screenBounds.yMax)
		{
			p.position = p.position.SetY(screenBounds.yMin - R);
			repositioned = true;
		}
		return repositioned;
	}

	private bool Wrap(Transform t)
	{
		Vector2 center = mainCamera.transform.position;
		Vector2 pos = t.position;
		float rHorisontal = screenBounds.width/2;
		float rVertical = screenBounds.height/2;
		bool wrapped = false;

		if(pos.x > center.x + rHorisontal)
		{
			t.position = t.position.SetX(pos.x - 2*rHorisontal);
			wrapped = true;
		}
		else if(pos.x < center.x - rHorisontal)
		{
			t.position = t.position.SetX(pos.x + 2*rHorisontal);
			wrapped = true;
		}
		
		if(pos.y > center.y + rVertical)
		{
			t.position = t.position.SetY(pos.y - 2*rVertical);
			wrapped = true;
		}
		else if(pos.y < center.y - rVertical)
		{
			t.position = t.position.SetY(pos.y + 2*rVertical);
			wrapped = true;
		}
		return wrapped;
	}


	public void CreateSpaceShip(MSpaceshipData data)
	{
		InputController controller = null; 
		#if UNITY_STANDALONE
		controller = new StandaloneInputController();
		#else
		tabletController.Init();
		controller = tabletController;
		#endif
		spaceship = ObjectsCreator.CreateSpaceShip (controller, data);
		Add2Objects(spaceship);
		spaceship.cacheTransform.position = mainCamera.transform.position.SetZ (0);
	}


	public void HandleGunFire (PolygonGameObject bullet)
	{
		PutOnFirstNullPlace<PolygonGameObject>(bullets, bullet);
	}

	public void HandleSpawnFire (PolygonGameObject spawn)
	{
		Add2Objects(spawn);
	}

    public void CreateDropForObject(PolygonGameObject obj, MAsteroidCommonData aData) //TODO: color + value data
	{
		obj.dropID = new DropID ();
		DropData dropData = new DropData
		{
			startingArea = obj.polygon.area,
			areaLeft = obj.polygon.area,
			dropsCount = obj.polygon.area/10f,
			dropChance = 0.8f,
			asteroidData = aData,
		};
		id2drops [obj.dropID] = dropData;
	}
	
	private void CreateMinimapIndicatorForObject(PolygonGameObject obj)
	{
		if(obj.minimapIndicator != null)
		{
			Debug.LogWarning("indicator already exists");
			return;
		}

		Color col = Color.white;

		switch (obj.layerNum) 
		{
		case CollisionLayers.ilayerUser:
			col = Color.blue;
			break;

		case CollisionLayers.ilayerTeamUser:
			col = Color.green;
			break;

		case CollisionLayers.ilayerTeamEnemies:
			col = Color.red;
			break;

		case CollisionLayers.ilayerAsteroids:
			col = Color.gray;
			break;

		case CollisionLayers.ilayerMisc:
			return;
		}

		var R = 2 + Mathf.Pow(obj.polygon.area, 0.4f);
		var verts = PolygonCreator.CreatePerfectPolygonVertices(R, 5);
		obj.minimapIndicator = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject>(verts, col);
		obj.minimapIndicator.cacheTransform.parent = obj.cacheTransform;
		obj.minimapIndicator.cacheTransform.localPosition = Vector3.zero;
		obj.minimapIndicator.gameObj.layer = LayerMask.NameToLayer("Minimap");
	}

	public void GetRandomPosition(Vector2 range, SpawnPositioning pos, out Vector2 position, out float lookAngle)
	{
		float angle = UnityEngine.Random.Range(pos.angleFromShip - pos.angleFromShipRange, pos.angleFromShip + pos.angleFromShipRange) * Mathf.Deg2Rad;
		float len = UnityEngine.Random.Range(range.x, range.y);
		var dist = new Vector2 (Mathf.Cos (angle) * len, Mathf.Sin (angle) * len);
		position = (Vector2)cameraTransform.position + dist;
		float pangle2Ship = Math2d.GetRotationDg(-dist) + pos.angle2Ship;
		lookAngle = UnityEngine.Random.Range (pangle2Ship - pos.angle2ShipRange, pangle2Ship + pos.angle2ShipRange);
//		obj.cacheTransform.rotation = Quaternion.Euler (0, 0, pangle2Ship);
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

	public static bool IsNull(PolygonGameObject target)
	{
		return target == null || target.cacheTransform == null;
	}






	/*
	 * FUTURE UPDATES
	 * stop toers from flowing after hit
	 * make destroyed spaceship parts realy easy to break
	 * turrets
     * explosion on turrets
	 * lazer fix when target destroyed
	 * destroy shaceship wrecks on bounds check?
	 * faster rocket parts fade on hit?
	 * ipad trampolines (2 if i recall correctly, about interfaces)
	 * poison asteroids
	 * camera shake on explosions
	 * sky texture?
	 * duplicating enemy and illusions enemy?
	 * gravity shield
	 * death refactor && missiles explosion on death
	 * explision by vertex
	 * gravity misiles
	 * change rotation (thruster attach) missiles?
	 * mine missiles
	 * bullets shoot missiles?
	 * shoot place levels
	 * deflect shields
	 * more efficeient stars render
	 * fire enimies
	 * cold enemies
	 * cool ships explosion (not so circled)
	 * gravity enemies
	 * texture on asteroids?
	 * joystick position fixed
	 * bullets and shooters refactoring 
	 * Z pos refactoring
	 * magnet enemy
	 * achievements and ship unlocks for them (luke - survive astroid field, reach 100% acc in more than x shots)
	 * dissolve bullet and shader
	 * bad effects on power-up destroy. Monster spawn, tough emeny, rocket launch, spawn of new very PoweR up! 
	 * bosses (arcanoid?)
	 * drops from enemies
	 * 
	 * 
	 * 
	 */
}
