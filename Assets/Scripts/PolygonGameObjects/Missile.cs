using UnityEngine;
using System.Collections;

public class Missile : BulletBase
{
	private float thrust = 25f;
	private float maxVelocity = 40f;
	private float maxVelocitySqr;
	private float rotationSpeed = 200f;

	private Transform targetTransform;

	Rotaitor rotaitor;

	void Awake () 
	{
		cacheTransform = transform;
	}
	
	public void Init(GameObject target, ShootPlace place)
	{
		//base.Init (place);
		this.targetTransform = target.transform;
		maxVelocitySqr = maxVelocity * maxVelocity;
		rotaitor = new Rotaitor (cacheTransform, rotationSpeed);
	}
	
	public override void Tick(float delta)
	{
		base.Tick (delta);

		RotateOnTarget (delta);
		ApplyThrust (delta);
	}

	private void ApplyThrust(float deltaTime)
	{
		var delta = deltaTime * thrust;
		velocity += delta * cacheTransform.right;

		if(velocity.sqrMagnitude > maxVelocitySqr)
		{
			velocity = velocity.normalized * maxVelocity;
		}
	}

	private void RotateOnTarget(float deltaTime)
	{
		AimSystem aim = new AimSystem (targetTransform.position, -velocity/2f, cacheTransform.position, maxVelocity);  
		if (!aim.canShoot) 
		{
			Vector2 distToTraget = targetTransform.position - cacheTransform.position;
			var currentAimAngle = Math2d.GetRotation (ref distToTraget) / Math2d.PIdiv180;
			rotaitor.Rotate (deltaTime, currentAimAngle);
		}
		else
		{
			var currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
			rotaitor.Rotate(deltaTime, currentAimAngle);
		}
	}
}
