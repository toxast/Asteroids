using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject 
{
	public float shootAngle = 15f;
	public float turnSpeed = 220f;

    float _maxSpeed = 0;
	public float maxSpeedSqr{ get; private set; }
	public float maxSpeed {
        get { return _maxSpeed; }
        set {
            _maxSpeed = value;
            maxSpeedSqr = _maxSpeed * _maxSpeed;
        }
    }

	float passiveBrake = 2f;
    public float brake = 15f;
    public float thrust = 45f;
    public float stability = 0.5f;

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
			var pmain = th.thrust.main;
			pmain.startLifetimeMultiplier = th.defaultLifetime;
		}
	}

    public void SetAlphaAndInvisibility(float alpha) {
        SetAlpha(alpha);
        SetInvisible(alpha == 0);
    }

    public override void SetAlpha (float a)
	{
		base.SetAlpha (a);

		foreach (var item in thrusters) {
			//fuck new unity particle systems, trying to just set the alpha
			var ps = item.thrust;
			var pmain = ps.main;
			var startCol = pmain.startColor;
			var colr = startCol.color;
			colr.a = item.startingColorAlpha * a;
			startCol.color = colr;
			pmain.startColor = startCol;
		}
	}

	protected override void Awake()
	{
		base.Awake ();
		velocity = Vector2.zero;
	}

	public void InitSpaceShip(PhysicalData physical, SpaceshipData data)
	{
		InitPolygonGameObject (physical);

		shootAngle = data.shootAngle;
		turnSpeed = data.turnSpeed;
        brake = data.brake;
		passiveBrake = 2;//data.passiveBrake;
        thrust = data.thrust;
		maxSpeed = data.maxSpeed;
		stability = data.stability;
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

	public void SetThrusters(List<ParticleSystemsData> tdatas)
	{
		foreach (var t in tdatas) 
		{
			if(t.prefab == null)
			{
				Debug.LogError("null thruster");
			}
			else
			{
				var thrusterInstance  = Instantiate(t.prefab) as ParticleSystem;
				var pmain = thrusterInstance.main;
				Math2d.PositionOnParent (thrusterInstance.transform, t.place, cacheTransform, true, 1);
				Thruster newThruster = new Thruster();
				newThruster.defaultLifetime = pmain.startLifetimeMultiplier;
				newThruster.thrust = thrusterInstance;
				newThruster.startingColorAlpha = pmain.startColor.color.a;
				pmain.startLifetime = newThruster.defaultLifetime / 3f;
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
		ParticleSystemsData d = new ParticleSystemsData ();
		d.place = place;
		d.prefab = p;
		List<ParticleSystemsData> tdatas = new List<ParticleSystemsData> ();
		tdatas.Add (d);
		SetThrusters (tdatas);
	}

	public void Accelerate(float delta)	{
		base.Accelerate(delta, thrust, stability, maxSpeed, maxSpeedSqr, cacheTransform.right);
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
			var pmain = th.thrust.main;
			var stLifetime = Mathf.Clamp(pmain.startLifetimeMultiplier + dthrust, th.defaultLifetime/2f,  th.defaultLifetime);
			pmain.startLifetimeMultiplier = stLifetime;
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

        if (inputController.accelerating) {
            Accelerate(delta);
        } else if (inputController.braking) {
            Brake(delta, brake);
        } else {
            Brake(delta, passiveBrake);
        }
	}


    public void TurnRight(float delta) {
        TurnDirection(true, delta);
    }

    public void TurnLeft(float delta) {
        TurnDirection(false, delta);
    }

    private void TurnDirection(bool right, float delta) {
        float angle = 90f * (right ? -1 : 1);
        var newDir = Math2d.RotateVertexDeg(cacheTransform.right, angle);
        TurnByDirection(newDir, delta);
    }

    public bool lastRotationDirLeft = false;
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
        lastRotationDirLeft = turnLeft;

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

    public override void SetSpawnParent(PolygonGameObject prnt) {
        base.SetSpawnParent(prnt);
        if (inputController != null) {
            inputController.SetSpawnParent(prnt);
        } else {
            Debug.LogError("inputController is null when SetSpawnParent");
        }
    }
}
