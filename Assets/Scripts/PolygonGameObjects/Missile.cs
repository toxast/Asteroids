using UnityEngine;
using System.Collections;

public class Missile : PolygonGameObject
{
	private float lifeTime = 20f;

	private float thrust = 25f;
	private float maxVelocity = 40f;
	private float maxVelocitySqr;
	private float rotationSpeed = 200f;
	public float damage;

	private Transform targetTransform;

	Rotaitor rotaitor;

	void Awake () 
	{
		cacheTransform = transform;
	}
	
	public void Init(GameObject target, ShootPlace place)
	{
		this.targetTransform = target.transform;
		this.damage = place.damage;
		//this.lifeTime = place.lifeTime;

		maxVelocitySqr = maxVelocity * maxVelocity;

		rotaitor = new Rotaitor (cacheTransform, rotationSpeed);
	}
	
	public override void Tick(float delta)
	{
		RotateOnTarget (delta);
		ApplyThrust (delta);

		Vector3 deltaDistance = velocity*delta;
		cacheTransform.position += deltaDistance;


		lifeTime -= delta; 
		if(lifeTime < 0)
		{
			Destroy(gameObject);
		}
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
