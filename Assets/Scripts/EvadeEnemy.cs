using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvadeEnemy : PolygonGameObject
{
	private class DangerBullet
	{
		public Bullet bullet;
		public Vector3 relativePos;
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

	float speed = 10f;
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
				float cos = Math2d.Cos(-pos, bullet.GetSpeed());

				if(pos.sqrMagnitude > 1500f ||  cos < 0f || (pos.sqrMagnitude > 700f && cos < dangerAngle))
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

				//TODO: delete
				/*if(intersections.Count > 2)
				{
					string intersectionsSrt = string.Empty;
					foreach(var isc in intersections)
					{
						intersectionsSrt += " " + isc;
					}
					Debug.Log(intersectionsSrt);
				}*/

				int indxZero = -1;
				for (int i = 0; i < intersections.Count; i++) 
				{
					if(intersections[i] > 0)
					{
						indxZero = i;
						break;
					}
				}

				float delta = 0.1f;
				float requiredSpace = polygon.R*2 + bullet.polygon.R*2;
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
									//Debug.LogWarning("1 right - halfRequiredSpace");
									savePoint = right - halfRequiredSpace - delta;
								}
								else
								{
									//Debug.LogWarning("2 left + halfRequiredSpace");
									savePoint = left + halfRequiredSpace + delta;
								}
								break;
							}
						}
						else if(right <= 0)
						{
							//Debug.LogWarning("3 right - halfRequiredSpace");
							savePoint = right - halfRequiredSpace - delta;
						}
						else if(left >= 0)
						{
							//Debug.LogWarning("4 left - halfRequiredSpace");
							savePoint = left + halfRequiredSpace + delta;
						}
						else
						{
							Debug.LogError("wtf");
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

				Vector2 safe2d = Math2d.RotateVertex(new Vector2(savePoint, 0), cosA, -sinA); //-a
				safePoint = cacheTransform.position + (Vector3)safe2d;
				//cacheTransform.position = cacheTransform.position + (Vector3)safe2d;
				//Debug.DrawLine(cacheTransform.position, cacheTransform.position + (Vector3)safe2d);
			}
			 

			float interval = UnityEngine.Random.Range(0.1f, 0.3f);
			yield return new WaitForSeconds(interval); //WaitForSeconds(interval);
		}
	}


}
