using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientationEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }
	public float extraOffsetAngle = 0;

	AdvancedTurnComponent rotaitor;
	PolygonGameObject relative;
	Data data;

	public float rotaitingSpeed{
		get{ return rotaitor.turnSpeed;}
	}

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
			//Debug.DrawLine (holder.position, holder.position + targetDir.normalized * 10, Color.yellow);
			if (targetDir != Vector2.zero) {
				if (data.offsetAngle + extraOffsetAngle != 0) {
					targetDir = Math2d.RotateVertexDeg (targetDir, data.offsetAngle + extraOffsetAngle);
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


