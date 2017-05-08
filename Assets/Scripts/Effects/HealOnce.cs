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
		holder.AddParticles(new List<ParticleSystemsData> { data.effect});
	}

	[System.Serializable]
	public class Data : IApplyable{
		public float total = 20;
		public ParticleSystemsData effect{ get { return MParticleResources.Instance.healOnceParticles.data;} } 

		public IHasProgress Apply(PolygonGameObject picker) {
			picker.AddEffect (new HealOnce (this));
			return null;
		}
	}
}


public class HealOnCollision : DurationEffect {
    protected override eType etype { get { return eType.HealOnCollision; } }
    public override bool CanBeUpdatedWithSameEffect { get { return true; } }

    Data data;

    public HealOnCollision(Data data) : base(data) {
        this.data = data;
    }

    public override void SetHolder(PolygonGameObject holder) {
        base.SetHolder(holder);
        holder.OnCollision += Holder_OnCollision;
    }

    private void Holder_OnCollision(PolygonGameObject other, float dmgDealt) {
        if (!IsFinished()) {
            holder.Heal(dmgDealt * data.percent);
        }
    }

    public override void UpdateBy(TickableEffect sameEffect) {
        base.UpdateBy(sameEffect);
        var same = sameEffect as HealOnCollision;
        IncreaseTimeLeft(same.data.duration);
    }

    public override void OnExpired() {
        holder.OnCollision -= Holder_OnCollision;
    }

    [System.Serializable]
    public class Data : IHasDuration, IApplyable {
        public float duration = 4;
        public float iduration { get { return duration; } set { duration = value; } }
        public float percent = 0.2f;

        public IHasProgress Apply(PolygonGameObject picker) {
            var effect = new HealOnCollision(this);
            effect = picker.AddEffect(effect);
            return effect;
        }
    }
}