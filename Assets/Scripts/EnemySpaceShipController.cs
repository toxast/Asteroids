using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpaceShipController : InputController, IGotTarget
{
	PolygonGameObject thisShip;
	PolygonGameObject target;
	List<Bullet> bullets;
	bool shooting = false;
	bool accelerating = false;
	Vector2 turnDirection;

	State state;
	private enum State
	{
		ATTACK,
		TURN,
		SIDERUN,
		NO_ACCELERATION_ATTACK,
	}

	private float siderunDuration = 1f;
	private float siderunAttackDuration = 1.5f;

	public EnemySpaceShipController(PolygonGameObject thisShip, PolygonGameObject target, List<Bullet> bullets)
	{
		this.bullets = bullets;
		this.thisShip = thisShip;
		this.target = target;
		state = State.ATTACK;
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
		while(true)
		{
			if(target != null)
			{
				Vector2 dir = target.cacheTransform.position - thisShip.cacheTransform.position;
				if(dir.sqrMagnitude < 300f)
				{
					if(Math2d.Chance(0.8f))
					{
						yield return thisShip.StartCoroutine(Siderun(dir, 0.5f));
						yield return thisShip.StartCoroutine(Attack (false, 2f));
					}
					else
					{
						yield return thisShip.StartCoroutine(TurnBack(2f));
					}
				}

				//TODO: t !+ null
				dir = target.cacheTransform.position - thisShip.cacheTransform.position;
				if(bullets.Exists(b => b != null && CheckForBulletCollision(b, dir)))
				{
					yield return thisShip.StartCoroutine(Siderun(dir, 0.5f));
				}
				
				yield return thisShip.StartCoroutine(Attack(true, 0));
			}
			else
			{
				accelerating = false;
				shooting = false;
				//TODO: break
			}
			yield return new WaitForSeconds(0f);
		}
	}

	private bool CheckForBulletCollision(Bullet b, Vector2 dir)
	{
		float angle = Math2d.AngleRAD2 (b.velocity, thisShip.velocity);
		if(angle > 4f*Mathf.PI/5f && angle < 6f*Mathf.PI/5f)
		{
			var cos = Mathf.Cos(Math2d.AngleRAD2 (b.velocity, dir));
			return cos  < -0.8f;
		}

		return false;
	}

	private IEnumerator Attack(bool acceleration, float duration)
	{
		accelerating = acceleration;
		shooting = true;

		while(duration >= 0)
		{
			if(target == null)
				yield break;

			Vector2 dir = target.cacheTransform.position - thisShip.cacheTransform.position;
			turnDirection = dir;
			yield return new WaitForSeconds(0);
			duration -= Time.deltaTime;
		}
	}

	private IEnumerator TurnBack(float duration)
	{
		accelerating = true;
		shooting = false;
		Vector2 dir = target.cacheTransform.position - thisShip.cacheTransform.position;
		turnDirection = -dir;
		yield return new WaitForSeconds(duration);
	}

	private IEnumerator Siderun(Vector2 dir, float duration)
	{
		turnDirection = Math2d.RotateVertex(dir, 1f);
		accelerating = true;
		shooting = false;
		yield return new WaitForSeconds(duration);
	}

	private IEnumerator ChangeStateIn(float sec, State s)
	{
		yield return new WaitForSeconds (sec);
		state = s;
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
}
