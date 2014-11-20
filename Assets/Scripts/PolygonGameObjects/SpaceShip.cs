using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	float turnSpeed = 200f;

	float brake = 10f;
	float thrust = 20f;
	float maxSpeed = 40f;
	float maxSpeedSqr;

	float maxRotation = 250f;
	//float drag = 0.5f;


	public event System.Action<ShootPlace, Transform> FireEvent;

	private List<ShootPlace> shooters;


	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}

	public void SetShootPlaces(List<ShootPlace> shooters)
	{
		this.shooters = shooters;
	}

	private void TurnRight(float delta)
	{	
		float dRotation = turnSpeed * delta;
		Turn (dRotation);
	}

	private void TurnLeft(float delta)
	{
		float dRotation = -turnSpeed * delta;
		Turn (dRotation);
	}

	private void Turn(float dRotation)
	{
		if(Mathf.Abs(rotation) > 0)
		{
			var dsign = Mathf.Sign (dRotation);
			if(dsign != Mathf.Sign(rotation))
			{
				rotation += dRotation;
				if(dsign == Mathf.Sign(rotation))
				{
					rotation = 0;
				}
			}
			else
			{
				rotation -= dRotation;
				if(dsign != Mathf.Sign(rotation))
				{
					rotation = 0;
				}
				cacheTransform.Rotate(Vector3.back, dRotation);
			}
		}
		else
		{
			cacheTransform.Rotate(Vector3.back, dRotation);
		}
	}

	private void Accelerate(float delta)
	{
		velocity += cacheTransform.right * delta * thrust;
		//RestictSpeed();
	}

	private void Brake(float delta)
	{
		var newMagnitude = velocity.magnitude - delta * brake; 
		if (newMagnitude < 0)
			newMagnitude = 0;

		velocity = velocity.normalized * newMagnitude;

		if(rotation != 0)
		{
			if(rotation > 0)  TurnLeft(delta);
			else TurnRight(delta);
		}
		//RestictSpeed();
	}

	private void RestictSpeed()
	{
		if(velocity.sqrMagnitude > maxSpeedSqr)
		{
			velocity = velocity.normalized * maxSpeed;
		}
	}

	public override void Tick(float delta)
	{
		if(Input.GetKey(KeyCode.D))
		{
			TurnRight(delta);
		}
		else if(Input.GetKey(KeyCode.A))
		{
			TurnLeft(delta);
		}
		else
		{
//			var d = delta * rotationBreak;
//			var r_abs = Mathf.Abs(rotation);
//			var r_sign = Mathf.Sign(rotation);
//			if(r_abs < d)
//			{
//				r_abs = 0;
//			}
//			else
//			{
//				rotation = (r_abs - d) * r_sign;
//			}
		}


		if(Input.GetKey(KeyCode.W))
		{
			Accelerate(delta);
		}
		else if(Input.GetKey(KeyCode.S))
		{
			Brake(delta);
		}

		if(firingSpeedPUpTimeLeft > 0)
		{
			firingSpeedPUpTimeLeft -= delta;
		}
		float kff = (firingSpeedPUpTimeLeft > 0) ? firingSpeedPUpKoeff : 1;

		shooters.ForEach(shooter => shooter.Tick(delta*kff));

		if(Input.GetKey(KeyCode.Space))
		{
			Shoot();
		}

		cacheTransform.position += velocity*delta;

		if(rotation != 0)
			cacheTransform.Rotate(Vector3.back, rotation*delta);
	}

	private void Shoot()
	{
		for (int i = 0; i < shooters.Count; i++) 
		{
			if(shooters[i].ShootIfReady())
			{
				if(FireEvent != null)
				{
					FireEvent(shooters[i], cacheTransform);
				}
			}
		}
	}


	private float firingSpeedPUpKoeff = 1f;
	private float firingSpeedPUpTimeLeft = 0f;
	public void ChangeFiringSpeed(float koeff, float duration)
	{
		firingSpeedPUpKoeff = koeff;
		firingSpeedPUpTimeLeft = duration;
	}
 	
}
