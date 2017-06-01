using UnityEngine;
using System.Collections;

public class SawEnemy : PolygonGameObject, IFreezble
{
	private float initialRotation;
	private float initialVelocity;

	private float rotationChargingRate;
	private float rotationSlowingRate;
	private float chargeRotation;
	private float velocityslowingRate;
	MSawData data;
	protected AIHelper.AccuracyChangerAdvanced accuracyChanger;
	protected float accuracy { get { return accuracyChanger.accuracy; } }

	public virtual void InitSawEnemy(MSawData data) 
	{
		this.data = data;
		this.reward = data.reward;
		InitPolygonGameObject (data.physical);
		Asteroid.InitRandomMovement (this, data.speed, data.rotation);
		accuracyChanger = new AIHelper.AccuracyChangerAdvanced(data.accuracy, this);

		initialRotation = rotation;
		initialVelocity = velocity.magnitude;

		this.chargeRotation = data.chargeRotation;
		this.rotationChargingRate = (data.chargeRotation) / data.prepareTime;
		this.rotationSlowingRate = (data.chargeRotation) / data.slowingDuration;
		this.velocityslowingRate = (data.chargeSpeed.max) / data.slowingDuration;

		if (data.useInvisibilityBeh) {
			this.invisibilityComponent = new InvisibilityComponent (this, data.invisData);
			this.increaceAlphaOnHitAndDropInvisibility = true;
		}
	}

	void Start () {
		StartCoroutine(CheckDistanceAndCharge());
	}

    protected virtual bool DoCharge() {
        return true;
    }

	public override void Freeze(float multipiler){
		rotationSlowingRate *= multipiler;
		velocityslowingRate *= multipiler;
		rotationChargingRate *= multipiler;
		base.Freeze (multipiler);
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		accuracyChanger.Tick (delta);
	}

	IEnumerator CheckDistanceAndCharge() {
		float deltaTime = 0.3f;
		while (true) {
            while (!DoCharge()) {
                yield return null;
            }
			yield return new WaitForSeconds (deltaTime); 
			if (!Main.IsNull (target)) {
				while (!Main.IsNull (target) && Mathf.Abs (rotation) < chargeRotation) {
					rotation += Mathf.Sign (rotation) * rotationChargingRate * Time.deltaTime;
					if (data.useInvisibilityBeh) {
						float approximateTimeToCharge = Mathf.Abs ((chargeRotation - Mathf.Abs (rotation)) / rotationChargingRate);
						if (approximateTimeToCharge <= data.invisData.fadeInDuration) {
							invisibilityComponent.SetState (false);
						}
					}
					yield return null;
				}
				if (!Main.IsNull (target)) {
					yield return StartCoroutine (Charge ()); 

					if (data.useInvisibilityBeh) {
						invisibilityComponent.SetState (true);
					}
					yield return StartCoroutine (Slow ());
				}
			} else {
				Slow (deltaTime);
			}
		}
	}

	GunsShowEffect currentGunsShowEffect;
	IEnumerator Charge() {
		float chargeSpeed = data.chargeSpeed.RandomValue;
		float chargeSpeedSqr = chargeSpeed * chargeSpeed;
		if (!Main.IsNull (target)) {
			if (data.gunsShowChargeEffect != null) {
				currentGunsShowEffect = new GunsShowEffect (data.gunsShowChargeEffect);
				AddEffect (currentGunsShowEffect);
			}
			AimSystem aim = new AimSystem (target.position, target.velocity * accuracy, position, chargeSpeed);
			if (aim.canShoot) {
				velocity = chargeSpeed * aim.directionDist.normalized;
			} else {
				Vector2 direction = target.position - position;
				velocity = chargeSpeed * direction.normalized;
			}
		}

		bool forceStopWhenMissed = Math2d.Chance(data.chanceForceStopWhenMissed);
		bool continueWhileGoingAfterTarget = Math2d.Chance(data.chanceContinueChargeUntilMiss);
		var chargeLeftDuration = data.chargeDuration.RandomValue;
		while (chargeLeftDuration > 0) {
			chargeLeftDuration -= Time.deltaTime;
			yield return null; 
			AccelerateTowardsTarget (chargeSpeed, chargeSpeedSqr, Time.deltaTime);
			if ((forceStopWhenMissed || continueWhileGoingAfterTarget) && !Main.IsNull(target)) {
				bool missed = Vector2.Dot ((target.position - position), velocity) <= 0;
				if (missed && forceStopWhenMissed) {
					break;
				}
				if (!missed && continueWhileGoingAfterTarget && chargeLeftDuration <= 0) {
					chargeLeftDuration = 0.0001f;
				}
			}
		}

		if (currentGunsShowEffect != null) {
			currentGunsShowEffect.ForceFinish ();
			currentGunsShowEffect = null;
		}
	}

	private void AccelerateTowardsTarget(float chargeSpeed, float chargeSpeedSqr, float deltaTime){
		if (data.thrust > 0 && !Main.IsNull(target)) {
			AimSystem aim = new AimSystem (target.position, target.velocity * accuracy, position, chargeSpeed);
			Accelerate (deltaTime, data.thrust, data.stability, chargeSpeed, chargeSpeedSqr, aim.directionDist.normalized);
		}
	}

	private bool Slow(float deltaTime) {
		float diff = Mathf.Abs (rotation) - Mathf.Abs (initialRotation);
		float delta = rotationSlowingRate * deltaTime;
		bool slowingRotation = diff > 0;
		if (slowingRotation) {
			if (delta < diff) {
				rotation -= Mathf.Sign (rotation) * delta;
			} else {
				rotation = initialRotation;
				slowingRotation = false;
			}
		}
		var vmag = velocity.magnitude;
		diff = vmag - initialVelocity;
		delta = velocityslowingRate * deltaTime;
		bool slowingVelocity = diff > 0;
		if (slowingVelocity) {
			if (delta < diff) {
				velocity = (velocity / vmag) * (vmag - delta);
			} else {
				velocity = (velocity / vmag) * initialVelocity;
				slowingVelocity = false;
			}
		}
		return slowingVelocity || slowingRotation;
	}
	
	IEnumerator Slow() {
		float deltaTime = 0.15f;
		bool slowing = true;
		while (slowing) {
			slowing = Slow (deltaTime);
			yield return new WaitForSeconds (deltaTime); 
		}
	}
}
