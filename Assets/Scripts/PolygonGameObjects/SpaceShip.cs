using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	float turnSpeed = 220f;

	float passiveBrake = 2f;
	float brake = 15f;
	float thrust = 45f;
	float maxSpeed = 20f;
	float maxSpeedSqr;

	ParticleSystem thrustPSystem;
	float defaultThrustLifetime;

	protected InputController inputController;

	bool acceleratedThisTick = false;

	protected override float healthModifier {
		get {
			return base.healthModifier * Singleton<GlobalConfig>.inst.SpaceshipHealthModifier;
		}
	}

	void Awake()
	{
		cacheTransform = transform;
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}

	public void Init(SpaceshipData data)
	{
		turnSpeed = data.turnSpeed;
		passiveBrake = data.passiveBrake;
		thrust = data.thrust;
		maxSpeed = data.maxSpeed;
		maxSpeedSqr = maxSpeed*maxSpeed;
	}

	public void SetController(InputController iController)
	{
		inputController = iController;
	}

	public void SetThruster(ParticleSystem p, Vector2 pos)
	{
		thrustPSystem = p;
		thrustPSystem.gameObject.transform.parent = cacheTransform;
		Vector3 pos3 = pos;
		pos3.z = 1;
		thrustPSystem.gameObject.transform.localPosition = pos3;
		defaultThrustLifetime = thrustPSystem.startLifetime;
		thrustPSystem.startLifetime = defaultThrustLifetime / 3f;
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
			float stability = 0.5f;
			dRotation*= stability;

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
		float k = 0.5f; 

		float deltaV = delta * thrust;
		velocity -= k * velocity.normalized * deltaV;
		velocity += (1 + k)*cacheTransform.right * deltaV;

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
		base.Tick (delta);

		acceleratedThisTick = false;

		InputTick (delta);

		if(!acceleratedThisTick)
		{
			Brake(delta, passiveBrake);
		}

		//RestictSpeed ();

		TickGuns (delta);

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

	private void TickGuns(float delta)
	{
		if(firingSpeedPUpTimeLeft > 0)
		{
			firingSpeedPUpTimeLeft -= delta;
		}
		float kff = (firingSpeedPUpTimeLeft > 0) ? firingSpeedPUpKoeff : 1;
		
		var d = delta * kff;
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].Tick(d);
		}
	}

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

	public void Shoot()
	{
		for (int i = 0; i < guns.Count; i++) 
		{
			guns[i].ShootIfReady();
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
