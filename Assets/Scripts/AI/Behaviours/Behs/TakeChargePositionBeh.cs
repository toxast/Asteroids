using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TakeChargePositionBeh : DelayedActionBeh {

	float chargingDist;
	public TakeChargePositionBeh(CommonBeh.Data data, IDelayFlag delay, float chargingDist) : base(data,delay){
		this.chargingDist = chargingDist;
	}

	public override bool IsReadyToAct ()
	{
		return base.IsReadyToAct () && !Main.IsNull(target);
	}

	protected override IEnumerator Action ()
	{
		Vector2 newDir;
		float duration;
		GetDirectionForChargePosition (out newDir, out duration);
		SetFlyDir (newDir);
		var wait = WaitForSeconds (duration);
		while (wait.MoveNext ()) yield return true;
	}

	private void GetDirectionForChargePosition(out Vector2 dir, out float time) {

		float estimateSeconds = Random.Range(1f,2f);
		Vector2 estimateTargetPos = target.position + estimateSeconds * target.velocity;

		float maxTravelDuration = 3f;
		float R = chargingDist * 0.5f;

		//now try to position ship at R distance away from estimateTargetPos in less then maxTravelDuration.
		float maxTravelDistance = Math2d.GetDistance (maxTravelDuration, 0, thisShip.originalMaxSpeed, thisShip.thrust);

		Vector2 estimateDir = estimateTargetPos - thisShip.position;
		Vector2 dirNorm = estimateDir.normalized;
		float distToTarget = estimateDir.magnitude;

		float d = maxTravelDistance;
		if (distToTarget - maxTravelDistance > R) { //too far from target
			dir = dirNorm;
			time = maxTravelDuration;
		} else if(distToTarget + maxTravelDistance < R){ //too close to target
			dir = -dirNorm;
			time = maxTravelDuration;
		} else {
			float maxTravelAngle = Mathf.Acos((Mathf.Abs(R*R - d*d - distToTarget*distToTarget)) / (2f * d * distToTarget));
			Debug.LogWarning("maxTravelAngle " + Mathf.Rad2Deg * maxTravelAngle);
			Vector2 travelDirNorm = dirNorm * Mathf.Sign (distToTarget - R);
			float travelAngle = Random.Range (-maxTravelAngle, maxTravelAngle);
			dir = Math2d.RotateVertex(travelDirNorm, travelAngle);
			float delta = Mathf.Abs (distToTarget - R);
			float travelDist = delta + (d - delta) * (travelAngle/maxTravelAngle);
			travelDist = Mathf.Abs (travelDist);
			time = Math2d.GetDuration (travelDist, 0, thisShip.originalMaxSpeed, thisShip.thrust);
			//Debug.DrawLine (thisShip.position, thisShip.position + dir * travelDist, Color.magenta, 5f);
			//Debug.DrawLine (target.position, estimateTargetPos, Color.green, 5f);
		}

		Debug.LogWarning("fly to charge position " + time + " " + maxTravelDuration );
		time = Mathf.Clamp (time, 0, maxTravelDuration);
	}
}
