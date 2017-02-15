using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FlamingBullet : PolygonGameObject {
	float deceleration;
	DOTEffect.Data dot;
	float startingSpeedMagnitude;

	float force = 10f;
	float forceAngleChangeSpeed = 120f;

	float killThreshold;
	float applyForceThreshold;
	float forceChangeSign;
	float maxVelocity;
	Vector2 forceDir;
	float lastVelocity = 0;

	public void InitFlamingBullet( MFlamerGunData data, Vector2 startingSpeed ) {
		this.deceleration = data.deceleration.RandomValue;
        this.dot = data.dot;
		forceChangeSign = Math2d.RandomSign();
		startingSpeedMagnitude = startingSpeed.magnitude;
		maxVelocity = startingSpeedMagnitude;
		lastVelocity = startingSpeedMagnitude;
		applyForceThreshold = startingSpeedMagnitude * 0.5f;
		killThreshold = startingSpeedMagnitude * 0.1f;
    }

	public override void OnHit(PolygonGameObject other) {
        other.AddEffect(new BurningEffect(dot));
    }

	public override void Tick(float delta) {
		base.Tick(delta);
		var oldVelocity = lastVelocity;
		Brake(delta, deceleration);
		var curVelocity = velocity.magnitude;
		maxVelocity = curVelocity;
		if (curVelocity < killThreshold) {
			Kill ();
		}
		if (oldVelocity > applyForceThreshold && curVelocity < applyForceThreshold) {
			forceDir = Math2d.RotateVertexDeg (new Vector2 (1, 0), UnityEngine.Random.Range (0f, 360f));
		}
		if (curVelocity < applyForceThreshold) {
			forceDir = Math2d.RotateVertexDeg (forceDir, forceChangeSign * forceAngleChangeSpeed * delta);
			Accelerate (delta, force, 0.5f, maxVelocity, maxVelocity * maxVelocity, forceDir);
			Debug.DrawLine (position + forceDir * 10f, position);
		}
		lastVelocity = curVelocity;
	}
}

public class FlamerGun : BulletGun<FlamingBullet>
{
	MFlamerGunData fdata;

	public override float Range	{
		get { 
			float t =  0.8f * bulletSpeed / fdata.deceleration.Middle ;
			return t * bulletSpeed - 0.5f * t * t * fdata.deceleration.Middle; 
		}
	}
	public override float BulletSpeedForAim{ get { return fdata.bulletSpeed * 0.8f; } }

    public FlamerGun(Place place, MFlamerGunData data, PolygonGameObject parent):base(place, data, parent)
	{
		fdata = data;
	}

    protected override void InitBullet(FlamingBullet b)
    {
		base.InitBullet(b);
		b.InitFlamingBullet(fdata, b.velocity);
    }

	protected override PolygonGameObject.DestructionType GetBulletDestruction ()
	{
		return PolygonGameObject.DestructionType.eDisappear;
	}

	protected override float GetBulletVelocity ()
	{
		return base.GetBulletVelocity () + UnityEngine.Random.Range(-fdata.velocityRandomRange, fdata.velocityRandomRange);
	}
}
