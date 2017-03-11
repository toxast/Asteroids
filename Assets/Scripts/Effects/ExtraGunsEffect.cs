using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtraGunsEffect : TickableEffect {
	protected override eType etype { get { return eType.ExtraGuns; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	Data data;
	float timeLeft;
	List<Gun> guns;

	public ExtraGunsEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
		guns = new List<Gun> ();
		foreach (var gunplace in data.guns) {
			var gun = gunplace.GetGun (holder);
			guns.Add (gun);
		}
		holder.AddExtraGuns (guns);
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			if (IsFinished ()) {
				holder.RemoveGuns (guns);
			}
		}
	}

	[System.Serializable]
	public class Data : IHasDuration, IApplyable {
		public float duration;
		public float iduration{get {return duration;} set{duration = value;}}
		public List<MGunSetupData> guns;

		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new ExtraGunsEffect (this));
		}
	}


}


