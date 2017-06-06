using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotationEffect : TickableEffect {
	
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepRotation; } }

	Data data;

	public KeepRotationEffect(Data data) {
		this.data = data;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		var deltaRotation = data.acceleration * delta;
		var diff = data.targetRotation - holder.rotation;
		if (Mathf.Abs (diff) < deltaRotation) {
			holder.rotation = data.targetRotation;
		} else {
			holder.rotation += Mathf.Sign (diff) * deltaRotation;
		}
	}

	[System.Serializable]
	public class Data{
		public float targetRotation;
		public float acceleration;
	}
}

public class KeepOrientationEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }

	Rotaitor rotaitor;
	PolygonGameObject relative;
	Data data;

	public KeepOrientationEffect(Data data, PolygonGameObject relative) {
		this.relative = relative;
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder)	{
		base.SetHolder (holder);
		rotaitor = new Rotaitor (holder.cacheTransform, data.rotaitingSpeed);
	}

	public override void Tick (float delta) {
		if (!Main.IsNull (relative) && rotaitor != null) {
			rotaitor.Rotate(delta, Math2d.AngleRad(new Vector2(1,0), relative.position - holder.position) * Mathf.Rad2Deg);
		}
	}

	public override bool IsFinished ()	{
		return Main.IsNull (relative);
	}

	[System.Serializable]
	public class Data{
		public float rotaitingSpeed;
	}
}

