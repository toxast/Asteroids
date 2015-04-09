using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	public float turnSpeed = 220f;

	float passiveBrake = 2f;
	float brake = 15f;
	float thrust = 45f;
	float maxSpeed = 20f;
	float maxSpeedSqr;


	private class Thruster
	{
		public ParticleSystem thrust; 
		public float defaultLifetime;
	}
	List<Thruster> thrusters = new List<Thruster> ();
	public List<PolygonGameObject> turrets = new List<PolygonGameObject>();
//	ParticleSystem thrustPSystem; //TODO refactor
//	float defaultThrustLifetime;

	public DropCollector collector;

	protected InputController inputController;

	bool acceleratedThisTick = false;

	protected override float healthModifier {
		get {
			return base.healthModifier * Singleton<GlobalConfig>.inst.SpaceshipHealthModifier;
		}
	}

	protected override void Awake()
	{
		base.Awake ();
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector3.zero;
	}

	public override void SetTarget(IPolygonGameObject target)
	{
		base.SetTarget (target);

		var cnt = inputController as IGotTarget; //TODO no as
		if (cnt != null)
			cnt.SetTarget (target);
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

	public void SetThrusters(List<ThrusterSetupData> tdatas)
	{
		foreach (var t in tdatas) 
		{
			var thrusterInstance  = Instantiate(t.thrusterPrefab) as ParticleSystem;
			Math2d.PositionOnShooterPlace (thrusterInstance.transform, t.place, cacheTransform, true, 1);

			Thruster newThruster = new Thruster();
			newThruster.defaultLifetime = thrusterInstance.startLifetime;
			newThruster.thrust = thrusterInstance;
			thrusterInstance.startLifetime = newThruster.defaultLifetime / 3f;
			thrusters.Add(newThruster);
		}
	}

	public void AddTurret(Place place, PolygonGameObject turret)
	{
		Math2d.PositionOnShooterPlace(turret.cacheTransform, place, cacheTransform, true, -1);
		turrets.Add (turret);
	}

	public void SetThruster(ParticleSystem p, Vector2 pos)
	{
		Place place = new Place
		{
			dir = new Vector2(1,0),
			pos = pos,
		};
		ThrusterSetupData d = new ThrusterSetupData ();
		d.place = place;
		d.thrusterPrefab = p;
		List<ThrusterSetupData> tdatas = new List<ThrusterSetupData> ();
		tdatas.Add (d);
		SetThrusters (tdatas);
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

		//if(!acceleratedThisTick)
		//{
		//	Brake(delta, passiveBrake);
		//}

		//RestictSpeed ();

		TickGuns (delta);

		foreach (var t in turrets)
		{
			t.Tick(delta);
		}

		foreach(var th in thrusters)
		{
			//Or speed and rate by 3;
			//Or another trust
			//And change colr a bit??
			float dthrust = th.defaultLifetime * delta * 0.5f;
			dthrust = (acceleratedThisTick)? dthrust : -dthrust;
			th.thrust.startLifetime = Mathf.Clamp(th.thrust.startLifetime + dthrust, th.defaultLifetime/2f,  th.defaultLifetime);
		}
	}

	private void TickGuns(float delta)
	{
		if (deathAnimation != null && deathAnimation.started)
			return;

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

		if(inputController.IsBraking())
		{
			Brake(delta, brake);
		}
	}

	private void TurnByDirection(Vector3 dir, float delta)
	{
		bool turnLeft = Mathf.Sign(Math2d.Cross2(dir, cacheTransform.right)) < 0;
		var drot = turnSpeed * delta;
		var r = Mathf.Abs (rotation);
		if(r > turnSpeed)
		{
			if(r < turnSpeed + drot)
			{
				rotation = Mathf.Clamp(rotation, -turnSpeed, turnSpeed);
			}
			else
			{
				rotation = Mathf.Sign(rotation) * (r - drot);
			}
			base.ApplyRotation(delta);
		}
		else
		{
			rotation = turnLeft ? -turnSpeed : turnSpeed;
			Rotaitor rotaitor = new Rotaitor(cacheTransform, turnSpeed);
			var currentAimAngle = Math2d.GetRotation(dir) / Math2d.PIdiv180 ;
			bool reachedAngle = rotaitor.Rotate(delta, currentAimAngle);
			if(reachedAngle)
				rotation = 0;
		}
	}

	protected override void ApplyRotation (float dtime)
	{
		//base.ApplyRotation (dtime);
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
