using UnityEngine;
using System.Collections;
using System;

public class AimSystem 
{
	//if target reachable
	public bool canShoot{get; private set;}

	//time the bullet will fly till collision, valid only if canShoot is true
	//public float time; 

	//shot direction, valid only if canShoot is true
	public Vector2 direction; 

	Vector2 right = new Vector2(1,0);
	//angle [0, 2*Pi] to rotate from Vector2(1,0) to direction vector 
	public float directionAngleRAD
	{
		get
		{
			return Math2d.AngleRAD(ref right, ref direction);
		}
	}

	public AimSystem(Vector2 targetPosition, Vector2 targetSpeed, Vector2 selfPosition, float bulletSpeed)
	{
		if(bulletSpeed == Mathf.Infinity)
		{
			canShoot = true;
			direction = targetPosition - selfPosition;
		}
		else
		{
			canShoot = false;
			Vector2 dist = targetPosition - selfPosition;

			//Att + Bt + C = 0;
			float A = targetSpeed.sqrMagnitude - bulletSpeed*bulletSpeed;
			float B = 2*(targetSpeed.x * dist.x  +  targetSpeed.y * dist.y);
			float C = dist.sqrMagnitude;
			
			float D = B*B - 4*A*C;
			if(D > 0)
			{
				float Dsqrt = Mathf.Sqrt(D);
				float t1 = (-B + Dsqrt)/(2*A);
				float t2 = (-B - Dsqrt)/(2*A);
				
				float t = -1;
				if(t1 > 0 && t2 > 0) t = Mathf.Min(t1, t2);
				else if(t1 > 0) t = t1;
				else if(t2 > 0) t = t2;

				canShoot = t > 0;
				if(canShoot)
				{
					direction = targetSpeed + dist/t;
				}
			}
		}
	}
}
