using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBulletEffect : TickableEffect 
{
	protected override eType etype { get { return eType.HeavyBullet; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;
	float timeLeft;

	public HeavyBulletEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		holder.heavyBulletData = data;
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			if (IsFinished ()) {
				holder.heavyBulletData = null;
			}
		}
	}

	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as HeavyBulletEffect;
		timeLeft += same.data.duration;
		data = same.data;
		holder.heavyBulletData = same.data;
	}

	[System.Serializable]
	public class Data : IApplyable, IHasDuration{
		public float duration;
		public float multiplier;
		public float iduration{get {return duration;} set{duration = value;}}

		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new HeavyBulletEffect (this));
		}
	}
}
