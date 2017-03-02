using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class EvadeSystem
{
	private struct DangerBullet : IComparable<DangerBullet>
	{
		public PolygonGameObject bullet;
		public Vector2 dist;
		public float sqrMagnitude;
		
		public DangerBullet(PolygonGameObject bullet, Vector2 dist)
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
	private int closestBulletsToAvoid = 30;
	private float bulletDetectionRange = 40f;
	private float bulletDetectionRangeSqr;
	private float angleConsiderRange = 27f;
	private float angleConsiderRangeSqr;


	//results
	public bool safeAtCurrentPosition;
	public Vector2 safePosition;

	public EvadeSystem(List<PolygonGameObject> bullets, PolygonGameObject victim)
	{
		bulletDetectionRangeSqr = bulletDetectionRange*bulletDetectionRange;
		angleConsiderRangeSqr = angleConsiderRange*angleConsiderRange;

		List<DangerBullet> dangerBullets = SelectBulletsToEvade(bullets, victim);
		
		safeAtCurrentPosition = (!dangerBullets.Any());
		
		List<float> intersections = new List<float>();
		if(!safeAtCurrentPosition)
		{
			DangerBullet blt  = dangerBullets[0];
			//Rotate everything. So perpendecular to bullet speed line will be y=0
			float cosA = Math2d.Cos(new Vector2(0f, -1f), blt.bullet.velocity);
			float sinA = Math2d.Cos(new Vector2(-1f, 0f), blt.bullet.velocity);

			intersections = RotateAndGetIntersections (dangerBullets, cosA, sinA);

			float requiredSpace = victim.polygon.R*2 + blt.bullet.polygon.R*2;
			float safePoint1d = FindSpaceInIntervals(intersections, requiredSpace);
			//rotate back
			Vector2 safe2d = Math2d.RotateVertex(new Vector2(safePoint1d, 0), cosA, -sinA);
			safePosition = victim.position + safe2d;
		}
		else
		{
			safePosition = victim.position;
		}
	}

	//returns selected bullets sorted by distance in acending order
	private List<DangerBullet> SelectBulletsToEvade(List<PolygonGameObject> bullets, PolygonGameObject victim)
	{
		var victimPos = victim.position;
		List<DangerBullet> dangerBullets = new List<DangerBullet>();
		for (int i = 0; i < bullets.Count; i++) 
		{
			var bullet = bullets[i];
			if(bullet == null) continue;

			if((bullet.collisions & victim.layerCollision) == 0)
				continue;

			Vector2 pos = (Vector2)bullet.position - victimPos; 
			float cos = Math2d.Cos(-pos, bullet.velocity);
			DangerBullet b = new DangerBullet(bullet, pos);

			bool bulletOutOfRange = b.sqrMagnitude > bulletDetectionRangeSqr;
			if(bulletOutOfRange) continue;

			bool bulletGoesAway = cos < 0f;
			if(bulletGoesAway) continue;

			bool bigAngleAndFar = b.sqrMagnitude > angleConsiderRangeSqr && cos < dangerAngle;
			if(bigAngleAndFar) continue;
			
			dangerBullets.Add(b);
		}
		
		if(dangerBullets.Count > 0)
		{
			dangerBullets.Sort();


			int range = Math.Min(closestBulletsToAvoid, dangerBullets.Count);
			dangerBullets = dangerBullets.GetRange(0, range);
		}	

		return dangerBullets;
	}

	//rotate bullets by angle A and intersect with line y = 0;
	//returns x coordinates of intersections
	private List<float> RotateAndGetIntersections(List<DangerBullet> dangerBullets, float cosA, float sinA)
	{
		List<float> intersections = new List<float>();

		for (int i = 0; i < dangerBullets.Count; i++) 
		{
			DangerBullet dbullet = dangerBullets[i];
			Vector2 posRotated = Math2d.RotateVertex(dbullet.dist, cosA, sinA);
			Vector2 speedRotated = Math2d.RotateVertex(dbullet.bullet.velocity, cosA, sinA);
			
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

		return intersections;
	}

	//find closest to (0,0) interval size of victimR*2 + bulletsR*2
	private float FindSpaceInIntervals(List<float> intersections, float space)
	{
		intersections.Add(float.MaxValue/3f);
		intersections.Add(float.MinValue/3f);
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
		float halfRequiredSpace = space/2f;
		float requiredSpace = space;
		float safePoint = 0;
		
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
							safePoint = right - halfRequiredSpace - delta;
						}
						else
						{
							safePoint = left + halfRequiredSpace + delta;
						}
						break;
					}
				}
				else if(right <= 0)
				{
					safePoint = right - halfRequiredSpace - delta;
				}
				else if(left >= 0)
				{
					safePoint = left + halfRequiredSpace + delta;
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

		return safePoint;
	}


	
}
