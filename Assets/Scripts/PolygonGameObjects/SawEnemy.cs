using UnityEngine;
using System.Collections;

public class SawEnemy : PolygonGameObject, IGotRotation, IGotVelocity
{
	private Vector3 velocity;
	private float rotation;

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

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
	}
	
	public float Rotation
	{
		get
		{
			return rotation;
		}
	}

	public void Init(PolygonGameObject target)
	{
		initialRotation = Random.Range(40,100);
		initialVelocity = Random.Range(2f, 7f);
		chargeSpeed = 90f/Mathf.Sqrt(mass);

		//TODO: asteroid?
		float a = Random.Range(0f, 359f) * Math2d.PIdiv180;
		velocity = new Vector3(Mathf.Cos(a)*initialVelocity, Mathf.Sin(a)*initialVelocity, 0f);
		rotation = initialRotation;

		this.target = target;
		detectionDistanceSqr = detectionDistance*detectionDistance;
		initialVelocitySqr = initialVelocity * initialVelocity;
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

	IEnumerator Charge()
	{
		Vector2 dist = target.cacheTransform.position - cacheTransform.position;
		velocity = chargeSpeed * dist.normalized;
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
