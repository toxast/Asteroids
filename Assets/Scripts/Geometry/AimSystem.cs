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
	public Vector2 directionDist; 

	Vector2 right = new Vector2(1,0);
	//angle [0, 2*Pi] to rotate from Vector2(1,0) to direction vector 
	public float directionAngleRAD
	{
		get
		{
			return Math2d.AngleRad(ref right, ref directionDist);
		}
	}

	public AimSystem(Vector2 targetPosition, Vector2 targetSpeed, Vector2 selfPosition, float bulletSpeed)
	{
        if (bulletSpeed == Mathf.Infinity)
		{
			canShoot = true;
			directionDist = targetPosition - selfPosition;
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
				
				time = -1;
				if(t1 > 0 && t2 > 0)time = Mathf.Min(t1, t2);
				else if(t1 > 0) time = t1;
				else if(t2 > 0) time = t2;

				canShoot = time > 0 && !float.IsInfinity(time);
				if(canShoot)
				{
					directionDist = targetSpeed + dist/time;
				} 
			}
		}

        if (!canShoot) {
            directionDist = targetPosition - selfPosition;
            time = directionDist.magnitude / bulletSpeed;
        }
	}
}


public class SuicideAim
{
	//shot direction, valid only if canShoot is true
	public Vector2 direction{get; private set;} 

	bool fullDataCalculated = false;

	//if target reachable
	bool _canShoot;
	public bool canShoot{
		get
		{ 
			if (!fullDataCalculated) {
				CalculateFullData ();
			}
			return _canShoot;
		} 
		private set{ _canShoot = value;}
	}

	//time utill collision, valid only if canShoot is true
	float _time;
	public float time{
		get
		{ 
			if (!fullDataCalculated) {
				CalculateFullData ();
			}
			return _time;
		} 
		private set{ _time = value;}
	}

	float _angleBetweenCurrentAndBestSpeed;
	public float angleBetweenCurrentAndBestSpeed{
		get
		{ 
			if (!fullDataCalculated) {
				CalculateFullData ();
			}
			return _angleBetweenCurrentAndBestSpeed;
		} 
		private set{ _angleBetweenCurrentAndBestSpeed = value;}
	}

	Vector2 selfSpeed;
	float selfSpeedMagnitude;
	float aimTime;
//	float turnSpeed;

	public SuicideAim(PolygonGameObject target, SpaceShip thisShip, float accuracy) : 
	this(target.position, target.velocity, thisShip.position, thisShip.velocity, thisShip.turnSpeed, accuracy)
	{}
	
	public SuicideAim(Vector2 targetPosition, Vector2 targetSpeed, Vector2 selfPosition, Vector2 selfSpeed, float turnSpeed, float accuracy) {
		this.selfSpeed = selfSpeed;
//		this.turnSpeed = turnSpeed;
		var aimVelocity = accuracy * targetSpeed;
		selfSpeedMagnitude = selfSpeed.magnitude;
		AimSystem aim = new AimSystem(targetPosition, aimVelocity, selfPosition, selfSpeedMagnitude);
		aimTime = aim.time;
		//transform perfect speed accordingly to current speed
		Vector2 perfectSpeedDir = aim.directionDist.normalized;
		Vector2 rightPerfect = Math2d.MakeRight(perfectSpeedDir);
		var vProj = Vector2.Dot(rightPerfect, selfSpeed);
		direction = perfectSpeedDir * selfSpeedMagnitude - vProj * rightPerfect;
	}

	void CalculateFullData() {
		var angleBetweenCurrentAndBestSpeed = Math2d.ClosestAngleBetweenNormalizedRad(selfSpeed/selfSpeedMagnitude, direction.normalized) * Mathf.Rad2Deg;
		if(angleBetweenCurrentAndBestSpeed < 50f) {
			time = aimTime; //+ angleBetweenCurrentAndBestSpeed / turnSpeed;
			canShoot = true;
		} else {
			canShoot = false;
		}
	}
}