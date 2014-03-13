using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject
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

	private List<Bullet> bullets;

	private float dangerAngle = Mathf.Sqrt(2)/2f; //30 degrees

	public void SetBulletsList(List<Bullet> bullets)
	{
		this.bullets = bullets;
	}

	private Vector3 safePoint;

	void Start()
	{
		safePoint = cacheTransform.position;
		StartCoroutine(Evade());
	}

	float speed = 5;
	void Update()
	{
		float delta = speed * Time.deltaTime;
		if((cacheTransform.position - safePoint).sqrMagnitude < delta*delta)
		{
			cacheTransform.position = safePoint;
		}
		else
		{
			cacheTransform.position += (safePoint - cacheTransform.position).normalized*delta;
		}
	}

	IEnumerator Evade()
	{
		while(true)
		{
			List<Bullet> dangerBullets = new List<Bullet>();
			for (int i = 0; i < bullets.Count; i++) 
			{
				Bullet bullet = bullets[i];
				if(bullet == null)
				{
					continue;
				}

				//todo: cache, use in closest
				Vector2 pos = bullet.cacheTransform.position - cacheTransform.position; 
				if(pos.sqrMagnitude > 1500 ||  (pos.sqrMagnitude > 700 && Math2d.Cos(-pos, bullet.GetSpeed()) < dangerAngle))
				{
					continue;
				}

				//bullet.SetColor(Color.blue);
				dangerBullets.Add(bullet);
			}

			//pick closest bullet
			float minDist = float.MaxValue;
			int minIndx = -1;
			for (int i = 0; i < dangerBullets.Count; i++) 
			{
				Bullet bullet = dangerBullets[i];
				Vector2 dist = bullet.cacheTransform.position - cacheTransform.position; 
				if(dist.sqrMagnitude < minDist)
				{
					minDist = dist.sqrMagnitude;
					minIndx = i;
				}
			}

			if(minIndx >= 0)
			{
				Bullet bullet = dangerBullets[minIndx];
				//rotate everything so perpendecular to bullet speed line will be y=0
				float cosA = Math2d.Cos(new Vector2(0f, -1f), bullet.GetSpeed());
				float sinA = Math2d.Cos(new Vector2(-1f, 0f), bullet.GetSpeed());

				List<float> intersections = new List<float>();
				for (int i = 0; i < dangerBullets.Count; i++) 
				{
					Bullet dbullet = dangerBullets[i];
					Vector2 dist = dbullet.cacheTransform.position - cacheTransform.position; 
					Vector2 posRotated = Math2d.RotateVertex(dist, cosA, sinA);
					Vector2 speedRotated = Math2d.RotateVertex(dbullet.GetSpeed(), cosA, sinA);

					float x;
					if(speedRotated.y == 0f)
					{
						x = posRotated.x;
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


				float requiredSpace = polygon.R*2 + bullet.polygon.R*2;
				float halfRequiredSpace = requiredSpace/2;
				float savePoint = 0;

				int curIndex = indxZero;
				int min = 2;
				int max = 1;
				while(true)
				{
					if(curIndex-1 < 0)
					{
						curIndex ++;
						break;
					}
					float left = intersections[curIndex-1];
					float right = intersections[curIndex];
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
								if(right < -left)
								{
									savePoint = right - halfRequiredSpace;
								}
								else
								{
									savePoint = left + halfRequiredSpace;
								}
								break;
							}
						}
						else if(right < 0)
						{
							savePoint = right - halfRequiredSpace;
						}
						else //left > 0
						{
							savePoint = left + halfRequiredSpace;
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
							max++;
							curIndex = max;
						}
					}
				}

				//turn save point

				Vector2 safe2d = Math2d.RotateVertex(new Vector2(savePoint, 0), cosA, -sinA); //-a
				safePoint = cacheTransform.position + (Vector3)safe2d;
				//Debug.DrawLine(cacheTransform.position, cacheTransform.position + (Vector3)safe2d);
			}
			 

			float interval = UnityEngine.Random.Range(0.2f, 0.3f);
			yield return new WaitForSeconds(interval);
		}
	}


}
