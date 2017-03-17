using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWaveEffect : TickableEffect{
	protected override eType etype { get { return eType.IceWave; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	Data data;
	bool used = false;

	public IceWaveEffect(Data data) {
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
	}

	public override bool IsFinished() {
		return used;
	}

	public override void Tick (float delta) {
		if (!IsFinished ()) {
			used = true;
			int layer = CollisionLayers.GetBulletLayerNum (holder.layerLogic);
			int collision = CollisionLayers.GetLayerCollisions (layer);
			new IceWave (holder.position, data.radius, data.iceData, 1f, Singleton<Main>.inst.gObjects, collision);
			AddEffects ();
		}
	}

	void AddEffects() {
		var clone = data.ringEffect.Clone ();
		clone.overrideSize = data.radius * 2;
		holder.AddParticles(new List<ParticleSystemsData> {clone});
	}

	[System.Serializable]
	public class Data : IApplyable{
		public float radius;
		public IceEffect.Data iceData;
		public ParticleSystemsData ringEffect;
		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new IceWaveEffect (this));
		}
	}
}