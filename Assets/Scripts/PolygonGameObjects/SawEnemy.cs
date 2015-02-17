using UnityEngine;
using System.Collections;

public class SawEnemy : Asteroid
{
	private float initialRotation;
	private float initialVelocity;

	private float rotationChargingSpeed = 120f;
	private float chargeRotation = 300f;
	private float detectionDistance = 50f;
	private float chargeSpeed;
	private float brakes = 7f;
	private float chargeDuration = 3f;

	private float initialVelocitySqr;
	private float detectionDistanceSqr;

	public override void Init()
	{
		base.Init ();
		initialRotation = Random.Range(40,100);
		initialVelocity = Random.Range(2f, 7f);
		chargeSpeed = 130f/Mathf.Sqrt(mass);

		//TODO: asteroid?
		float a = Random.Range(0f, 359f) * Math2d.PIdiv180;
		velocity = new Vector3(Mathf.Cos(a)*initialVelocity, Mathf.Sin(a)*initialVelocity, 0f);
		rotation = initialRotation;

		detectionDistanceSqr = detectionDistance*detectionDistance;
		initialVelocitySqr = initialVelocity * initialVelocity;
	}

	protected override float healthModifier {
		get {
			return base.healthModifier * 3;
		}
	}

	void Start () 
	{
		StartCoroutine(CheckDistanceAndCharge());
	}

	IEnumerator CheckDistanceAndCharge()
	{
		float deltaTime = 0.3f;

		while(true)
		{
			yield return new WaitForSeconds(deltaTime); 

			if(!Main.IsNull(target))
			{
				Vector2 dist = target.cacheTransform.position - cacheTransform.position;
				bool targetInRange = dist.sqrMagnitude < detectionDistanceSqr;
				if(targetInRange)
				{
					if(rotation > chargeRotation)
					{
						yield return StartCoroutine( Charge() ); 
						yield return StartCoroutine( Slow() ); 
					}
					else
					{
						rotation += rotationChargingSpeed * deltaTime;
					}
				}
				else
				{
					bool slowingRotation = rotation > initialRotation;
					if(slowingRotation)
					{
						rotation -= rotationChargingSpeed * deltaTime;
					}
					
					bool slowingVelocity = velocity.sqrMagnitude > initialVelocitySqr;
					if(slowingVelocity)
					{
						velocity -= velocity.normalized * deltaTime * brakes;
					}
				}

			}
		}
	}

	IEnumerator Charge()
	{
		if(!Main.IsNull(target))
		{
			AimSystem aim = new AimSystem(target.cacheTransform.position, target.velocity, cacheTransform.position, chargeSpeed);
			if(aim.canShoot)
			{
				velocity = chargeSpeed * aim.direction.normalized;
			}
			else
			{
				Vector2 direction = target.cacheTransform.position - cacheTransform.position;
				velocity = chargeSpeed * direction.normalized;
			}
		}
		yield return new WaitForSeconds(chargeDuration); 
	}
	
	IEnumerator Slow()
	{
		float deltaTime = 0.3f;
		
		bool slowing = true;
		while(slowing)
		{
			bool slowingRotation = rotation > initialRotation;
			if(slowingRotation)
			{
				rotation -= rotationChargingSpeed * deltaTime;
			}
			
			bool slowingVelocity = velocity.sqrMagnitude > initialVelocitySqr;
			if(slowingVelocity)
			{
				velocity -= velocity.normalized * deltaTime * brakes;
			}
			
			slowing = slowingVelocity || slowingRotation;
			
			yield return new WaitForSeconds(deltaTime); 
		}
	}
}
