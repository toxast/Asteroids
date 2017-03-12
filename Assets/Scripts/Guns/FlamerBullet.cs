using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FlamerBullet : PolygonGameObject {
	float deceleration;

	float force = 10f;
	float forceAngleChangeSpeed = 120f;

	float killThreshold;
	float applyForceThreshold;
	float forceChangeSign;
	float maxVelocity;
	Vector2 forceDir;
	float lastVelocity = 0;
	MFlamerGunData data;

	public void InitFlamingBullet( MFlamerGunData data, float startingSpeedMagnitude ) {
		this.data = data;
		this.deceleration = data.deceleration.RandomValue;
        this.burnDotData = data.dot;
		forceChangeSign = Math2d.RandomSign();
		maxVelocity = startingSpeedMagnitude;
		lastVelocity = startingSpeedMagnitude;
		applyForceThreshold = startingSpeedMagnitude * 0.4f;
		if (data.forceUseBulletLifetime) {
			killThreshold = -1; //not used
			InitLifetime (data.lifeTime);
		} else {
			killThreshold = startingSpeedMagnitude * 0.1f;
		}
    }
		
	public override void Tick(float delta) {
		base.Tick(delta);
		var oldVelocity = lastVelocity;
		Brake(delta, deceleration);
		var curVelocity = velocity.magnitude;
		maxVelocity = curVelocity;
		if (!data.forceUseBulletLifetime && curVelocity < killThreshold) {
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

