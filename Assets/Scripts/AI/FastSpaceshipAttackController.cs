using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FastSpaceshipAttackController : InputController, IGotTarget
{

	PolygonGameObject thisShip;
	IPolygonGameObject target;
	List<IBullet> bullets;
	bool shooting = false;
	bool accelerating = false;
	bool braking = false;
	Vector2 turnDirection;
	float bulletsSpeed;

	float accuracy = 0f;


	public FastSpaceshipAttackController(PolygonGameObject thisShip, List<IBullet> bullets, float bulletsSpeed)
	{
		this.bulletsSpeed = bulletsSpeed;
		this.bullets = bullets;
		this.thisShip = thisShip;
		thisShip.StartCoroutine (Logic ());
		thisShip.StartCoroutine (AccuracyChanger ());
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
				//bool behaviourChosen = false;

				Vector2 dir = target.position - thisShip.position;
				Vector2 dirNorm = dir.normalized;
				float vprojThis = Vector2.Dot(thisShip.velocity, dirNorm); //+ means towards other ship
				float vprojTarget = Vector2.Dot(target.velocity, -dirNorm); //+ means towards other ship
				float evadeSign = Mathf.Sign(Math2d.Cross2(target.velocity, dir));

				if(dir.magnitude < thisShip.polygon.R + target.polygon.R + 5 + 1*(vprojThis + vprojTarget))
				{
					//уклон от столкновения и атака
					//Debug.LogWarning("уклон и атака");
					float angle = UnityEngine.Random.Range(90-25, 90+25);
					var newDir = Math2d.RotateVertex(dirNorm, evadeSign * angle * Mathf.Deg2Rad);
					float duration = 0.5f;
					yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); //TODO: by maxV
					bool accelration = UnityEngine.Random.Range(-1f, 1f) > 0;
					duration = 1f;
					yield return thisShip.StartCoroutine(AimAttack(accelration, duration, accuracy));
				}
				else 
				{
					bool bulletDanger = false;

					foreach(var b in bullets)
					{
						if(BulletDanger(b))
						{
							var bulletDir =  b.position - thisShip.position;
							float bulletEvadeSign = Mathf.Sign(Math2d.Cross2(b.velocity, bulletDir));
							float angle = 90;
							var newDir = Math2d.RotateVertex(bulletDir.normalized, bulletEvadeSign * angle * Mathf.Deg2Rad);
							float duration = 0.5f;
							yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration));
							bulletDanger = true;
							break;
						}
					}
//
					if(!bulletDanger)
					{
						if(bullets.Exists(b => BulletDanger(b)))
						{
							Vector2 newDir = RotateDirection(dir, 45f, 90f);
							yield return thisShip.StartCoroutine(SetState(newDir, true, false, 1f));
						}
						else if(vprojThis > 0 && vprojTarget < 0) //этот догоняет свою цель
						{
							//Debug.LogWarning("атака сзади");
							//перехватывающий маневр и атака
							float angle = UnityEngine.Random.Range(-30, 60);
							float duration = 0.5f;
							var newDir =  Math2d.RotateVertex(dirNorm, (-evadeSign) * angle * Mathf.Deg2Rad); 
							//Debug.LogWarning("old dir: " + (Vector2)thisShip.cacheTransform.right.normalized + " new: " + newDir);
							yield return thisShip.StartCoroutine(SetState(newDir, true, false, duration)); 
							bool accelration = UnityEngine.Random.Range(-1f, 1f) > 0;
							duration = 1f;
							yield return thisShip.StartCoroutine(AimAttack(accelration, duration, accuracy));
							shooting = false;
						}
						else
						{
							turnDirection = dir;
							accelerating = true;
						}
					}
				}
			}
			yield return null;
		}
	}

	private IEnumerator SetState(Vector2 dir, bool accelerating, bool shooting, float duration)
	{
		turnDirection = dir;
		this.accelerating = accelerating;
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
				//Debug.LogWarning(accuracy);
				lastDir = target.velocity;
			}
			yield return new WaitForSeconds(dtime);
		}
	}

	private IEnumerator AimAttack(bool acceleration, float duration, float accuracy)
	{
		accelerating = acceleration;
		shooting = true;
		
		while(duration >= 0)
		{
			if(Main.IsNull(target))
				yield break;

			Vector2 relativeVelocity = ((Vector2)target.velocity - Main.AddSpipSpeed2TheBullet(thisShip));
			AimSystem a = new AimSystem(target.cacheTransform.position, accuracy * relativeVelocity, thisShip.cacheTransform.position, bulletsSpeed);
			if(a.canShoot)
			{
				turnDirection = a.direction;
			}
			else
			{
				turnDirection = target.cacheTransform.position - thisShip.cacheTransform.position;
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
		
		float angleVS = Math2d.AngleRAD2 (b.velocity, thisShip.velocity);
		float cosVS = Mathf.Cos (angleVS);
		if(cosVS < -0.9f)
		{
			var cos = Mathf.Cos(Math2d.AngleRAD2 (b.velocity, dir2thisShip));
			return cos  > 0.9f;
		}
		
		return false;
	}

	private Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
		return Math2d.RotateVertex(dir, angle*Math2d.PIdiv180);
	}


}
