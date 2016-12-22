using UnityEngine;
using System.Collections;

public class SawEnemy : Asteroid
{
	private float initialRotation;
	private float initialVelocity;

	private float rotationChargingRate;
	private float rotationSlowingRate;
	private float chargeRotation;
	private float veloityChargeRate;
	private float velocityslowingRate;
	private float chargeDuration;
	private float chargeSpeed;

	//private float initialVelocitySqr;

	public void InitSawEnemy(SawInitData data) 
	{
		this.reward = data.reward;
		this.InitAsteroid (data.physical, data.speed, data.rotation);

		initialRotation = rotation;
		initialVelocity = velocity.magnitude;
		//initialVelocitySqr = initialVelocity;

		this.chargeRotation = data.chargeRotation;
		this.chargeSpeed = data.chargeSpeed;

		this.rotationChargingRate = (data.chargeRotation - initialRotation) / data.prepareTime;
		this.veloityChargeRate = (data.chargeSpeed - initialVelocity) / data.prepareTime;

		this.rotationSlowingRate = (data.chargeRotation - initialRotation) / data.slowingDuration;
		this.velocityslowingRate = (data.chargeSpeed - initialVelocity) / data.slowingDuration;

		this.chargeDuration = data.chargeDuration;
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
				if(Mathf.Abs(rotation) > chargeRotation)
				{
					yield return StartCoroutine( Charge() ); 
					yield return StartCoroutine( Slow() ); 
				}
				else
				{
					rotation += Mathf.Sign(rotation) * rotationChargingRate * deltaTime;
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
		float diff = Mathf.Abs (rotation) - Mathf.Abs (initialRotation);
		float delta = rotationSlowingRate * deltaTime;
		bool slowingRotation = diff > 0;
		if(slowingRotation)
		{
			if (delta < diff) {
				rotation -= Mathf.Sign (rotation) * delta;
			} else {
				rotation = Mathf.Sign (rotation) * initialRotation;
				slowingRotation = false;
			}
		}

		var vmag = velocity.magnitude;
		diff = vmag - initialVelocity;
		delta = velocityslowingRate * deltaTime;
		bool slowingVelocity = diff > 0;
		if(slowingVelocity)
		{
			if (delta < diff) {
				velocity = (velocity / vmag) * (vmag - delta);
			} else {
				velocity = (velocity / vmag) * initialVelocity;
				slowingVelocity = false;
			}
		}
		
		return slowingVelocity || slowingRotation;
	}
	
	IEnumerator Slow()
	{
		float deltaTime = 0.15f;
		
		bool slowing = true;
		while(slowing)
		{
			slowing = Slow(deltaTime);
			
			yield return new WaitForSeconds(deltaTime); 
		}
	}
}
