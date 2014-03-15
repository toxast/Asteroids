using UnityEngine;
using System.Collections;
using System;

public class AimSystem 
{
	//if target reachable
	public bool canShoot{get; private set;}

	//time the bullet will fly till collision, valid only if canShoot is true
	public float time; 

	//shot direction, valid only if canShoot is true
	public Vector2 direction; 

	Vector2 right = new Vector2(1,0);
	//angle to rotate from Vector2(1,0) to direction vector
	public float directionAngle
	{
		get
		{
			float sign = Mathf.Sign(Math2d.Rotate(ref right, ref direction));
			return sign * Mathf.Acos(Math2d.Cos(ref direction, ref right));
		}
	}

	public AimSystem(Vector2 targetPosition, Vector2 targetSpeed, Vector2 selfPosition, float bulletSpeed)
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
				direction = new Vector2();
				direction.x = targetSpeed.x + dist.x/t;
				direction.y = targetSpeed.y + dist.y/t;
			}
		}
	}
}
