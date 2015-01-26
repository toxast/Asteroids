using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject, IGotTarget
{
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

	public event System.Action<ShootPlace, Transform> FireEvent;

	private float movingSpeed = 7f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;
	private float rangeAngle = 15f; //if angle to target bigger than this - dont even try to shoot
	private float cannonsRotatingSpeed = 55f;

	private bool avoiding = false;
	private Vector3 currentSafePoint;
	private float currentAimAngle = 0;
	private int goRoundTargetSign = 1;

	private List<BulletBase> incomingBullets;
	private PolygonGameObject target;
	Rotaitor cannonsRotaitor;
	ShootPlace shooter;

	public void Init(List<BulletBase> incomingBullets, ShootPlace shooter)
	{
		this.shooter = shooter;
		this.incomingBullets = incomingBullets;

		cannonsRotaitor = new Rotaitor(cacheTransform, cannonsRotatingSpeed);

		StartCoroutine(Evade());

		StartCoroutine(Aim());

		StartCoroutine(FireCoroutine());

		StartCoroutine(ChangeRotationSign());
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);

		if (target == null)
			return;

		float deltaDist = movingSpeed * delta;

		if(avoiding)
		{
			MoveToSafePoint(deltaDist);
		}
		else
		{
			KeepTargetDistance(deltaDist);
			RotateAroundTarget(deltaDist);
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
	}

	private void RotateAroundTarget(float deltaDist)
	{
		Vector2 dist = target.cacheTransform.position - cacheTransform.position;
		Vector2 rotateDirection = new Vector2(dist.y, -dist.x).normalized; //right
		cacheTransform.position += (Vector3) rotateDirection * deltaDist * goRoundTargetSign;
	}

	private void RotateCannon(float deltaTime)
	{
		cannonsRotaitor.Rotate(deltaTime, currentAimAngle);
 	}

	IEnumerator Evade()
	{
		while(true)
		{
			EvadeSystem evade = new EvadeSystem(incomingBullets, this);
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
			if(target != null)
			{
				AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, shooter.speed);
				if(aim.canShoot)
				{
					currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
				}
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private IEnumerator FireCoroutine()
	{
		float defaultInterval = shooter.fireInterval;
		float shortInterval = defaultInterval/2f;

		float deltaTime = defaultInterval;
		while(true)
		{
			yield return new WaitForSeconds(deltaTime);

			if(target != null)
			{
				if(Mathf.Abs(cannonsRotaitor.DeltaAngle(currentAimAngle)) < rangeAngle)
				{
					Fire();
					deltaTime = defaultInterval;
				}
				else
				{
					deltaTime = shortInterval;
				}
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
