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
		, 1f).ToArray();


	private float speed = 10f;
	private float minDistanceToTargetSqr = 600;
	private float maxDistanceToTargetSqr = 800;


	private List<Bullet> bullets;
	private SpaceShip target;

	private bool avoiding = false;
	private Vector3 safePoint;

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
	}

	void Update()
	{
		float delta = speed * Time.deltaTime;

		if(avoiding)
		{
			Vector2 dist = cacheTransform.position - safePoint;
			if(dist.sqrMagnitude < delta*delta)
			{
				cacheTransform.position = safePoint;
				avoiding = false;
			}
			else
			{
				cacheTransform.position += (safePoint - cacheTransform.position).normalized*delta;
			}
		}
		else
		{
			Vector2 dist = target.cacheTransform.position - cacheTransform.position;
			float sqrDist = dist.sqrMagnitude;
			if(sqrDist < minDistanceToTargetSqr)
			{
				cacheTransform.position -= (Vector3) dist.normalized * delta;
			}
			else if (sqrDist > maxDistanceToTargetSqr)
			{
				cacheTransform.position += (Vector3) dist.normalized * delta;
			}
		}
	}

	IEnumerator Evade()
	{
		while(true)
		{
			EvadeSystem evade = new EvadeSystem(bullets, this);
			avoiding = !evade.safeAtCurrentPosition;
			safePoint = evade.safePosition;
			yield return new WaitForSeconds(0.1f); 
		}
	}

	private IEnumerator Aim()
	{
		float shotInterval = 1.5f;
		float bulletSpeed = 30f;

		while(true)
		{
			AimSystem aim = new AimSystem(target.cacheTransform.position, target.speed, cacheTransform.position, bulletSpeed);
			if(aim.canShoot)
			{
				float angle = aim.directionAngle * (180f/Mathf.PI);
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
				Fire();
			}
			yield return new WaitForSeconds(shotInterval);
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
