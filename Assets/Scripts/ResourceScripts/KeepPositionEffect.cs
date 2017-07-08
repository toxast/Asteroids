using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPositionEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }

	PolygonGameObject relative;
	Data data;

	public KeepPositionEffect(Data data, PolygonGameObject relative) {
		this.relative = relative;
		this.data = data;
	}

	public override void Tick (float delta) {
		if (!Main.IsNull (relative)) {
			var targetPos = relative.position + Math2d.RotateVertexDeg (data.pos, relative.cacheTransform.eulerAngles.z);
			FollowAim aim = new FollowAim(targetPos, relative.velocity,  holder.position, holder.velocity, data.force, data.maxSpeed);
			if (aim.forceDir != Vector2.zero) {
				Debug.DrawLine (holder.position, holder.position + aim.forceDir.normalized * data.force * aim.forceMultiplier, Color.red);
				holder.Accelerate (delta, data.force * aim.forceMultiplier, data.stability, data.maxSpeed, data.maxSpeed*data.maxSpeed, aim.forceDir.normalized);
			}
			Debug.DrawLine (holder.position, targetPos, Color.green);
		}
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
