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


