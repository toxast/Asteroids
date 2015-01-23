using UnityEngine;
using System.Collections;

public class SawEnemy : PolygonGameObject, IGotTarget
{
	private PolygonGameObject target;

	private float initialRotation;
	private float initialVelocity;

	private float rotationChargingSpeed = 80f;
	private float chargeRotation = 300f;
	private float detectionDistance = 40f;
	private float chargeSpeed;
	private float brakes = 5f;
	private float chargeDuration = 3f;

	private float initialVelocitySqr;
	private float detectionDistanceSqr;

	public void Init(PolygonGameObject ptarget)
	{
		initialRotation = Random.Range(40,100);
		initialVelocity = Random.Range(2f, 7f);
		chargeSpeed = 90f/Mathf.Sqrt(mass);

		//TODO: asteroid?
		float a = Random.Range(0f, 359f) * Math2d.PIdiv180;
		velocity = new Vector3(Mathf.Cos(a)*initialVelocity, Mathf.Sin(a)*initialVelocity, 0f);
		rotation = initialRotation;

		SetTarget(ptarget);
		detectionDistanceSqr = detectionDistance*detectionDistance;
		initialVelocitySqr = initialVelocity * initialVelocity;
	}

	protected override float healthModifier {
		get {
			return base.healthModifier * 3;
		}
	}

	public void SetTarget(PolygonGameObject target)
	{
		this.target = target;
	}

	void Start () 
	{
		StartCoroutine(CheckDistanceAndCharge());
	}

	//TODO: asteroid?
	public override void Tick(float delta)
	{
		cacheTransform.position += velocity * delta;
		cacheTransform.Rotate(Vector3.back, rotation*delta);
	}

	IEnumerator CheckDistanceAndCharge()
	{
		float deltaTime = 0.3f;

		while(true)
		{
			yield return new WaitForSeconds(deltaTime); 

			if(target != null)
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
		if(target != null)
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
