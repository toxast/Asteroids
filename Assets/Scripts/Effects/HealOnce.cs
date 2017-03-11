using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOnce : TickableEffect{
	protected override eType etype { get { return eType.HealOnce; } }
	public override bool CanBeUpdatedWithSameEffect { get { return false; } }

	Data data;
	bool used = false;

	public HealOnce(Data data) {
		this.data = data;
	}

	public override void SetHolder (PolygonGameObject holder){
		base.SetHolder (holder);
	}

	public override bool IsFinished() {
		return used;
	}
	
	public override void Tick (float delta) {
		if (!IsFinished ()) {
			used = true;
			holder.Heal (data.total);
			AddEffects ();
		}
	}

	void AddEffects(){
		holder.SetParticles(new List<ParticleSystemsData> { data.effect});
	}

	[System.Serializable]
	public class Data : IApplyable{
		public float total = 20;
		public ParticleSystemsData effect;

		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new HealOnce (this));
		}
	}
}