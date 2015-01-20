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

	ParticleSystem thrustPSystem;
	float defaultThrustLifetime;

	Joystick joystickControl;
	FireButton fireButton;
	FireButton accelerateButton;
    
	bool acceleratedThisTick = false;

	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}
	
	public void SetJoystick(Image pJoystick)
	{
		joystickControl = gameObject.AddComponent<Joystick> ();
		joystickControl.Set (pJoystick, maxOffset);
	}
	
	public void SetThruster(ParticleSystem p)
	{
		thrustPSystem = p;
		thrustPSystem.gameObject.transform.parent = cacheTransform;
		defaultThrustLifetime = thrustPSystem.startLifetime;
		thrustPSystem.startLifetime = defaultThrustLifetime / 3f;
	}

	public void SetTabletControls(FireButton fireButton, FireButton accelerateButton)
	{
		this.fireButton = fireButton;
		this.accelerateButton = accelerateButton;
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
		velocity += cacheTransform.right * delta * thrust;
		acceleratedThisTick = true;
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
		acceleratedThisTick = false;

#if UNITY_STANDALONE
		KeyboardControlTick (delta);

		if(Input.GetKey(KeyCode.Space))
		{
			Shoot();
		}
#else
		if(fireButton.pressed)
			Shoot ();
		
		if(accelerateButton.pressed)
			Accelerate (delta);

        Joystick3 (delta);
#endif

		TickShooters (delta);

		cacheTransform.position += velocity*delta;

		if(rotation != 0)
			cacheTransform.Rotate(Vector3.back, rotation*delta);


		if(thrustPSystem != null)
		{
			//Or speed and rate by 3;
			//Or another trust
			//And change colr a bit??
			float dthrust = defaultThrustLifetime * delta * 0.5f;
			dthrust = (acceleratedThisTick)? dthrust : -dthrust;
			thrustPSystem.startLifetime = Mathf.Clamp(thrustPSystem.startLifetime + dthrust, defaultThrustLifetime/2f, defaultThrustLifetime);
		}
	}

	private void TickShooters(float delta)
	{
		if(firingSpeedPUpTimeLeft > 0)
		{
			firingSpeedPUpTimeLeft -= delta;
		}
		float kff = (firingSpeedPUpTimeLeft > 0) ? firingSpeedPUpKoeff : 1;
		
		shooters.ForEach(shooter => shooter.Tick(delta*kff));
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
			Accelerate(delta);
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
