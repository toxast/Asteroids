using UnityEngine;
using System.Collections;

public class Missile : SpaceShip, IBullet
{
	public float damage{ get; set;}
	protected float lifeTime = 5f;

	protected override void Awake ()
	{
		base.Awake ();
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		lifeTime -= delta; 
	}
	
	public bool Expired()
	{
		return lifeTime < 0;
	}

	public void Init()
	{
		DeathAnimation.MakeDeathForThatFellaYo (this, true);
	}

//	public void Init(PolygonGameObject target, float maxVelocity, float dmg, float lifetime)
//	{
//		DeathAnimation.MakeDeathForThatFellaYo (this, true);
//		base.Init (dmg, lifetime);
//		this.maxVelocity = maxVelocity;
//		this.target = target;
//		maxVelocitySqr = maxVelocity * maxVelocity;
//		rotaitor = new Rotaitor (cacheTransform, rotationSpeed);
//	}
	
//	public override void Tick(float delta)
//	{
//		base.Tick (delta);
//
//		if (target == null)
//			return;
//
//		RotateOnTarget (delta);
//		ApplyThrust (delta);
//	}
//
//	private void ApplyThrust(float deltaTime)
//	{
//		float k = 0.7f; 
//		
//		float deltaV = deltaTime * thrust;
//		velocity -= k * velocity.normalized * deltaV;
//		velocity += (1 + k)*cacheTransform.right * deltaV;
//
//		if(velocity.sqrMagnitude > maxVelocitySqr)
//		{
//			velocity = velocity.normalized * maxVelocity;
//		}
//	}
//
//	private void RotateOnTarget(float deltaTime)
//	{
//		var aimVelocity = (target.velocity - velocity) * 0.5f;
//		//var aimVelocity = target.velocity * 1.5f - velocity; imba
//		AimSystem aim = new AimSystem (target.cacheTransform.position, aimVelocity, cacheTransform.position, maxVelocity);  
//		if (!aim.canShoot) 
//		{
//			Vector2 distToTraget = target.cacheTransform.position - cacheTransform.position;
//			var currentAimAngle = Math2d.GetRotation (ref distToTraget) / Math2d.PIdiv180;
//			rotaitor.Rotate (deltaTime, currentAimAngle);
//		}
//		else
//		{
//			var currentAimAngle = aim.directionAngleRAD / Math2d.PIdiv180;
//			rotaitor.Rotate(deltaTime, currentAimAngle);
//		}
//	}
}
