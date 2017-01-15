using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EarthSpaceshipController : BaseSpaceshipController, IGotTarget
{
	List<PolygonGameObject> bullets;
	float comformDistanceMin, comformDistanceMax;
	float accuracy = 0f;
	bool turnBehEnabled = true;
	bool evadeBullets = true;

	AIHelper.Data tickData = new AIHelper.Data();

	float asteroidShieldRadius = 30f;
	float shieldRotationSpeed = 30f;
	int maxShieldsCount = 8;
	float force = 5;
	float maxSpeed = 30;
	float maxSpeedSqr;

	List<PolygonGameObject> shields;
	List<Vector2> targetPositions;
	MAsteroidData ast;
	float currentAngle = 0;
	float deltaAngle;
	float rotationSpeed; 
	public EarthSpaceshipController (SpaceShip thisShip, List<PolygonGameObject> bullets, MEarthSpaceshipData data) : base(thisShip)
	{
		rotationSpeed = 2f * Mathf.PI * asteroidShieldRadius / (360f / shieldRotationSpeed);
		maxShieldsCount = data.maxShields;
		ast = data.asteroidData;
		deltaAngle = 360f / maxShieldsCount;
		maxSpeed = rotationSpeed + thisShip.maxSpeed;
		maxSpeedSqr = maxSpeed * maxSpeed;
		this.bullets = bullets;

		comformDistanceMax = 50;
		comformDistanceMin = 30;

		thisShip.StartCoroutine (Logic ());

		var accData = data.accuracy;
		accuracy = accData.startingAccuracy;
		if(accData.isDynamic)
			thisShip.StartCoroutine (AccuracyChanger (accData));
	}

	private IEnumerator Logic()
	{
		yield return null;

		shields = new List<PolygonGameObject> (maxShieldsCount);
		for (int i = 0; i < maxShieldsCount; i++) {
			var shieldObj = ObjectsCreator.CreateAsteroid (ast);
			shieldObj.position = thisShip.position;
			shields.Add (shieldObj);
			Singleton<Main>.inst.Add2Objects (shieldObj);
		}
		targetPositions = new List<Vector2> (maxShieldsCount);
		for (int i = 0; i < maxShieldsCount; i++) {
			targetPositions.Add (thisShip.position);
		}

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
					Vector2 targetVelocity = rotationSpeed * (/*-Math2d.MakeRight (targetPos - thisShip.position) + */(thisShip.position - targetPos)).normalized;
					targetPositions [i] = targetPos;
					SuicideAim aim = new SuicideAim (targetPos, targetVelocity, item.position, item.velocity, 300, 1f);
					Debug.DrawLine (thisShip.position, targetPos);
					Debug.DrawLine (targetPos, targetPos + targetVelocity);
					Debug.DrawLine ( item.position,  item.position + item.velocity, Color.green);
					Debug.DrawLine ( item.position,  item.position + aim.direction.normalized * 10, Color.cyan);
//					if (aim.canShoot) {
//						if (aim.time * item.velocity.magnitude > (targetPos - item.position).magnitude && item.velocity.sqrMagnitude > targetVelocity.sqrMagnitude) {
//							item.Brake (Time.deltaTime, force * 0.1f);
//						} else {
//							item.Accelerate (Time.deltaTime, force, 0.5f, maxSpeed, maxSpeedSqr, aim.direction.normalized); 
//						}
//					} else {
						item.Accelerate (Time.deltaTime, force, 0.5f, maxSpeed, maxSpeedSqr, aim.direction.normalized); 
//					}
				}
				angle += deltaAngle;
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

