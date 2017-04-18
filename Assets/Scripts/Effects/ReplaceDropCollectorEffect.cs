using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceDropCollectorEffect : DurationEffect {
	protected override eType etype { get { return eType.PullDrops; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;

	List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();

	public ReplaceDropCollectorEffect(Data data) : base(data) {
		this.data = data;
	}
	DropCollector oldCollector = null;
	public override void SetHolder (PolygonGameObject holder){
		base.SetHolder (holder);
		var sp = holder as SpaceShip;
		if (sp != null) {
			oldCollector = sp.collector;
			sp.collector = new DropCollector (data.force, data.range);
		}
	}

	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as ReplaceDropCollectorEffect;
		IncreaseTimeLeft (same.data.duration);
	}

	public override void OnExpired() {
		var sp = holder as SpaceShip;
		if (sp != null) {
			sp.collector = oldCollector;
		}
	}

	[System.Serializable]
	public class Data: IHasDuration, IApplyable {
		public float duration = 3;
		public float iduration{get {return duration;} set{duration = value;}}
		public float force = 10f;
		public float range = 70f;

		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new ReplaceDropCollectorEffect (this);
			effect = picker.AddEffect (effect);
			return effect;
		}
	}
}
