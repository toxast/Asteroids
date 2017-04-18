using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EffectDeathAnimation : TickableEffect{

	protected override eType etype { get { return eType.EffectDeathAnimation; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false;} }

	protected Data data;
	bool spawned = false;

	public EffectDeathAnimation(Data data) {
		this.data = data;
	}

	public override void Tick (float delta) {
		if (!spawned && data.effect.iprogress == 1) {
			spawned = true;
			holder.AddParticles (data.particles);
		}
	}

	public override bool IsFinished() {	return spawned; }

	[System.Serializable]
	public class Data : IApplyable {
		public IHasProgress effect;
		public List<ParticleSystemsData> particles;
		public IHasProgress Apply(PolygonGameObject picker) {
			picker.AddEffect (new EffectDeathAnimation (this));
			return null;
		}
	}
}