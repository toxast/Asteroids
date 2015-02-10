using UnityEngine;
using System.Collections;

public class Missile : BulletBase
{
	private float thrust = 35f;
	private float maxVelocity;
	private float maxVelocitySqr;
	private float rotationSpeed = 200f;

	Rotaitor rotaitor;

	void Awake () 
	{
		cacheTransform = transform;
	}
	
	public void Init(PolygonGameObject target, float maxVelocity, float dmg, float lifetime)
	{
		base.Init (dmg, lifetime);
		this.maxVelocity = maxVelocity;
		this.target = target;
		maxVelocitySqr = maxVelocity * maxVelocity;
		rotaitor = new Rotaitor (cacheTransform, rotationSpeed);
	}
	
	public override void Tick(float delta)
	{
		base.Tick (delta);

		if (target == null)
			return;

		RotateOnTarget (delta);
		ApplyThrust (delta);
	}

	private void ApplyThrust(float deltaTime)
	{
		float k = 0.7f; 
		
		float deltaV = deltaTime * thrust;
		velocity -= k * velocity.normalized * deltaV;
		velocity += (1 + k)*cacheTransform.right * deltaV;

		if(velocity.sqrMagnitude > maxVelocitySqr)
		{
			velocity = velocity.normalized * maxVelocity;
		}
	}

	private void RotateOnTarget(float deltaTime)
	{
		AimSystem aim = new AimSystem (target.cacheTransform.position, -velocity/2f, cacheTransform.position, maxVelocity);  
		if (!aim.canShoot) 
		{
			Vector2 distToTraget = target.cacheTransform.position - cacheTransform.position;
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
