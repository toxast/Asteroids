using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//freeze with 1 should not change the object behaviour
//freeze with 2 should freeze the object 2 times
//freeze with 0.5 should unfreeze previous call with 2.
//freeze 0 will not be called, minimum is minFreeze (0.01f)
public class IceEffect : TickableEffect {
    const float minFreeze = 0.01f;
    protected override eType etype { get { return eType.Ice; } }
    public override bool CanBeUpdatedWithSameEffect { get { return true; } }

	float iceEffectSize;
    Data data;
    float timeLeft;
    float currentFreezeAmount;

    public IceEffect(Data data) {
        this.data = data;
        timeLeft = data.duration;
        currentFreezeAmount = data.freezeAmount;
    }

    public override void SetHolder(PolygonGameObject holder) {
        base.SetHolder(holder);
		iceEffectSize = Mathf.Clamp(Mathf.Pow (holder.polygon.R, 0.5f), 1, 4);
        UpdateFreezeAmount(currentFreezeAmount);
    }

    public override void Tick(float delta) {
        base.Tick(delta);
        if (!IsFinished()) {
            timeLeft -= delta;
            if (IsFinished()) {
                UpdateFreezeAmount(0);
            }
        }
    }

    public override bool IsFinished() { return timeLeft <= 0; }

    public override void UpdateBy(TickableEffect sameEffect) {
        base.UpdateBy(sameEffect);
        IceEffect effect = sameEffect as IceEffect;
        //w goes to duration, 1-w to amount
        float incomingFreeze = effect.data.duration * effect.data.freezeAmount;
		float weightTime = (effect.data.duration / timeLeft);
		float weightAmount = (effect.data.freezeAmount / currentFreezeAmount);
		weightTime *= weightTime;
		weightAmount *= weightAmount;
		float addToAmount = incomingFreeze * (weightAmount / (weightTime + weightAmount));
		float addToTime = incomingFreeze - addToAmount;
		float oldAmount = currentFreezeAmount;
		float oldTime = timeLeft;
		float newTotalFreeze;
		newTotalFreeze = currentFreezeAmount * timeLeft + addToTime;
        timeLeft = newTotalFreeze / currentFreezeAmount;
		newTotalFreeze = currentFreezeAmount * timeLeft + addToAmount;
        currentFreezeAmount = newTotalFreeze / timeLeft;
        UpdateFreezeAmount(currentFreezeAmount);
		//Debug.LogWarning ("add to time " + addToTime + " add to amount " + addToAmount);
		//Debug.LogWarning ("time " + oldTime + "->" + timeLeft + " amount: " + oldAmount + "->" + currentFreezeAmount);
    }

    private void UpdateFreezeAmount(float freezeAmount) {
		float freeze01 = Mathf.Clamp01(freezeAmount / holder.massSqrt07);
		Debug.LogWarning (holder.name + " freeze01 " + freeze01 + " timeLeft " + timeLeft);
        UpdateFreeze01(freeze01);
        UpdateIceEffectsCount(freeze01);
    }

    float lastMultiplier = 1f;
    private void UpdateFreeze01(float freeze01) {
        if (holder != null) {
            var lastMultiplierReverse = 1f / lastMultiplier;
            holder.Freeze(lastMultiplierReverse);
            float multipiler = 1f - Mathf.Clamp(freeze01, 0f, 1 - minFreeze);
            holder.Freeze(multipiler);
            lastMultiplier = multipiler;
        }
    }

    List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();
    private void UpdateIceEffectsCount(float freeze01) {
		int iceEffectsCount = 0;
		if (!IsFinished ()) {
			iceEffectsCount =  Mathf.CeilToInt(2.5f * freeze01 * Mathf.Pow(holder.polygon.area, 0.8f) / Mathf.Pow(iceEffectSize, 1.3f));
		}
		//Debug.LogWarning ("area " + holder.polygon.area  + " " + iceEffectSize);
		//Debug.LogWarning ("freeze01 " + freeze01 + " iceEffectsCount " + iceEffectsCount);
        int diff = iceEffectsCount - spawnedEffects.Count;
        if (diff != 0) {
            if (diff > 0) {
                for (int i = 0; i < diff; i++) {
                    var effect = data.effect.Clone();
                    effect.place.pos = holder.polygon.GetRandomAreaVertex();
                    effect.overrideDelay = UnityEngine.Random.Range(0f, 2f);
					effect.overrideSize = iceEffectSize * UnityEngine.Random.Range (0.8f, 1.2f);
                    spawnedEffects.AddRange(holder.AddParticles(new List<ParticleSystemsData> { effect }));
                }
            } else {
                for (int i = 0; i < -diff; i++) {
                    int last = spawnedEffects.Count - 1;
					spawnedEffects [last].Stop ();
                    GameObject.Destroy(spawnedEffects[last].gameObject, 5);
                    spawnedEffects.RemoveAt(last);
                }
            }
        }
    }

    [System.Serializable]
	public class Data : IClonable<Data> {
        public float duration = 0;
        public float freezeAmount = 0;
		public ParticleSystemsData effect{ get { return MParticleResources.Instance.iceParticles.data;} } 

		public bool Initialized() {
			return duration > 0 && freezeAmount > 0;
		}

		public Data Clone()
		{
			return Clone (1);
		}

		public Data Clone(float multiplier)
		{
			Data r = new Data ();
			float sqrt = Mathf.Sqrt (multiplier);
			r.duration = duration * sqrt;
			r.freezeAmount = freezeAmount * sqrt;
			return r;
		}
    }
}
