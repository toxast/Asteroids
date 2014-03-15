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


	private float dangerAngle = Mathf.Sqrt(2)/2f; //45 degrees
	private float speed = 10f;
	private int closestBulletsToAvoid = 3; 
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
			//TODO: vector2
			if((cacheTransform.position - safePoint).sqrMagnitude < delta*delta)
			{
				cacheTransform.position = safePoint;
				avoiding = false;
			}
			else
			{
				cacheTransform.position += (safePoint - cacheTransform.position).normalized*delta;
			}
		}
		//else
		//{
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
		//}
	}

	IEnumerator Evade()
	{
		while(true)
		{
			List<DangerBullet> dangerBullets = new List<DangerBullet>();
			for (int i = 0; i < bullets.Count; i++) 
			{
				Bullet bullet = bullets[i];
				if(bullet == null)
				{
					continue;
				}

				Vector2 pos = bullet.cacheTransform.position - cacheTransform.position; 
				float cos = Math2d.Cos(-pos, bullet.GetSpeed());
				DangerBullet b = new DangerBullet(bullet, pos);

				if(b.sqrMagnitude > 1500f ||  cos < 0f || (b.sqrMagnitude > 700f && cos < dangerAngle))
				{
					continue;
				}
				//bullet.SetColor(Color.blue);

				dangerBullets.Add(b);
			}

			avoiding = (dangerBullets.Count > 0);

			if(dangerBullets.Count > 0)
			{
				dangerBullets.Sort();
				int range = Math.Min(closestBulletsToAvoid, dangerBullets.Count);
				dangerBullets = dangerBullets.GetRange(0, range);

				DangerBullet blt  = dangerBullets[0];
				//Rotate everything. So perpendecular to bullet speed line will be y=0
				float cosA = Math2d.Cos(new Vector2(0f, -1f), blt.bullet.GetSpeed());
				float sinA = Math2d.Cos(new Vector2(-1f, 0f), blt.bullet.GetSpeed());

				List<float> intersections = new List<float>();
				for (int i = 0; i < dangerBullets.Count; i++) 
				{
					DangerBullet dbullet = dangerBullets[i];
					Vector2 posRotated = Math2d.RotateVertex(dbullet.dist, cosA, sinA);
					Vector2 speedRotated = Math2d.RotateVertex(dbullet.bullet.GetSpeed(), cosA, sinA);

					float x;
					if(speedRotated.y == 0f)
					{
						x = float.MaxValue/3f;
					}
					else
					{
						x = (speedRotated.x / speedRotated.y) * posRotated.y + posRotated.x;
					}
					intersections.Add(x);
				}
				intersections.Add(float.MaxValue/3f);
				intersections.Add(float.MinValue/3f);

				//find closest interval size of R
				intersections.Sort();
				int indxZero = -1;
				for (int i = 0; i < intersections.Count; i++) 
				{
					if(intersections[i] > 0)
					{
						indxZero = i;
						break;
					}
				}

				float delta = 0.1f; //to avoid calculation rounds
				float requiredSpace = polygon.R*2 + blt.bullet.polygon.R*2;
				float halfRequiredSpace = requiredSpace/2f;
				float savePoint = 0;

				int curIndex = indxZero-1;
				int min = curIndex;
				int max = curIndex+1;
				while(true)
				{
					float left = intersections[curIndex];
					float right = intersections[curIndex + 1];
					if(right-left > requiredSpace)
					{
						if(left < 0 && right > 0)
						{
							if(left < -halfRequiredSpace && right > halfRequiredSpace)
							{
								//it is save here
								break;
							}
							else
							{

								if( -left > right)
								{
									savePoint = right - halfRequiredSpace - delta;
								}
								else
								{
									savePoint = left + halfRequiredSpace + delta;
								}
								break;
							}
						}
						else if(right <= 0)
						{
							savePoint = right - halfRequiredSpace - delta;
						}
						else if(left >= 0)
						{
							savePoint = left + halfRequiredSpace + delta;
						}
						break;
					}
					else
					{
						float absMin = Mathf.Abs(intersections[min]);
						float absMax = Mathf.Abs(intersections[max]);
						if(absMin < absMax)
						{
							min--;
							curIndex = min;
						}
						else
						{

							curIndex = max;
							max++;
						}
					}
				}

				//turn save point
				Vector2 safe2d = Math2d.RotateVertex(new Vector2(savePoint, 0), cosA, -sinA);
				safePoint = cacheTransform.position + (Vector3)safe2d;
			}

			yield return new WaitForSeconds(0.1f); 
		}
	}

	private IEnumerator Aim()
	{
		Vector2 fire = new Vector2();
		float bulletSpeed = 30f;

		while(true)
		{
			Vector2 targetPos = target.cacheTransform.position;
			Vector2 targetVelocity = target.speed;

			Vector2 pos = cacheTransform.position;

			Vector2 dist = targetPos - pos;

			float A = targetVelocity.sqrMagnitude - bulletSpeed*bulletSpeed;
			float B = targetVelocity.x * dist.x  +  targetVelocity.y * dist.y;
			float C = dist.sqrMagnitude;

			float D = B*B - 4*A*C;
			if(D > 0)
			{
				float Dsqrt = Mathf.Sqrt(D);
				float t1 = (-B + Dsqrt)/(2*A);
				float t2 = (-B - Dsqrt)/(2*A);

				float t = -1;
				if(t1 > 0) t = t1;
				else if(t2 > 0) t = t2;

				if(t > 0)
				{
					fire.x = targetVelocity.x + dist.x/t;
					fire.y = targetVelocity.y + dist.y/t;

					Vector2 right = new Vector2(1,0);
					float angle = (float)(180f/Math.PI) * Mathf.Acos(Math2d.Cos(ref fire, ref right)) * Mathf.Sign(Math2d.Rotate(ref fire, ref right));

					transform.rotation = Quaternion.Euler(new Vector3(0,0, -angle));

					Fire();
				}
				else
				{
					Debug.LogError("t1 = " + t1 + " t2 = " + t2 );
				}
			}
			else
			{
				//out of range
			}

			yield return new WaitForSeconds(0.5f);
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
