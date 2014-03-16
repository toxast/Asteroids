using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EvadeSystem
{
	/*private struct DangerBullet : IComparable<DangerBullet>
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

	//settings
	private float dangerAngle = Mathf.Sqrt(2f)/2f; //45
	private int closestBulletsToAvoid = 3;

	//temp
	Vector2 victimPos;

	private List<DangerBullet> SelectBulletsToEvade(List<Bullet> bullets)
	{
		List<DangerBullet> dangerBullets = new List<DangerBullet>();
		for (int i = 0; i < bullets.Count; i++) 
		{
			Bullet bullet = bullets[i];
			if(bullet == null) continue;
			
			Vector2 pos = bullet.cacheTransform.position - victimPos; 
			float cos = Math2d.Cos(-pos, bullet.GetSpeed());
			DangerBullet b = new DangerBullet(bullet, pos);
			
			if(b.sqrMagnitude > 1500f ||  cos < 0f || (b.sqrMagnitude > 700f && cos < dangerAngle))
			{
				continue;
			}
			
			dangerBullets.Add(b);
		}
		
		//avoiding = (dangerBullets.Count > 0);
		
		if(dangerBullets.Count > 0)
		{
			dangerBullets.Sort();
			int range = Math.Min(closestBulletsToAvoid, dangerBullets.Count);
			dangerBullets = dangerBullets.GetRange(0, range);
		}	

		return dangerBullets;
	}

	private List<float> GetIntersections(List<DangerBullet> dangerBullets)
	{
		List<float> intersections = new List<float>();
		if (dangerBullets.Count == 0)
			return intersections;

		DangerBullet blt  = dangerBullets[0];
		//Rotate everything. So perpendecular to bullet speed line will be y=0
		float cosA = Math2d.Cos(new Vector2(0f, -1f), blt.bullet.GetSpeed());
		float sinA = Math2d.Cos(new Vector2(-1f, 0f), blt.bullet.GetSpeed());
		

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
	}

	public EvadeSystem(List<Bullet> bullets, Vector2 selfPosition)
	{
		this.victimPos = selfPosition;

		List<DangerBullet> dangerBullets = SelectBulletsToEvade(bullets);

		//avoiding = (dangerBullets.Count > 0);
		
		List<float> intersections = GetIntersections (dangerBullets);
		intersections.Add(float.MaxValue/3f);
		intersections.Add(float.MinValue/3f);
		intersections.Sort();

			//find closest interval size of R
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
	}
	*/
}
