using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	float turnSpeed = 120f;

	float brake = 15f;
	float thrust = 15f;
	float maxSpeed = 30f;
	float maxSpeedSqr;

	float maxRotation = 250f;
	//float drag = 0.5f;

	float minOffset = 15f;
	float maxOffset = 80f;
	Vector2 joystickPos = Vector2.zero;

	public event System.Action<ShootPlace, Transform> FireEvent;

	private List<ShootPlace> shooters;

	Joystick joystickControl;
	//Image joystick;
	public void SetJoystick(Image pJoystick)
	{
	//	joystick = pJoystick;
		joystickControl = gameObject.AddComponent<Joystick> ();
		joystickControl.Set (pJoystick, maxOffset);
	}

	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}

	protected override float healthModifier {
		get {
			return base.healthModifier;
		}
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

	public void Accelerate(float delta)
	{
		Accelerate (delta, thrust);
	}

	private void Accelerate(float delta, float pThrust)
	{
		velocity += cacheTransform.right * delta * pThrust;
		//RestictSpeed();
	}

	private void Brake(float delta, float pBrake)
	{
		var newMagnitude = velocity.magnitude - delta * pBrake; 
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
		Joystick3 (delta);

		KeyboardControlTick (delta);

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



	private void Joystick1(float delta)
	{
		var len = joystickControl.lastDisr.magnitude;
		if(len > minOffset)
		{
			//turn
			bool turnLeft = Mathf.Sign(Math2d.Cross2(joystickControl.lastDisr, cacheTransform.right)) < 0;
			if(turnLeft)
				TurnLeft(delta);
			else
				TurnRight(delta);

			//accelerate
			float pThrust = thrust * (Mathf.Clamp(len, minOffset, maxOffset) - minOffset) / (maxOffset - minOffset);
			Accelerate(delta, pThrust);
		}
		else if(joystickControl.IsPressing)
		{
			Brake(delta, brake);
		}
	}


	private void Joystick2(float delta)
	{
		var dir = joystickControl.lastDisr;
		var len = dir.magnitude;
		if(Mathf.Abs(dir.x) > minOffset)
		{
			//turn
			float turn = Mathf.Sign(dir.x) * ((Mathf.Clamp(Mathf.Abs(dir.x), minOffset, maxOffset) - minOffset) / (maxOffset - minOffset));
			TurnRight(delta * turn);
		}

		if(Mathf.Abs(dir.y) > minOffset)
		{
			if(dir.y > 0)
			{
				//accelerate
				float pThrust = thrust * (Mathf.Clamp(dir.y , minOffset, maxOffset) - minOffset) / (maxOffset - minOffset);
				Accelerate(delta, pThrust);
			}
			else
			{
				//brake
				float pBrake = brake * (Mathf.Clamp(-dir.y , minOffset, maxOffset) - minOffset) / (maxOffset - minOffset);
				Brake(delta, pBrake);
			}
		}
		else if(joystickControl.IsPressing)
		{
			Brake(delta, brake);
		}
	}


	private void Joystick3(float delta)
	{
		var len = joystickControl.lastDisr.magnitude;
		if(len > minOffset)
		{
			//turn
			bool turnLeft = Mathf.Sign(Math2d.Cross2(joystickControl.lastDisr, cacheTransform.right)) < 0;
			if(turnLeft)
				TurnLeft(delta);
			else
				TurnRight(delta);
		}
		//else if(joystickControl.IsPressing)
		//{
		//	Brake(delta, brake);
		//}
	}


	void KeyboardControlTick(float delta)
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
			Accelerate(delta, thrust);
		}
		else if(Input.GetKey(KeyCode.S))
		{
			Brake(delta, brake);
		}
	}


	public void Shoot()
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
