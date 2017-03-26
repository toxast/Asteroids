using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBulletEffect : DurationEffect {
	protected override eType etype { get { return eType.HeavyBullet; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;

	public HeavyBulletEffect(Data data) : base(data.duration) {
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		holder.heavyBulletData = data;
	}

    public override void OnExpired() {
        holder.heavyBulletData = null;
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
		public bool applyForceToLazer = false;
		public float iduration{get {return duration;} set{duration = value;}}
		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new HeavyBulletEffect (this);
			picker.AddEffect (effect);
			return effect;
		}
	}
}
