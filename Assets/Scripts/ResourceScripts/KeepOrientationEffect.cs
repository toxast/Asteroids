using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientationEffect : TickableEffect {

	public override bool CanBeUpdatedWithSameEffect { get { return false; } }
	protected override eType etype { get {return eType.KeepOrientation; } }
	public float extraOffsetAngle = 0;
	public bool forceFinish = false;
	public bool pause = false;
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

	public void MultiplyOriginalTurnSpeed(float mul){
		rotaitor.MultiplyOriginalTurnSpeed (mul);
	}

	public override void Tick (float delta) {
		if (pause) {
			return;
		}

		if (!Main.IsNull (relative) && rotaitor != null) {
			Vector2 targetDir;
			if (!data.relativeToHolderForward) {
				 targetDir = holder.position - relative.position;
			} else {
				targetDir = relative.cacheTransform.right;
			}
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
		return forceFinish || Main.IsNull (relative);
	}

	[System.Serializable]
	public class Data{
		public float offsetAngle;
		public float rotaitingSpeed;
		public bool relativeToHolderForward; //or holder pos
	}
}


