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
		if (IsFinished()) {
			foreach (var item in spawnedEffects) {
				GameObject.Destroy(item.gameObject);
			}
			spawnedEffects.Clear();
		} else {
			float burningArea = Mathf.Clamp(( 3f * currentDps * timeLeft )/ holder.fullHealth, 0.1f, 0.8f);
			int burningsCount = (int)Mathf.Max(3f, 3f*holder.polygon.R * burningArea);
			int diff = burningsCount - spawnedEffects.Count;
			if (diff != 0) {
				if (diff > 0) {
					for (int i = 0; i < diff; i++) {
						var effect = data.effect.Clone();
						effect.place.pos = holder.polygon.GetRandomAreaVertex();
						effect.overrideSize = UnityEngine.Random.Range (2f, Mathf.Min(4f, holder.polygon.R/2f));
						effect.overrideDelay = UnityEngine.Random.Range (0f, 2f);
						spawnedEffects.AddRange(holder.SetParticles(new List<ParticleSystemsData> { effect }));
					}
				} else {
					for (int i = 0; i < -diff; i++) {
						int last = spawnedEffects.Count - 1;
						GameObject.Destroy(spawnedEffects[last].gameObject);
						spawnedEffects.RemoveAt(last);
					}
				}
			}
		}
	}
}

//freeze with 1 should not change the object behaviour
//freeze with 2 should freeze the object 2 times
//freeze with 0.5 should unfreeze previous call with 2.
//freeze 0 will not be called, minimum is minFreeze (0.01f)
public interface IFreezble { 
    void Freeze(float multiplier); 
}

public class IceEffect : TickableEffect {
    const float minFreeze = 0.01f;
    protected override eType etype { get { return eType.Ice; } }
    public override bool CanBeUpdatedWithSameEffect { get { return true; } }

    Data data;
    IFreezble freezableHolder;
    float timeLeft;
    float currentFreezeAmount;

    public IceEffect(Data data) {
        this.data = data;
        timeLeft = data.duration;
        currentFreezeAmount = data.freezeAmount;
    }

    public override void SetHolder(PolygonGameObject holder) {
        base.SetHolder(holder);
        freezableHolder = holder as IFreezble;
        UpdateFreezeAount(currentFreezeAmount);
    }

    public override void Tick(float delta) {
        base.Tick(delta);
        if (!IsFinished()) {
            timeLeft -= delta;
            if (IsFinished()) {
                UpdateFreezeAount(0);
            }
        }
    }

    public override bool IsFinished() { return timeLeft <= 0; }

    public override void UpdateBy(TickableEffect sameEffect) {
        base.UpdateBy(sameEffect);
        IceEffect effect = sameEffect as IceEffect;
        //half goes to duration, half to amount
        float halfAmount = 0.5f * effect.data.duration * effect.data.freezeAmount;
        float newTotalAmount;
        newTotalAmount = currentFreezeAmount * timeLeft + halfAmount;
        timeLeft = newTotalAmount / currentFreezeAmount;

        newTotalAmount = currentFreezeAmount * timeLeft + halfAmount;
        currentFreezeAmount = newTotalAmount / timeLeft;
        UpdateFreezeAount(currentFreezeAmount);
    }

    private void UpdateFreezeAount(float freezeAmount) {
        float freeze01 = Mathf.Clamp01(freezeAmount / holder.mass);
        UpdateFreeze01(freeze01);
        UpdateIceEffectsCount(freeze01);
    }

    float lastMultiplier = 1f;
    private void UpdateFreeze01(float freeze01) {
        if (freezableHolder != null) {
            var lastMultiplierReverse = 1f / lastMultiplier;
            freezableHolder.Freeze(lastMultiplierReverse);
            float multipiler = 1f - Mathf.Clamp(freeze01, 0f, 1 - minFreeze);
            freezableHolder.Freeze(multipiler);
            lastMultiplier = multipiler;
        }
        //spaceshipHolder.MultiplyStability(lastMultiplierReverse);
        //spaceshipHolder.MultiplyThrust(lastMultiplierReverse);
        //spaceshipHolder.MultiplyTurnSpeed(lastMultiplierReverse);
    }

    

   

    const float oneIceEffectArea = 5 * 5;
    List<ParticleSystem> spawnedEffects = new List<ParticleSystem>();
    private void UpdateIceEffectsCount(float freeze01) {
        if (IsFinished()) {
            foreach (var item in spawnedEffects) {
                GameObject.Destroy(item.gameObject);
            }
            spawnedEffects.Clear();
        } else {
            int burningsCount = (int)(freeze01 * holder.polygon.area / oneIceEffectArea);
            int diff = burningsCount - spawnedEffects.Count;
            if (diff != 0) {
                if (diff > 0) {
                    for (int i = 0; i < diff; i++) {
                        var effect = data.effect.Clone();
                        effect.place.pos = holder.polygon.GetRandomAreaVertex();
                        effect.overrideDelay = UnityEngine.Random.Range(0f, 2f);
                        spawnedEffects.AddRange(holder.SetParticles(new List<ParticleSystemsData> { effect }));
                    }
                } else {
                    for (int i = 0; i < -diff; i++) {
                        int last = spawnedEffects.Count - 1;
                        GameObject.Destroy(spawnedEffects[last].gameObject);
                        spawnedEffects.RemoveAt(last);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class Data {
        public float duration = 3;
        public float freezeAmount = 20;
        public ParticleSystemsData effect;
    }
}

