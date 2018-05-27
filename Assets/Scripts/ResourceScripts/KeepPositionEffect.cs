using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPositionEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }
	public bool pause = false;

	PolygonGameObject relative;
	Data data;
	float mult = 1;

	public KeepPositionEffect(Data data, PolygonGameObject relative) {
		this.relative = relative;
		this.data = data;
	}

	public override void Tick (float delta) {
		if (pause) {
			return;
		}

		float force = data.force * mult;
		float maxSpeed = data.maxSpeed * mult;
		float maxSpedSqr = maxSpeed * maxSpeed;
		if (!Main.IsNull (relative)) {
			var targetPos = relative.position + Math2d.RotateVertexDeg (data.pos, relative.cacheTransform.eulerAngles.z);
			FollowAim aim = new FollowAim(targetPos, relative.velocity,  holder.position, holder.velocity, force, maxSpeed);
			if (aim.forceDir != Vector2.zero) {
				Debug.DrawLine (holder.position, holder.position + aim.forceDir.normalized * force * aim.forceMultiplier, Color.red);
				if (aim.forceMultiplier < 1) {
					Debug.LogError (aim.forceMultiplier);			
				}
				holder.Accelerate (delta, force * aim.forceMultiplier, data.stability, maxSpeed, maxSpedSqr, aim.forceDir.normalized);
			}
			Debug.DrawLine (holder.position, targetPos, Color.green);
		}
	}


	public void MultiplyForceAndVelocity(float mul){
		mult *= mul;
	}

	public override bool IsFinished ()	{
		return Main.IsNull (relative);
	}

	[System.Serializable]
	public class Data{
		public float stability = 1;
		public float maxSpeed;
		public float force;
		public Vector2 pos;
	}
}
