using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour {

	public Polygon polygon;
	public Transform cacheTransform;


	public event System.Action FireEvent;

	Vector3 speed;

	float turnSpeed = 330f;

	float brake = 4f;
	float thrust = 20f;
	float maxSpeed = 40f;
	float maxSpeedSqr;
	//float drag = 0.5f;

	float fireInterfal = 0.3f;
	float timeToNextShot = 0f;

	void Awake()
	{
		cacheTransform = transform;
	}

	public void Init(Polygon polygon)
	{
		this.polygon = polygon;

		maxSpeedSqr = maxSpeed*maxSpeed;
		speed = Vector3.zero;
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
	
	public void Tick(float delta)
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


		if (timeToNextShot > 0) 
		{
			timeToNextShot -= delta;
		}

		if(Input.GetKey(KeyCode.Space) && (timeToNextShot <= 0))
		{
			timeToNextShot = fireInterfal;
			Fire();
		}


		cacheTransform.position += speed*delta;
	}


 	private void Fire()
	{
		if(FireEvent != null)
		{
			FireEvent();
		}
	}
}
