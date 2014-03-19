using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject
{
	public event System.Action<EvadeEnemy> FireEvent;

	private struct DangerBullet : IComparable<DangerBullet>
	{
		public Bullet bullet;
		public Vector2 dist;
		public float sqrMagnitude;

		public DangerBullet(Bullet bullet, Vector2 dist)
		{
			this.bullet = bullet;
			this.dist = dist;
			sqrMagnitude = dist.sqrMagnitude;
		}

		public int CompareTo(DangerBullet other)
		{
			return sqrMagnitude.CompareTo(other.sqrMagnitude);
		}
	}

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
		, 2f).ToArray();


	private float movingSpeed = 6f;
	private float rotatingSpeed = 55f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;


	private List<Bullet> bullets;
	private SpaceShip target;

	private bool avoiding = false;
	private Vector3 currentSafePoint;
	private float currentAimAngle = 0;

	//if angle to target bigger than this - dont even try to shoot
	private float rangeAngle = 15f; 

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
			avoiding = false;
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

	private void RotateCannon(float deltaTime)
	{
		float deltaAngle = deltaTime*rotatingSpeed;

		Vector3 currentAngles = cacheTransform.rotation.eulerAngles;

		 
		float diff = currentAimAngle - currentAngles.z;

		if(Mathf.Abs(diff) <= deltaAngle)
		{
			cacheTransform.rotation = Quaternion.Euler(currentAngles.SetZ(currentAimAngle));
		}
		else
		{
			float sign = Mathf.Sign(diff)*Mathf.Sign(180f - Mathf.Abs(diff));
			deltaAngle *= sign;
			cacheTransform.rotation = Quaternion.Euler(currentAngles + new Vector3(0, 0, deltaAngle));
		}
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
		float bulletSpeed = 30f; //TODO bullets param

		while(true)
		{
			AimSystem aim = new AimSystem(target.cacheTransform.position, target.speed, cacheTransform.position, bulletSpeed);
			if(aim.canShoot)
			{
				currentAimAngle = aim.directionAngle / Math2d.PIdiv180;
			}
			yield return new WaitForSeconds(aimInterval);
		}
	}

	private IEnumerator FireCoroutine()
	{
		float defaultInterval = 1.5f;
		float shortInterval = 0.5f;

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

	private void Fire()
	{
		if(FireEvent != null)
		{
			FireEvent(this);
		}
	}


}
