using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpaceShip : PolygonGameObject , IFreezble
{
	public float shootAngle = 15f;

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

	public float thrust { get; private set; }
	public float turnSpeed { get; private set; }
	public float stability { get; private set; }

	public float originalTurnSpeed { get; private set; }
	public float originalMaxSpeed { get; private set; }

	private float currentThrustMultiplier = 1;
	public void MultiplyThrust(float mul){ thrust *= mul;  currentThrustMultiplier *= mul;}
	public void MultiplyMaxSpeed(float mul){ maxSpeed *= mul; }
	public void MultiplyTurnSpeed(float mul){ turnSpeed *= mul; }
	public void MultiplyStability(float mul){ stability *= mul; }
	public void MultiplyBrake(float mul){ passiveBrake *= mul;  brake *= mul;}

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

	public override void Freeze(float multipiler){
		base.Freeze (multipiler);
		MultiplyStability(multipiler);
		MultiplyThrust(multipiler);
		MultiplyTurnSpeed(multipiler);
		MultiplyBrake (multipiler);

		if (inputController != null) {
			inputController.Freeze (multipiler);
		}
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

	TurnComponent turnComponent;
	public void InitSpaceShip(SpaceshipData data)
	{
		shootAngle = data.shootAngle;
		turnSpeed = data.turnSpeed;
        brake = data.brake;
		passiveBrake = 2;//data.passiveBrake;
        thrust = data.thrust;
		maxSpeed = data.maxSpeed;
		stability = data.stability;

		originalTurnSpeed = turnSpeed;
		originalMaxSpeed = maxSpeed;
		turnComponent = new TurnComponent (this);
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

	public void Accelerate(float delta, float value01 = 1)	{
		base.Accelerate(delta, thrust * value01, stability, maxSpeed, maxSpeedSqr, cacheTransform.right);
		acceleratedThisTick = true;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
		acceleratedThisTick = false;
		InputTick (delta);
		TickGuns (delta);
		foreach(var th in thrusters) {
			float dthrust = th.defaultLifetime * delta * 0.5f;
			dthrust = (acceleratedThisTick)? dthrust : -dthrust;
			var pmain = th.thrust.main;
			var stLifetime = Mathf.Clamp(pmain.startLifetimeMultiplier + dthrust, th.defaultLifetime/2f,  th.defaultLifetime * currentThrustMultiplier);
			pmain.startLifetimeMultiplier = stLifetime;
		}
	}

	void InputTick(float delta)
	{
		inputController.Tick (delta);
		var dir = inputController.turnDirection;
		bool shooting = inputController.shooting;
		if (RestrictShootingByFastRotation && Mathf.Abs (rotation) > turnSpeed * 1.2f) {
			shooting = false;
		}
		if (dir != Vector2.zero) {
			TurnByDirection (dir, delta);
		} else {
			TurnByDirection (cacheTransform.right, delta);
		}
		if (shooting) {
			Shoot ();
		}
		if (inputController.accelerating) {
			Accelerate (delta, inputController.accelerateValue01);
		} else if (inputController.braking) {
			Brake (delta, brake);
		} else {
			//Brake (delta, passiveBrake);
		}
	}

	protected virtual bool RestrictShootingByFastRotation{get{ return true; } }

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
    private void TurnByDirection(Vector3 dir, float delta) {
		lastRotationDirLeft = turnComponent.TurnByDirection(dir, delta, turnSpeed);
    }

	protected override void ApplyRotation (float dtime)
	{
		//base.ApplyRotation (dtime); 
	}
    public override void SetSpawnParent(PolygonGameObject prnt) {
        base.SetSpawnParent(prnt);
        if (inputController != null) {
            inputController.SetSpawnParent(prnt);
        } else {
            Debug.LogError("inputController is null when SetSpawnParent");
        }
    }
}


public class TurnComponent{
	protected PolygonGameObject holder;
	public TurnComponent (PolygonGameObject holder){
		this.holder = holder;
	}

	public bool TurnByDirection(Vector3 dir, float delta, float turnSpeed)
	{
		bool turnLeft = Mathf.Sign(Math2d.Cross2(dir, holder.cacheTransform.right)) < 0;
		var deltaRotation = turnSpeed * delta;
		var absRot = Mathf.Abs (holder.rotation);
		if(absRot > turnSpeed)
		{ //decrease extra rotation
			if(absRot < turnSpeed + deltaRotation)
			{
				holder.rotation = Mathf.Clamp(holder.rotation, -turnSpeed, turnSpeed);
			}
			else
			{
				holder.rotation = Mathf.Sign(holder.rotation) * (absRot - deltaRotation);
			}
			holder.cacheTransform.Rotate(Vector3.back, holder.rotation*delta);
		}
		else
		{ //controllable instant rotation
			holder.rotation = turnLeft ? -turnSpeed : turnSpeed;
			Rotaitor rotaitor = new Rotaitor(holder.cacheTransform, turnSpeed);
			var currentAimAngle = Math2d.GetRotationRad(dir) / Math2d.PIdiv180 ;
			bool reachedAngle = rotaitor.Rotate(delta, currentAimAngle);
			if(reachedAngle)
				holder.rotation = 0;
		}
		return turnLeft;
	}
}

public class AdvancedTurnComponent : TurnComponent{
	public float originalTurnSpeed{ get; private set;}
	public float turnSpeed{ get; private set;}
	public AdvancedTurnComponent(PolygonGameObject holder, float turnSpeed): base(holder){
		this.originalTurnSpeed = turnSpeed;
		this.turnSpeed = turnSpeed;
	}

	public void Freeze(float mul){
		turnSpeed *= mul;
	}

	public void MultiplyOriginalTurnSpeed(float mul){
		originalTurnSpeed *= mul;
		turnSpeed *= mul;
	}

	public bool TurnByDirection(Vector3 dir, float delta) {
		return TurnByDirection (dir, delta, turnSpeed);
	}

	public bool TurnDirection(bool right, float delta) {
		float angle = 90f * (right ? -1 : 1);
		var newDir = Math2d.RotateVertexDeg(holder.cacheTransform.right, angle);
		return TurnByDirection(newDir, delta);
	}

	public bool isFastRotation(){
		return  Mathf.Abs (holder.rotation) > turnSpeed * 1.2f;
	}

	/// <summary>
	/// aimDirNorm should be normalized!
	/// </summary>
	public bool inAngleRange(Vector2 aimDirNorm, float shootAngle){
		var angleToRotate = Math2d.DegBetweenNormUnsigned (aimDirNorm, holder.cacheTransform.right);
		return (angleToRotate < shootAngle);
	}

	public bool IsDirectionToTheLeft(Vector2 dir){
		return Math2d.Cross2(holder.cacheTransform.right, dir) > 0;
	}
	
}