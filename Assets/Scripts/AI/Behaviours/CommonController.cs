using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommonController : InputController, IGotTarget
{
	SpaceShip thisShip;
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;
	
	IPolygonGameObject target;
	List<IBullet> bullets;
	float bulletsSpeed;
//	float bulletLifeTime;
	
	float comformDistanceMin, comformDistanceMax;
	//float comformDistanceMinSqr, comformDistanceMaxSqr;
	
	float accuracy = 0f;
	
//	int attacks = 0;

	bool turnBehEnabled = true;
	bool evadeBullets = true;
	
	public CommonController(SpaceShip thisShip, List<IBullet> bullets, Gun gun)
	{
		this.bulletsSpeed = gun.bulletSpeed;
//		this.bulletLifeTime = gun.lifeTime;
		this.bullets = bullets;
		this.thisShip = thisShip;
		thisShip.StartCoroutine (Logic ());
		thisShip.StartCoroutine (AccuracyChanger ());
		thisShip.StartCoroutine (BehavioursRandomTiming ());
		comformDistanceMax = gun.Range;
		comformDistanceMin = 30;
		
		float evadeDuration = (90f / thisShip.turnSpeed) + ((thisShip.polygon.R) * 2f) / (thisShip.maxSpeed * 0.8f);
		//Debug.LogWarning("initial evade duration " + evadeDuration);
		evadeBullets = evadeDuration < 1.2f;
		turnBehEnabled = evadeDuration < 3f;
		if(turnBehEnabled)
		{
			untilTurnMin = Mathf.Max(2f, Mathf.Sqrt(evadeDuration) * 2.5f);
			untilTurnMax = untilTurnMin * 1.8f;
			//Debug.LogWarning(untilTurnMin + " - " + untilTurnMax);
		}

		untilCheckAccelerationMin = evadeDuration / 6f;
		untilCheckAccelerationMax = untilCheckAccelerationMin * 2f;
	}
	
	public Vector2 TurnDirection ()
	{
		return turnDirection;
	}
	
	public bool IsShooting()
	{
		return shooting;
	}
	
	public bool IsAccelerating()
	{
		return accelerating;
	}
	
	public void SetTarget(IPolygonGameObject target)
	{
		this.target = target;
	}


	public void Tick(PolygonGameObject p)
	{
		
	}


	bool turnAction = false;
	float untilTurn = 0f;
	float untilTurnMax = 4.5f;
	float untilTurnMin = 2.5f;

	bool checkBulletsAction = false;
	float untilBulletsEvade = 1f;
	float untilBulletsEvadeMax = 4f;
	float untilBulletsEvadeMin = 1f;

	bool checkAccelerationAction = false;
	float untilCheckAcceleration = 0f;
	float untilCheckAccelerationMax = 0.1f;
	float untilCheckAccelerationMin = 0.0f;

	private IEnumerator BehavioursRandomTiming()
	{
		while(true)
		{
			TickActionVariable(ref turnAction, ref untilTurn, untilTurnMin, untilTurnMax);

			TickActionVariable(ref checkBulletsAction, ref untilBulletsEvade, untilBulletsEvadeMin, untilBulletsEvadeMax);

			TickActionVariable(ref checkAccelerationAction, ref untilCheckAcceleration, untilCheckAccelerationMin, untilCheckAccelerationMax);

			yield return null;
		}
	}

	private void TickActionVariable(ref bool action, ref float timeLeft, float min, float max)
	{
		if(!action)
			timeLeft -= Time.deltaTime;
		
		if(timeLeft < 0)
		{
			timeLeft = UnityEngine.Random.Range (min, max);
//			//Debug.LogWarning("in " + timeLeft);
			action = true;
		}
	}

	public bool IsBraking()
	{
		return braking;
	}
	
	private IEnumerator Logic()
	{
		this.shooting = true;

		float checkBehTimeInterval = 0.1f;
		float checkBehTime = 0;
		bool behaviourChosen = false;

		while(true)
		{
			if(!Main.IsNull(target))
			{
				behaviourChosen = false;
				checkBehTime -= Time.deltaTime;

				if(checkBehTime <= 0)
				{
					checkBehTime = checkBehTimeInterval;

					comformDistanceMin = Mathf.Min(target.polygon.R + thisShip.polygon.R, comformDistanceMax * 0.7f);
					//Debug.LogWarning(comformDistanceMin + " " + comformDistanceMax);
					Vector2 dir = target.position - thisShip.position;
					float dist2targetCenter = dir.magnitude;
					float dist2TargetR = dist2targetCenter - (thisShip.polygon.R + target.polygon.R);
//					float dist2TargetRmin = dist2targetCenter - (thisShip.polygon.R + target.polygon.R);
					Vector2 dirNorm = dir/dist2targetCenter;
					float vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
					float vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
					float evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity - thisShip.velocity, dir));

					if(checkAccelerationAction)
					{
						checkAccelerationAction = false;
						
						bool iaccelerate = false;
						if(dist2TargetR > (comformDistanceMax + comformDistanceMin)/2f)
						{
							vprojThis = Vector2.Dot(thisShip.velocity, dirNorm);
							vprojTarget = Vector2.Dot(target.velocity, -dirNorm);
							iaccelerate = (vprojThis + vprojTarget) < 0 ;
						}
						SetAcceleration(iaccelerate);
					}

					if(!behaviourChosen)
					{
						if(target.mass > thisShip.mass*0.8f)
						{
							if(dist2TargetR < 1.3f*(vprojThis + vprojTarget))
							{
								////Debug.LogWarning("collision evade");

								//уклон от столкновения и атака
								//////Debug.LogWarning("уклон и атака");
								float angle = UnityEngine.Random.Range(90-15, 90+25);
								var newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
								float duration = (angle / thisShip.turnSpeed) + ((thisShip.polygon.R + target.polygon.R) * 2f) / (thisShip.maxSpeed * 0.8f);// UnityEngine.Random.Range(0.5f, 1.5f);
								//Debug.LogWarning("evade COLLISION duration = " + duration);

								behaviourChosen = true;
								yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); 
							}
						}
					}

					if (!behaviourChosen)
					{
						if (evadeBullets && checkBulletsAction)
						{
							foreach (var b in bullets)
							{
								if (BulletDanger(b))
								{
									////Debug.LogWarning("bullet evade");
									behaviourChosen = true;
									var bulletDir = b.position - thisShip.position;
									float bulletEvadeSign = Mathf.Sign(Math2d.Cross2(b.velocity, bulletDir));
									float angle = 90;
									var newDir = Math2d.RotateVertex(bulletDir.normalized, bulletEvadeSign * angle * Mathf.Deg2Rad);
									float duration = (angle / thisShip.turnSpeed) + ((thisShip.polygon.R + b.polygon.R) * 2f) / (thisShip.maxSpeed / 2f);
									//Debug.LogWarning("evade BULLET duration = " + duration);
									yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration));
									break;
								}
							}

							checkBulletsAction = false;
							if(!behaviourChosen)
							{
								untilBulletsEvade = untilBulletsEvadeMin;
							}
						}
					}


					if(turnBehEnabled)
					{
						if(!behaviourChosen)
						{
							if(turnAction)
							{
								if(dist2TargetR > comformDistanceMax || dist2TargetR < comformDistanceMin)
								{
									float angle = 0;
									if(dist2TargetR > comformDistanceMax)
									{
										////Debug.LogWarning("far");
										angle = UnityEngine.Random.Range(-30, 30);
										
									}
									else
									{
										////Debug.LogWarning("close");
										angle = -evadeSign * UnityEngine.Random.Range(80, 100);
									}
									float restoreDist =  UnityEngine.Random.Range(30f, 40f);
									float duration =  restoreDist / thisShip.maxSpeed;

									var dirFromTarget =  Math2d.RotateVertex(-dirNorm, angle * Mathf.Deg2Rad);  
									dirFromTarget *= (comformDistanceMax + comformDistanceMin)/2f;
									dirFromTarget *= UnityEngine.Random.Range(0.8f, 1.2f);
									var newDir =  dir + dirFromTarget;

									behaviourChosen = true;
									yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); 
									turnAction = false;
								}
							}
						}
					}

					if(turnBehEnabled)
					{
						if(!behaviourChosen)
						{
							if(turnAction)
							{
								Vector2 newDir;
								if(dist2TargetR > (comformDistanceMax + comformDistanceMin)/2f)
								{
									float angle = UnityEngine.Random.Range(30, 80) * Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
									newDir = Math2d.RotateVertex(dirNorm, angle * Mathf.Deg2Rad);
								}
								else
								{
									float angle = UnityEngine.Random.Range(80, 110);
									newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
								}

								float turnDist =  UnityEngine.Random.Range(30f, 40f);
								float duration =  turnDist / thisShip.maxSpeed;

								//Debug.LogWarning("turn duration " + duration);
								behaviourChosen = true;
								yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); //TODO: by maxV
								turnAction = false;
							}
						}
					}
				}

				if(!behaviourChosen)
				{
					shooting = true;
					Vector2 relativeVelocity = (target.velocity - Main.AddShipSpeed2TheBullet(thisShip));
					AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity, thisShip.position, bulletsSpeed);
					if(a.canShoot)
					{
						turnDirection = a.direction;
					}
					else
					{
						turnDirection = target.position - thisShip.position;
					}
					yield return null;
				}
			}
			else
			{
				Brake();
				shooting = false;
				yield return new WaitForSeconds(0.5f); 
				checkBehTime -= 0.5f;
			}
		}
	}
	
	private void SetAcceleration(bool accelrate)
	{
		//Debug.LogWarning ("accelerating " + accelrate);
		accelerating = accelrate;
		
		if (accelerating)
			braking = false;
	}
	
	private void Accelerate()
	{
		accelerating = true;
		braking = false;
	}
	
	private void Brake()
	{
		accelerating = false;
		braking = true;
	}
	
	private IEnumerator EvadeCollision(Vector2 dirNorm, float evadeSign)
	{
		float angle = UnityEngine.Random.Range(90-25, 90+25);
		var newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
		float duration = 0.5f;
		yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration));
	}
	
	private IEnumerator SetState(Vector2 dir, bool accelerating, bool shooting, float duration)
	{
		turnDirection = dir;
		SetAcceleration (accelerating);
		this.shooting = shooting;
		yield return new WaitForSeconds(duration);
	}
	
	private IEnumerator AccuracyChanger()
	{
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = 0.5f;
		float time2reachFullAccuracy = 5f;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				float sameVelocityMesure = 0;
				if(Math2d.ApproximatelySame(target.velocity, Vector2.zero) || Math2d.ApproximatelySame(lastDir, Vector2.zero))
				{
					sameVelocityMesure = 1;
				}
				else
				{
					var cos =  Math2d.Cos(target.velocity, lastDir); 
					sameVelocityMesure = (cos > 0.9) ? 1 : -1; //TODO: 0.9?
				}
				accuracy += sameVelocityMesure*dtime/time2reachFullAccuracy;
				accuracy = Mathf.Clamp(accuracy, 0, 1);
				//////Debug.LogWarning(accuracy);
				lastDir = target.velocity;
			}
			yield return new WaitForSeconds(dtime);
		}
	}
	
	private IEnumerator AimAttack(bool acceleration, float duration, float accuracy)
	{
		SetAcceleration(acceleration);
		this.shooting = true;
		
		while(duration >= 0)
		{
			if(Main.IsNull(target))
				yield break;
			
			Vector2 relativeVelocity = (target.velocity - Main.AddShipSpeed2TheBullet(thisShip));
			AimSystem a = new AimSystem(target.position, accuracy * relativeVelocity, thisShip.position, bulletsSpeed);
			if(a.canShoot)
			{
				turnDirection = a.direction;
			}
			else
			{
				turnDirection = target.position - thisShip.position;
			}
			yield return new WaitForSeconds(0);
			duration -= Time.deltaTime;
		}
	}
	
	private bool BulletDanger(IBullet b)
	{
		if (b == null)
			return false;
		
		if((b.collision & thisShip.layer) == 0)
			return false;
		
		Vector2 dir2thisShip = thisShip.position - b.position;
		
		float angleVS = Math2d.AngleRad (b.velocity, thisShip.velocity);
		float cosVS = Mathf.Cos (angleVS);
		if(cosVS < -0.9f)
		{
			var cos = Mathf.Cos(Math2d.AngleRad (b.velocity, dir2thisShip));
			return cos  > 0.9f;
		}
		
		return false;
	}
	
	private Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
		return Math2d.RotateVertex(dir, angle*Mathf.Deg2Rad);
	}
	
	
}
