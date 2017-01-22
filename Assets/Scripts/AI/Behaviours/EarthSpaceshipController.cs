using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EarthSpaceshipController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> bullets;
	float comformDistanceMin, comformDistanceMax;
	float accuracy = 0f;
	bool turnBehEnabled = true;
	bool evadeBullets = true;

	AIHelper.Data tickData = new AIHelper.Data();

	int maxShieldsCount;
	float force;
	float maxSpeed;
	float maxSpeedSqr;
	float asteroidShieldRadius;
	float shieldRotationSpeed;
	float respawnShieldObjDuration;
	float shootInterval;
	float applyShootingForceDuration;

	List<PolygonGameObject> shields = new List<PolygonGameObject>();
	List<PolygonGameObject> brokenShields = new List<PolygonGameObject>();
	List<Vector2> targetPositions = new List<Vector2>();
	MAsteroidData ast;
	float currentAngle = 0;
	float deltaAngle;
	float rotationSpeed; 
	List<BulletObj> objectShootingWith = new List<BulletObj>();

	class BulletObj{
		public PolygonGameObject obj;
		public float startTime;
	}

	public EarthSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> bullets, MEarthSpaceshipData data) : base(thisShip)
	{
		asteroidShieldRadius = data.shieldRadius;
		shieldRotationSpeed = data.shieldRotationSpeed;
		maxShieldsCount = data.elementsCount;
		shootInterval = data.shootInterval;
		applyShootingForceDuration = data.applyShootingForceDuration;
		respawnShieldObjDuration = data.respawnShieldObjDuration;
		for (int i = 0; i < maxShieldsCount; i++) {
			targetPositions.Add (Vector2.zero);
		}
		rotationSpeed = 2f * Mathf.PI * asteroidShieldRadius / (360f / shieldRotationSpeed);
		ast = data.asteroidData;
		deltaAngle = 360f / maxShieldsCount;
		force = thisShip.thrust  + rotationSpeed / 0.5f;
		maxSpeed = rotationSpeed + thisShip.maxSpeed;
		maxSpeedSqr = maxSpeed * maxSpeed;
		this.bullets = bullets;

		comformDistanceMax = 50;
		comformDistanceMin = 30;

		thisShip.StartCoroutine (SpawnShieldObjects ());
		thisShip.StartCoroutine (RotateShields ());
		thisShip.StartCoroutine (LogicShip ());
		thisShip.StartCoroutine (LogicShoot ());
		thisShip.StartCoroutine (AimShootingObjects ());

		var accData = data.accuracy;
		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			thisShip.StartCoroutine (AccuracyChanger (accData));
	}

	private IEnumerator LogicShip() 
	{
		yield return null;
		while (true) {
			
			yield return null;
		}
	}

	private IEnumerator SpawnShieldObjects() 
	{
		yield return null;

		for (int i = 0; i < maxShieldsCount; i++) {
			var shieldObj = CreateShieldObj ();
			shields.Add (shieldObj);
		}

		while (true) {
			bool hasEmpty = shields.Exists (a => a == null);
			if(hasEmpty) {
				yield return new WaitForSeconds (respawnShieldObjDuration);
				int indx = shields.FindIndex (a => a == null);
				if (indx >= 0) {
					var shieldObj = CreateShieldObj ();
					shields[indx] = shieldObj;
				}
			}
			yield return null;
		}
	}

	private PolygonGameObject CreateShieldObj() {
		var shieldObj = ObjectsCreator.CreateAsteroid (ast);
		shieldObj.position = thisShip.position;
		shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(thisShip.cacheTransform.position.z + 1f);
		Singleton<Main>.inst.Add2Objects (shieldObj);
		return shieldObj;
	}

	private IEnumerator RotateShields()
	{
		while (true) {
			currentAngle += shieldRotationSpeed * Time.deltaTime;
			float angle = currentAngle;
			for (int i = 0; i < shields.Count; i++) {
				var item = shields [i];
				if (Main.IsNull (item)) {
					shields [i] = null;
				} else {
					var radAngle = angle * Mathf.Deg2Rad;
					Vector2 targetPos = thisShip.position + asteroidShieldRadius * new Vector2 (Mathf.Cos (radAngle), Mathf.Sin (radAngle));
					Vector2 targetVelocity = thisShip.velocity + rotationSpeed * (-Math2d.MakeRight (targetPos - thisShip.position) + 0.1f * (thisShip.position - targetPos)).normalized;
					targetPositions [i] = targetPos;
					FollowAim aim = new FollowAim(targetPos, targetVelocity, item.position, item.velocity, force);
					item.Accelerate (Time.deltaTime, force, 0.5f, maxSpeed, maxSpeedSqr, aim.forceDir.normalized); 
				}
				angle += deltaAngle;
			}
			yield return null;
		}
	}

	private IEnumerator LogicShoot() {
		yield return new WaitForSeconds (shootInterval/2f);
		while (true) {
			if (target != null) {
				var notNull = shields.FindAll (a => !Main.IsNull (a));
				if (notNull.Count > 0) {
					int rnd = Random.Range (0, notNull.Count);
					var obj = notNull [rnd];
					for (int i = 0; i < shields.Count; i++) {
						if(shields[i] == obj) {
							shields [i] = null;
							break;
						}
					}
					obj.destroyOnBoundsTeleport = true;
					Main.PutOnFirstNullPlace (objectShootingWith, new BulletObj{ obj = obj, startTime = Time.time }); ;
				}
				yield return new WaitForSeconds (shootInterval);
			} else {
				yield return new WaitForSeconds (1f);
			}
		}
	}

	//TODO: force animation
	private IEnumerator AimShootingObjects() 
	{
		while (true) {
			if (target != null) {
				for (int i = 0; i < objectShootingWith.Count; i++) {
					var bulletObj = objectShootingWith [i];
					if (bulletObj != null) {
						if (Main.IsNull (bulletObj.obj)) {
							objectShootingWith [i] = null;
						} else {
							if (Time.time - bulletObj.startTime > applyShootingForceDuration) {
								objectShootingWith [i] = null;
							} else {
								var item = bulletObj.obj;
								SuicideAim aim = new SuicideAim (target.position, target.velocity, item.position, item.velocity, 300f, 1f);
								item.Accelerate (Time.deltaTime, force, 0.5f, maxSpeed, maxSpeedSqr, aim.direction.normalized); 
							}
						}
					} 
				}
			}
			yield return null;
		}
	}

	private IEnumerator AccuracyChanger(AccuracyData data)
	{
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				AIHelper.ChangeAccuracy(ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds(dtime);
		}
	}

}

