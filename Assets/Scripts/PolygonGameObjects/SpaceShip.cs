using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	float turnSpeed = 220f;

	float brake = 15f;
	float thrust = 45f;
	float maxSpeed = 30f;
	float maxSpeedSqr;

	float maxRotation = 250f;
	//float drag = 0.5f;

	//float minOffset = 15f;

	public event System.Action<ShootPlace, Transform> FireEvent;

	private List<ShootPlace> shooters;

	ParticleSystem thrustPSystem;
	float defaultThrustLifetime;

	InputController inputController;

//	Joystick joystickControl;
//	FireButton fireButton;
//	FireButton accelerateButton;
    
	bool acceleratedThisTick = false;

	protected override float healthModifier {
		get {
			return base.healthModifier;
		}
	}

	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}

	public void SetController(InputController iController)
	{
		inputController = iController;
	}

//	public void SetJoystick(Image pJoystick)
//	{
//		joystickControl = gameObject.AddComponent<Joystick> ();
//		joystickControl.Set (pJoystick, maxOffset);
//	}
	
	public void SetThruster(ParticleSystem p)
	{
		thrustPSystem = p;
		thrustPSystem.gameObject.transform.parent = cacheTransform;
		defaultThrustLifetime = thrustPSystem.startLifetime;
		thrustPSystem.startLifetime = defaultThrustLifetime / 3f;
	}

//	public void SetTabletControls(FireButton fireButton, FireButton accelerateButton)
//	{
//		this.fireButton = fireButton;
//		this.accelerateButton = accelerateButton;
//	}

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
		if(rotation != 0)
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
		float deltaV = delta * thrust;
		velocity += cacheTransform.right * deltaV;

		if(velocity.sqrMagnitude > maxSpeedSqr)
		{
			velocity = velocity.normalized*(Mathf.Clamp(velocity.magnitude - deltaV, maxSpeed, Mathf.Infinity));
		}

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

		InputTick (delta);

		//RestictSpeed ();

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

//
//	private void Joystick3(float delta)
//	{
//		var dir = joystickControl.lastDisr;
//		var len = dir.magnitude;
//		if(len > minOffset)
//		{
//			TurnByDirection(dir, delta);
//		}
//		//else if(joystickControl.IsPressing)
//		//{
//		//	Brake(delta, brake);
//		//}
//	}
//

	void InputTick(float delta)
	{
		inputController.Tick (this);

		var dir = inputController.TurnDirection();

		if(dir != Vector2.zero)
		{
			TurnByDirection (dir, delta);
		}

		if(inputController.IsShooting())
		{
			Shoot();
		}

		if(inputController.IsAccelerating())
		{
			Accelerate(delta);
		}
	}

	private void TurnByDirection(Vector3 dir, float delta)
	{
		bool turnLeft = Mathf.Sign(Math2d.Cross2(dir, cacheTransform.right)) < 0;

		if(rotation != 0)
		{
			if(turnLeft)
				TurnLeft(delta);
			else
				TurnRight(delta);
		}
		else
		{
			Rotaitor rotaitor = new Rotaitor(cacheTransform, turnSpeed);
			var currentAimAngle = Math2d.GetRotation(dir) / Math2d.PIdiv180 ;
			rotaitor.Rotate(delta, currentAimAngle);
		}
	}

//
//	void KeyboardControlTick(float delta)
//	{
//		if(Input.GetKey(KeyCode.D))
//		{
//			TurnRight(delta);
//		}
//		else if(Input.GetKey(KeyCode.A))
//		{
//			TurnLeft(delta);
//		}
//		
//		if(Input.GetKey(KeyCode.W))
//		{
//			Accelerate(delta);
//		}
//		else if(Input.GetKey(KeyCode.S))
//		{
//			Brake(delta, brake);
//		}
//
//		if(Input.GetKey(KeyCode.Space))
//		{
//			Shoot();
//		}
//	}

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
