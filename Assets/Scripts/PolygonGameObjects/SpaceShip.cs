using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	public Vector3 speed;

	float turnSpeed = 200f;

	float brake = 4f;
	float thrust = 20f;
	float maxSpeed = 40f;
	float maxSpeedSqr;
	//float drag = 0.5f;


	public event System.Action<ShootPlace, Transform> FireEvent;

	private List<ShootPlace> shooters;


	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		speed = Vector3.zero;
	}

	public void SetShootPlaces(List<ShootPlace> shooters)
	{
		this.shooters = shooters;
	}

	private void TurnRight(float delta)
	{
		cacheTransform.Rotate(Vector3.back, turnSpeed*delta);
	}

	private void TurnLeft(float delta)
	{
		cacheTransform.Rotate(Vector3.back, -turnSpeed*delta);
	}

	private void Accelerate(float delta)
	{
		speed += cacheTransform.right * delta * thrust;
		RestictSpeed();
	}

	private void MoveBack(float delta)
	{
		speed = speed - cacheTransform.right * delta * brake;
		RestictSpeed();
	}

	private void RestictSpeed()
	{
		if(speed.sqrMagnitude > maxSpeedSqr)
		{
			speed = speed.normalized * maxSpeed;
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

		if(Input.GetKey(KeyCode.W))
		{
			Accelerate(delta);
		}
		else if(Input.GetKey(KeyCode.S))
		{
			MoveBack(delta);
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

		cacheTransform.position += speed*delta;
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
