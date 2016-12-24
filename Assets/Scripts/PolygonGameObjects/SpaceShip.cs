using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	public float shootAngle = 15f;
	public float turnSpeed = 220f;
	public float maxSpeed = 20f;

	//float passiveBrake = 2f;
	float brake = 15f;
	float thrust = 45f;
	float maxSpeedSqr;
	float stability = 0.5f;

	private class Thruster
	{
		public ParticleSystem thrust; 
		public float defaultLifetime;
		public float startingColorAlpha;
	}
	List<Thruster> thrusters = new List<Thruster> ();


	public DropCollector collector;

	protected InputController inputController;

	bool acceleratedThisTick = false;


	//hack for ships menu
	public void ShowFullThruster()
	{
		foreach (var th in thrusters) {
			th.thrust.startLifetime = th.defaultLifetime;
		}
	}

	public override void SetAlpha (float a)
	{
		base.SetAlpha (a);

		foreach (var item in thrusters) {
			var tcolor = item.thrust.startColor;
			tcolor.a = item.startingColorAlpha * a;
			item.thrust.startColor = tcolor;
		}
	}

	protected override void Awake()
	{
		base.Awake ();
		maxSpeedSqr = maxSpeed*maxSpeed;
		velocity = Vector2.zero;
	}

	public void InitSpaceShip(PhysicalData physical, SpaceshipData data)
	{
		InitPolygonGameObject (physical);

		shootAngle = data.shootAngle;
		turnSpeed = data.turnSpeed;
		//passiveBrake = data.passiveBrake;
		thrust = data.thrust;
		maxSpeed = data.maxSpeed;
		stability = data.stability;
		maxSpeedSqr = maxSpeed*maxSpeed;
	}

	public override void SetTarget(PolygonGameObject target)
	{
		base.SetTarget (target);

		var cnt = inputController as IGotTarget; //TODO no as
		if (cnt != null)
			cnt.SetTarget (target);
	}

	public void SetController(InputController iController)
	{
		inputController = iController;
	}

	public void SetThrusters(List<ThrusterSetupData> tdatas)
	{
		foreach (var t in tdatas) 
		{
			if(t.thrusterPrefab == null)
			{
				Debug.LogError("null thruster");
			}
			else
			{
				var thrusterInstance  = Instantiate(t.thrusterPrefab) as ParticleSystem;
				Math2d.PositionOnParent (thrusterInstance.transform, t.place, cacheTransform, true, 1);

				Thruster newThruster = new Thruster();
				newThruster.defaultLifetime = thrusterInstance.startLifetime;
				newThruster.thrust = thrusterInstance;
				newThruster.startingColorAlpha = thrusterInstance.startColor.a;
				thrusterInstance.startLifetime = newThruster.defaultLifetime / 3f;
				thrusters.Add(newThruster);
			}
		}
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
		float deltaV = delta * thrust;
		velocity -= stability * velocity.normalized * deltaV;
		velocity += (Vector2)((1 + stability)*cacheTransform.right * deltaV);

		if(velocity.sqrMagnitude > maxSpeedSqr)
		{
			velocity = velocity.normalized*(Mathf.Clamp(velocity.magnitude - deltaV, maxSpeed, Mathf.Infinity));
		}

		acceleratedThisTick = true;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);

		acceleratedThisTick = false;

		InputTick (delta);

		TickGuns (delta);

		foreach(var th in thrusters)
		{
			float dthrust = th.defaultLifetime * delta * 0.5f;
			dthrust = (acceleratedThisTick)? dthrust : -dthrust;
			th.thrust.startLifetime = Mathf.Clamp(th.thrust.startLifetime + dthrust, th.defaultLifetime/2f,  th.defaultLifetime);
		}
	}

	void InputTick(float delta)
	{
		inputController.Tick (this);

		var dir = inputController.turnDirection;

		if(dir != Vector2.zero)
		{
			TurnByDirection (dir, delta);
		}

		if(inputController.shooting)
		{
			Shoot();
		}
		else
		{
			//hack
			//spawner guns should shoot if there is a target
			if(!Main.IsNull(target) && spawnerGuns.Any())
			{
				for (int i = 0; i < spawnerGuns.Count; i++) 
				{
					guns[spawnerGuns[i]].ShootIfReady();
				}
			}
		}

		if(inputController.accelerating)
		{
			Accelerate(delta);
		}

		if(inputController.braking)
		{
			Brake(delta, brake);
		}
	}

	private void TurnByDirection(Vector3 dir, float delta)
	{
		bool turnLeft = Mathf.Sign(Math2d.Cross2(dir, cacheTransform.right)) < 0;
		var deltaRotation = turnSpeed * delta;
		var r = Mathf.Abs (rotation);
		if(r > turnSpeed)
		{
			if(r < turnSpeed + deltaRotation)
			{
				rotation = Mathf.Clamp(rotation, -turnSpeed, turnSpeed);
			}
			else
			{
				rotation = Mathf.Sign(rotation) * (r - deltaRotation);
			}
			base.ApplyRotation(delta);
		}
		else
		{
			rotation = turnLeft ? -turnSpeed : turnSpeed;
			Rotaitor rotaitor = new Rotaitor(cacheTransform, turnSpeed);
			var currentAimAngle = Math2d.GetRotationRad(dir) / Math2d.PIdiv180 ;
			bool reachedAngle = rotaitor.Rotate(delta, currentAimAngle);
			if(reachedAngle)
				rotation = 0;
		}
	}

	protected override void ApplyRotation (float dtime)
	{
		//base.ApplyRotation (dtime); 
	}


//	private float firingSpeedPUpKoeff = 1f;
//	private float firingSpeedPUpTimeLeft = 0f;
//	public void ChangeFiringSpeed(float koeff, float duration)
//	{
//		firingSpeedPUpKoeff = koeff;
//		firingSpeedPUpTimeLeft = duration;
//	}
}
