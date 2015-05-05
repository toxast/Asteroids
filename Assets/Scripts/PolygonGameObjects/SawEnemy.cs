using UnityEngine;
using System.Collections;

public class SawEnemy : Asteroid
{
	private float initialRotation;
	private float initialVelocity;

	private float rotationChargingSpeed;
	private float chargeRotation;
	private float chargeSpeed;
	private float brakes;
	private float chargeDuration;

	private float initialVelocitySqr;

	public void InitSawEnemy(SawInitData data) //300
	{
		this.InitAsteroid (data.density, data.healthModifier, data.speed, data.rotation);

		initialRotation = rotation;
		initialVelocity = velocity.magnitude;
		initialVelocitySqr = initialVelocity * initialVelocity;

		this.chargeSpeed = data.chargeSpeed;
		this.chargeRotation = data.chargeRotation;
		this.brakes = data.chargeSpeed / data.prepareTime;
		this.rotationChargingSpeed = data.chargeRotation / data.prepareTime;
		this.chargeDuration = data.chargeDuration;
	}

//	protected override float healthModifier {
//		get {
//			return base.healthModifier * 3;
//		}
//	}

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
				if(Mathf.Abs(rotation) > chargeRotation)
				{
					yield return StartCoroutine( Charge() ); 
					yield return StartCoroutine( Slow() ); 
				}
				else
				{
					rotation += Mathf.Sign(rotation) * rotationChargingSpeed * deltaTime;
				}
			}
			else
			{
				Slow(deltaTime);
			}
		}
	}

	IEnumerator Charge()
	{
		if(!Main.IsNull(target))
		{
			AimSystem aim = new AimSystem(target.position, target.velocity, position, chargeSpeed);
			if(aim.canShoot)
			{
				velocity = chargeSpeed * aim.direction.normalized;
			}
			else
			{
				Vector2 direction = target.position - position;
				velocity = chargeSpeed * direction.normalized;
			}
		}
		yield return new WaitForSeconds(chargeDuration); 
	}

	private bool Slow(float deltaTime)
	{
		bool slowingRotation = Mathf.Abs(rotation) > Mathf.Abs(initialRotation);
		if(slowingRotation)
		{
			rotation -= Mathf.Sign(rotation) * rotationChargingSpeed * deltaTime;
		}
		
		bool slowingVelocity = velocity.sqrMagnitude > initialVelocitySqr;
		if(slowingVelocity)
		{
			velocity -= velocity.normalized * brakes * deltaTime;
		}
		
		return slowingVelocity || slowingRotation;
	}
	
	IEnumerator Slow()
	{
		float deltaTime = 0.3f;
		
		bool slowing = true;
		while(slowing)
		{
			slowing = Slow(deltaTime);
			
			yield return new WaitForSeconds(deltaTime); 
		}
	}
}
