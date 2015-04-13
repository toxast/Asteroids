using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FastSpaceshipAttackController : InputController, IGotTarget
{
	PolygonGameObject thisShip;
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;

	IPolygonGameObject target;
	List<IBullet> bullets;
	float bulletsSpeed;
	float bulletLifeTime;

	float comformDistanceMin, comformDistanceMax;
	//float comformDistanceMinSqr, comformDistanceMaxSqr;

	float accuracy = 0f;

	int attacks = 0;

	public FastSpaceshipAttackController(PolygonGameObject thisShip, List<IBullet> bullets, Gun gun)
	{
		this.bulletsSpeed = gun.bulletSpeed;
		this.bulletLifeTime = gun.lifeTime;
		this.bullets = bullets;
		this.thisShip = thisShip;
		thisShip.StartCoroutine (Logic ());
		thisShip.StartCoroutine (AccuracyChanger ());

		comformDistanceMax = gun.Range;
		comformDistanceMin = 30;
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
	
	public bool IsBraking()
	{
		return braking;
	}
	
	private IEnumerator Logic()
	{
		while(true)
		{
			if(!Main.IsNull(target))
			{
				bool behaviourChosen = false;

				Vector2 dir = target.position - thisShip.position;
				float dist = dir.magnitude;
				Vector2 dirNorm = dir/dist;
				float vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
				float vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
				float evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity - thisShip.velocity, dir));

				if(!behaviourChosen)
				{
					if(dist < thisShip.polygon.R + target.polygon.R + 15 + 1.5*(vprojThis + vprojTarget))
					{
						//Debug.LogWarning("collision evade");
						behaviourChosen = true;
						//уклон от столкновения и атака
						////Debug.LogWarning("уклон и атака");
						float angle = UnityEngine.Random.Range(90-15, 90+25);
						var newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
						float duration = UnityEngine.Random.Range(0.5f, 1.5f);
						yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); //TODO: by maxV
						if(!Main.IsNull(target))
						{
							dir = target.position - thisShip.position;
							if(vprojThis < 0)
							{
								//Debug.LogWarning("after collision attack");
								duration = UnityEngine.Random.Range(0.8f, 1.6f);
								yield return thisShip.StartCoroutine(AimAttack(false, duration, accuracy));
							}
						}
					}
				}

				if(!behaviourChosen)
				{
					foreach(var b in bullets)
					{
						if(BulletDanger(b))
						{
							//Debug.LogWarning("bullet evade");
							behaviourChosen = true;
							var bulletDir =  b.position - thisShip.position;
							float bulletEvadeSign = Mathf.Sign(Math2d.Cross2(b.velocity, bulletDir));
							float angle = 90;
							var newDir = Math2d.RotateVertex(bulletDir.normalized, bulletEvadeSign * angle * Mathf.Deg2Rad);
							float duration = 0.7f;
							yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration));
							break;
						}
					}
				}

				if(!behaviourChosen)
				{
					behaviourChosen = true;
					if(dist > comformDistanceMax || dist < comformDistanceMin)
					{
						float angle = 0;
						float duration = 0;
						if(dist > comformDistanceMax)
						{
							//Debug.LogWarning("far");
							angle = UnityEngine.Random.Range(-30, 30);

						}
						else
						{
							//Debug.LogWarning("close");
							angle = -evadeSign * UnityEngine.Random.Range(80, 100);
						}
						duration = UnityEngine.Random.Range(0.7f, 1.7f);

						var dirFromTarget =  Math2d.RotateVertex(-dirNorm, angle * Mathf.Deg2Rad);  
						dirFromTarget *= (comformDistanceMax + comformDistanceMin)/2f;
						dirFromTarget *= UnityEngine.Random.Range(0.8f, 1.2f);
						var newDir =  dir + dirFromTarget;

						yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); 
						if(!Main.IsNull(target))
						{
							dir = target.position - thisShip.position;
							if(dir.magnitude > comformDistanceMin)
							{
								//Debug.LogWarning("the attack");
								yield return thisShip.StartCoroutine(AimAttack(false, 1f, accuracy));
							}
						}
					}
					else
					{
						if(attacks != 0 && (Math2d.Chance(0.3f) || attacks > 2))
						{ 
							//Debug.LogWarning("turn");

							attacks = 0;

							Vector2 newDir;
							if(dist > (comformDistanceMax + comformDistanceMin)/2f)
							{
								float angle = UnityEngine.Random.Range(30, 80) * Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
								newDir = Math2d.RotateVertex(dirNorm, angle * Mathf.Deg2Rad);
							}
							else
							{
								float angle = UnityEngine.Random.Range(80, 110);
								newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
							}
							float duration =  UnityEngine.Random.Range(0.7f, 1.7f);
							yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); //TODO: by maxV
//							bool accelration = thisShip.velocity.magnitude < 3f;
//							duration = 1f;
//							yield return thisShip.StartCoroutine(AimAttack(accelration, duration, accuracy));
						}
						else
						{
							attacks++;
							//Debug.LogWarning("AimAttack");
							bool acc = thisShip.velocity.magnitude < 3f || dist > (comformDistanceMax*0.6f + comformDistanceMin*0.4f);
							yield return thisShip.StartCoroutine(AimAttack(acc, 1f, accuracy));
						}
					}
				}
			}
			else
			{
				Brake();
				shooting = false;
				yield return new WaitForSeconds(0.5f);
			}

			yield return null;
		}
	}

	private void SetAcceleration(bool accelrate)
	{
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
				////Debug.LogWarning(accuracy);
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

			Vector2 relativeVelocity = (target.velocity - Main.AddSpipSpeed2TheBullet(thisShip));
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
