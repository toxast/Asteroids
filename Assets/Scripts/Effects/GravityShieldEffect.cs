using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityShieldEffect : DurationEffect {
	public override bool CanBeUpdatedWithSameEffect { get { return true; }	}
	protected override eType etype { get { return eType.GravityShield; } }
	Data data;
	float currentForce;
	List<PolygonGameObject> gobjects;
	List<PolygonGameObject> bullets;
	List<ParticleSystem> spawnedEffects = new List<ParticleSystem> ();

	public GravityShieldEffect(Data data) : base(data){
		this.data = data;
		currentForce = data.force;
		gobjects = Singleton<Main>.inst.gObjects;
		bullets = Singleton<Main>.inst.bullets;
	}

	public override void SetHolder (PolygonGameObject holder) {
		base.SetHolder (holder);
        var particles = data.particles.Clone();
        particles.ForEach(e => e.overrideSize = data.range * 2f);
        spawnedEffects = holder.AddParticles (particles);
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			var objectsAroundData = ExplosionData.CollectData (holder.position, data.range, gobjects, holder.collisions);
			var bulletsAroundData = ExplosionData.CollectData (holder.position, data.range, bullets, holder.collisions);
			new ForceExplosion (objectsAroundData, holder.position, delta * currentForce, data.distanceMatters, data.massMatters);
			new ForceExplosion (bulletsAroundData, holder.position, delta * currentForce, data.distanceMatters, data.massMatters);
		}
	}

	public override void OnExpired () {
		foreach (var effect in spawnedEffects) {
			effect.Stop ();
		}
	}

	/// <summary>
	/// repalces current dps with the maximum dps, adjusts the duration so the total damage is preserved
	/// duration is cut by the effect with the max dps
	/// </summary>
	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as GravityShieldEffect;
		timeLeft += same.data.duration;
		currentForce = Mathf.Max (currentForce, same.data.force);
	}

	[System.Serializable]
	public class Data: IHasDuration, IApplyable{
		public float duration = 30;
		public float range = 20;
		public float force = 20;
		public bool distanceMatters = false;
		public bool massMatters = false;
		public float iduration{get {return duration;} set{duration = value;}}
		public List<ParticleSystemsData> particles;
		public void Apply(PolygonGameObject picker) {
			picker.AddEffect (new GravityShieldEffect (this));
		}
	}
}