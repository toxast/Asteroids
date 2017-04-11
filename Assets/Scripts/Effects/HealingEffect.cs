using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingEffect : DurationEffect {
	protected override eType etype { get { return eType.HealOT; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;
	float currentHps;

	List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();

    public HealingEffect(Data data) : base(data) {
        this.data = data;
        currentHps = data.total / data.duration;
    }

    public override void SetHolder (PolygonGameObject holder){
		base.SetHolder (holder);
		AddEffects ();
	}

	public override void Tick (float delta) {
        if (!IsFinished()) {
            holder.Heal(currentHps * delta);
        }
        base.Tick(delta);
	}

	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as HealingEffect;
		float addTime = (same.data.total / currentHps);
		IncreaseTimeLeft (addTime);
	}

    public override void OnExpired() {
        DestroyEffects();
    }

    void AddEffects() {
        int amount = (int)(3 * Mathf.Pow(holder.polygon.R, 0.8f));
        for (int i = 0; i < amount; i++) {
            var effect = data.effect.Clone();
            effect.place.pos = holder.polygon.GetRandomAreaVertex();
            spawnedEffects.AddRange(holder.AddParticles(new List<ParticleSystemsData> { effect }));
        }
    }

    void DestroyEffects() {
        if (IsFinished()) {
            foreach (var item in spawnedEffects) {
                item.Stop();
                GameObject.Destroy(item.gameObject, 5f);
            }
            spawnedEffects.Clear();
        }
    }

    [System.Serializable]
	public class Data: IHasDuration, IApplyable {
		public float duration = 4;
		public float iduration{get {return duration;} set{duration = value;}}
		public float total = 20;
		public ParticleSystemsData effect{ get { return MParticleResources.Instance.healingParticles.data;} } 

		public IHasProgress Apply(PolygonGameObject picker) {
			var effect = new HealingEffect (this);
			effect = picker.AddEffect (effect);
			return effect;
		}
	}
}




