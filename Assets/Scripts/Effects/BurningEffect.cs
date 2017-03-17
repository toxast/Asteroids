using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningEffect : DOTEffect {
	protected override eType etype { get { return eType.Burning; } }
	List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();

	public BurningEffect (Data data) : base (data){}

	public override bool CanBeUpdatedWithSameEffect{get{ return true;}}

	public override void Tick(float delta) {
		base.Tick(delta);
		UpdateFlamesCount();
	}

	public override void UpdateBy(TickableEffect sameEffect) {
		base.UpdateBy(sameEffect);
		UpdateFlamesCount();
	}

	private void UpdateFlamesCount() {
		int burningsCount = 0;
		if (!IsFinished()) {
			float burningArea = Mathf.Clamp(( 3f * currentDps * timeLeft )/ holder.fullHealth, 0.1f, 0.8f);
			burningsCount = (int)Mathf.Max(3f, 3f*holder.polygon.R * burningArea);
		} 
		int diff = burningsCount - spawnedEffects.Count;
		if (diff != 0) {
			if (diff > 0) {
				for (int i = 0; i < diff; i++) {
					var effect = (data as Data).effect.Clone();
					effect.place.pos = holder.polygon.GetRandomAreaVertex();
					effect.overrideSize = UnityEngine.Random.Range (2f, Mathf.Min(4f, holder.polygon.R/2f));
					effect.overrideDelay = UnityEngine.Random.Range (0f, 2f);
					spawnedEffects.AddRange(holder.AddParticles(new List<ParticleSystemsData> { effect }));
				}
			} else {
				for (int i = 0; i < -diff; i++) {
					int last = spawnedEffects.Count - 1;
					spawnedEffects [last].Stop ();
					GameObject.Destroy(spawnedEffects[last].gameObject, 5f);
					spawnedEffects.RemoveAt(last);
				}
			}
		}
	}

	[System.Serializable]
	new public class Data : DOTEffect.Data{
		public ParticleSystemsData effect { get { return MParticleResources.Instance.burnParticles.data;} } 
	}
}


