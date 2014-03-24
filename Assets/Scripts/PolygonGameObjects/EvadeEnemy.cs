using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject
{
	public event System.Action<ShootPlace, Transform> FireEvent;

	public static Vector2[] vertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
			new Vector2(2f, -0.25f),
			new Vector2(1.25f, -0.25f),
			new Vector2(1f, -1f),
            new Vector2(0.25f, -1.25f),
            new Vector2(-0.75f, -0.75f),
            new Vector2(-1f, 0f),
		}
		, 1f).ToArray();


	private float movingSpeed = 7f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;


	private List<Bullet> bullets;
	private SpaceShip target;

	private bool avoiding = false;
	private Vector3 currentSafePoint;
	private float currentAimAngle = 0;

	//if angle to target bigger than this - dont even try to shoot
	private float rangeAngle = 15f; 

	private int goRoundTargetSign = 1;

	Rotaitor cannonsRotaitor;
	ShootPlace shooter;
	public void SetShooter(ShootPlace shooter)
	{
		this.shooter = shooter;

		float rotatingSpeed = 55f;
		cannonsRotaitor = new Rotaitor(cacheTransform, rotatingSpeed);
	}

	public void SetBulletsList(List<Bullet> bullets)
	{
		this.bullets = bullets;
	}

	public void SetTarget(SpaceShip ship)
	{
		this.target = ship;
	}

	void Start()
	{
		StartCoroutine(Evade());

		StartCoroutine(Aim());

		StartCoroutine(FireCoroutine());

		StartCoroutine(ChangeRotationSign());
	}

	public override void Tick(float delta)
	{
		float deltaDist = movingSpeed * delta;

		if(avoiding)
		{
			MoveToSafePoint(deltaDist);
		}
		else
		{
			KeepTargetDistance(deltaDist);
		}

		RotateCannon(delta);
	}

	private void MoveToSafePoint(float deltaDist)
	{
		Vector2 dist = cacheTransform.position - currentSafePoint;
		if(dist.sqrMagnitude < deltaDist*deltaDist)
		{
			cacheTransform.position = currentSafePoint;
			//avoiding = false;
		}
		else
		{
			cacheTransform.position += (currentSafePoint - cacheTransform.position).normalized*deltaDist;
		}
	}

	private void KeepTargetDistance(float deltaDist)
	{
		Vector2 dist = target.cacheTransform.position - cacheTransform.position;
		float sqrDist = dist.sqrMagnitude;
		if(sqrDist < minDistanceToTargetSqr)
		{
			cacheTransform.position -= (Vector3) dist.normalized * deltaDist;
		}
		else if (sqrDist > maxDistanceToTargetSqr)
		{
			cacheTransform.position += (Vector3) dist.normalized * deltaDist;
		}

		{
			Vector2 rotateDirection = new Vector2(dist.y, -dist.x).normalized; //right
			//rotate around
			cacheTransform.position += (Vector3) rotateDirection * deltaDist * goRoundTargetSign;
		}
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
 	}

	IEnumerator Evade()
	{
		while(true)
		{
			EvadeSystem evade = new EvadeSystem(bullets, this);
			avoiding = !evade.safeAtCurrentPosition;
			currentSafePoint = evade.safePosition;
			yield return new WaitForSeconds(0.1f); 
		}
	}

	private IEnumerator Aim()
	{
		float aimInterval = 0.5f;

		while(true)
		{
			AimSystem aim = new AimSystem(target.cacheTransform.position, target.speed, cacheTransform.position, shooter.speed);
			if(aim.canShoot)
			{
				currentAimAngle = aim.directionAngle / Math2d.PIdiv180;
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private IEnumerator FireCoroutine()
	{
		float defaultInterval = shooter.fireInterval;
		float shortInterval = defaultInterval/2f;

		float shotInterval = defaultInterval;
		while(true)
		{
			yield return new WaitForSeconds(shotInterval);

			if(Mathf.Abs(cacheTransform.rotation.eulerAngles.z - currentAimAngle) < rangeAngle)
			{
				Fire();
				shotInterval = defaultInterval;
			}
			else
			{
				shotInterval = shortInterval;
			}
		}
	}

	private IEnumerator ChangeRotationSign()
	{
		while(true)
		{
			float interval = UnityEngine.Random.Range(1f, 10f);
			yield return new WaitForSeconds(interval);
			goRoundTargetSign *= -1;
		}
	}

	private void Fire()
	{
		if(FireEvent != null)
		{
			FireEvent(shooter, cacheTransform);
		}
	}


}
