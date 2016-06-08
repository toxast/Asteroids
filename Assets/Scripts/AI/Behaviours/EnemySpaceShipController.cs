using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpaceShipController : InputController, IGotTarget
{
	PolygonGameObject thisShip;
	PolygonGameObject target;
	List<PolygonGameObject> bullets;
	public bool shooting{ get; private set; }
	public bool accelerating{ get; private set; }
	public bool braking{ get; private set; }
	public Vector2 turnDirection{ get; private set; }
	float bulletsSpeed;
	float teleportationDistance = 50f;


//	State state;
//	private enum State
//	{
//		ATTACK,
//		TURN,
//		SIDERUN,
//		NO_ACCELERATION_ATTACK,
//	}

	public EnemySpaceShipController(PolygonGameObject thisShip, List<PolygonGameObject> bullets, float bulletsSpeed)
	{
		this.bulletsSpeed = bulletsSpeed;
		this.bullets = bullets;
		this.thisShip = thisShip;
		thisShip.StartCoroutine (Logic ());
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public void Tick(PolygonGameObject p)
	{

	}

	private IEnumerator Logic()
	{
		float checkForBulletTime = UnityEngine.Random.Range(0.5f, 1f);
		float leftUntilRandomBeh = UnityEngine.Random.Range(2f, 3f);
		float leftUntilCheck = -1;
		while(true)
		{
			if(!Main.IsNull(target))
			{
				Vector2 dir = target.position - thisShip.position;
				if(dir.sqrMagnitude < 700f)
				{
					if(dir.sqrMagnitude < 400f)
					{
						yield return thisShip.StartCoroutine(TurnBack(3f));
					}
					else if(Math2d.Chance(0.4f))
					{
						Vector2 newDir = RotateDirection(dir, 20f, 50f);
						yield return thisShip.StartCoroutine(SetState(newDir, true, true, 0.5f));
						yield return thisShip.StartCoroutine(Attack (false, 2f));
					}
					else if(Math2d.Chance(0.4f))
					{
						yield return thisShip.StartCoroutine(Attack (false, 1.5f));
					}
					else
					{
						yield return thisShip.StartCoroutine(TurnBack(2f));
					}
				}
				else if(leftUntilCheck < 0)
				{

					if(bullets.Exists(b => CheckForBulletCollision(b)))
					{
						leftUntilCheck = checkForBulletTime;
						//yield return thisShip.StartCoroutine(Teleport());

						//yield return thisShip.StartCoroutine(FlyByArc(turnDirection, 90f, 1f));

						Vector2 newDir = RotateDirection(dir, 45f, 90f);
						yield return thisShip.StartCoroutine(SetState(newDir, true, false, 1f));
						yield return thisShip.StartCoroutine(Attack (false, 1f));
					}
					else
					{
						leftUntilCheck = checkForBulletTime/2f;
					}
				}
				else if(leftUntilRandomBeh < 0)
				{
					leftUntilRandomBeh = UnityEngine.Random.Range(2f, 3f);
					if(Math2d.Chance(0.3f))
					{
						yield return thisShip.StartCoroutine(AimAttack(false, 2));
					}
					else if(Math2d.Chance(0.3f))
					{
						yield return thisShip.StartCoroutine(Attack (false, 1.5f));
					}
					else if(Math2d.Chance(0.3f))
					{
						Vector2 newDir = RotateDirection(dir, 12, 30);
						yield return thisShip.StartCoroutine(SetState(newDir, true, false, 1f));
					}
				}
				else
				{
					yield return thisShip.StartCoroutine(Attack(true, 0));
				}
			}
			else
			{
				accelerating = false;
				shooting = false;
				braking = true;
			}
			yield return new WaitForSeconds(0f);
			leftUntilCheck -= Time.deltaTime;
			leftUntilRandomBeh -= Time.deltaTime;
		}
	}

	private bool CheckForBulletCollision(PolygonGameObject b)
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

	private IEnumerator Teleport()
	{
		Vector2 dir = target.position - thisShip.position;
		var dodgeDir = RotateDirection (dir, 15, 45);
		thisShip.position += dodgeDir.normalized * teleportationDistance;
		thisShip.cacheTransform.right = target.position - thisShip.position;
		turnDirection = thisShip.cacheTransform.right;
		yield return thisShip.StartCoroutine(Attack (false, 1.5f));
	}

	private IEnumerator Attack(bool acceleration, float duration)
	{
		accelerating = acceleration;
		shooting = true;

		while(duration >= 0)
		{
			if(Main.IsNull(target))
				yield break;

			Vector2 dir = target.position - thisShip.position;
			turnDirection = dir;
			yield return new WaitForSeconds(0);
			duration -= Time.deltaTime;
		}
	}


	private IEnumerator AimAttack(bool acceleration, float duration)
	{
		accelerating = acceleration;
		shooting = true;
		
		while(duration >= 0)
		{
			if(Main.IsNull(target))
				yield break;

			AimSystem a = new AimSystem(target.position, target.velocity - Main.AddShipSpeed2TheBullet(thisShip), thisShip.position, bulletsSpeed);
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

	private IEnumerator TurnBack(float duration)
	{
		accelerating = true;
		shooting = false;
		Vector2 dir = target.position - thisShip.position;
		turnDirection = -dir;
		yield return new WaitForSeconds(duration);
	}

	private IEnumerator SetState(Vector2 dir, bool accelerating, bool shooting, float duration)
	{
		turnDirection = dir;
		this.accelerating = accelerating;
		this.shooting = shooting;
		yield return new WaitForSeconds(duration);
	}

	private IEnumerator FlyByArc(Vector2 from, float angle, float duration)
	{
		accelerating = true;
		Vector2 to = Math2d.RotateVertex(from, angle*Mathf.Deg2Rad);
		float left = duration;
		while(left > 0)
		{
			left -= Time.deltaTime;
			turnDirection = Vector2.Lerp(from, to,  1f - left/duration);
			yield return null;
		}
	}


	private Vector2 RotateDirection(Vector2 dir, float angleMin, float angleMax)
	{
		float angle = UnityEngine.Random.Range (angleMin, angleMax) * Mathf.Sign (UnityEngine.Random.Range (-1f, 1f));
		return Math2d.RotateVertex(dir, angle*Mathf.Deg2Rad);
	}
}
