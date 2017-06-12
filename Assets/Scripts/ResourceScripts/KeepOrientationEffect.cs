using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientationEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }

	AdvancedTurnComponent rotaitor;
	PolygonGameObject relative;
	Data data;

	public KeepOrientationEffect(Data data, PolygonGameObject relative) {
		this.relative = relative;
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder)	{
		base.SetHolder (holder);
		rotaitor = new AdvancedTurnComponent (holder, data.rotaitingSpeed);
	}

	public override void Tick (float delta) {
		if (!Main.IsNull (relative) && rotaitor != null) {
			Vector2 targetDir = holder.position - relative.position;
			if (targetDir != Vector2.zero) {
				if (data.offsetAngle != 0) {
					targetDir = Math2d.RotateVertexDeg (targetDir, data.offsetAngle);
				}
				rotaitor.TurnByDirection (targetDir, delta);
			}
		}
	}

	public override bool IsFinished ()	{
		return Main.IsNull (relative);
	}

	[System.Serializable]
	public class Data{
		public float offsetAngle;
		public float rotaitingSpeed;
	}
}


