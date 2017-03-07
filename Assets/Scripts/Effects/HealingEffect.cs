using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingEffect : TickableEffect{
	protected override eType etype { get { return eType.HealOT; } }
	public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	Data data;
	float timeLeft;
	float currentHps;

	List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();

	public override void SetHolder (PolygonGameObject holder){
		base.SetHolder (holder);
		AddEffects ();
	}

	void AddEffects(){
		int amount= (int)(3*Mathf.Pow(holder.polygon.R, 0.8f));
		for (int i = 0; i < amount; i++) {
			var effect = data.effect.Clone();
			effect.place.pos = holder.polygon.GetRandomAreaVertex();
			spawnedEffects.AddRange(holder.SetParticles(new List<ParticleSystemsData> { effect }));
		}
	}

	void DestroyEffects(){
		if (IsFinished()) {
			foreach (var item in spawnedEffects) {
				item.Stop ();
				GameObject.Destroy(item.gameObject, 5f);
			}
			spawnedEffects.Clear();
		}
	}

	public HealingEffect(Data data) {
		if (data.duration <= 0) {
			Debug.LogError ("wrong HOT effect duration: " + data.duration);
		}

		this.data = data;
		timeLeft = data.duration;
		currentHps = data.total/ data.duration;
	}

	public override void Tick (float delta) {
		if (!IsFinished ()) {
			timeLeft -= delta;
			ActionFunc(currentHps * delta);
			if (IsFinished ()) {
				DestroyEffects ();
			}
		}
	}

	void ActionFunc(float amount){
		holder.Heal (amount);
	}

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as HealingEffect;
		timeLeft += (same.data.total / currentHps);
	}

	[System.Serializable]
	public class Data{
		public float duration = 4;
		public float total = 20;
		public ParticleSystemsData effect;
	}
}



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
	public class Data{
		public float total = 20;
		public ParticleSystemsData effect;
	}
}