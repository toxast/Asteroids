using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITickable
{
	void Tick (float delta);
}

public abstract class TickableEffect : ITickable 
{
	protected abstract eType etype{ get;}
	protected PolygonGameObject holder;

	public virtual void SetHolder(PolygonGameObject holder) {
		this.holder = holder;
	}
	public virtual void Tick(float delta) { }

	public virtual bool IsFinished(){
		return false;
	}

	public virtual bool CanBeUpdatedWithSameEffect{get{ return false;}}
	public bool IsTheSameEffect(TickableEffect effect){ return effect.etype == this.etype; }
	public virtual void UpdateBy(TickableEffect sameEffect) { }

	protected enum eType {
		None = 0,
		GravityShield,
		Burning,
	}
}


public abstract class DOTEffect : TickableEffect 
{
    protected Data data;
	protected float timeLeft;
	protected float currentDps;
    
	public DOTEffect(Data data) {
		this.data = data;
		timeLeft = data.duration;
		currentDps = data.dps;
	}

	public override void Tick (float delta) {
		base.Tick (delta);
		if (!IsFinished ()) {
			timeLeft -= delta;
			holder.Hit (currentDps * delta);
		}
    }

	public override bool IsFinished() {
		return timeLeft <= 0;
	}

	/// <summary>
	/// repalces current dps with the maximum dps, adjusts the duration so the total damage is preserved
	/// duration is cut by the effect with the max dps
	/// </summary>
	public override void UpdateBy (TickableEffect sameEffect) {
		base.UpdateBy (sameEffect);
		var same = sameEffect as DOTEffect;

		float maxDps;
		float maxDuration;
		if (currentDps > same.data.dps) {
			maxDps = currentDps;
			maxDuration = data.maxBuildUpDuration;
        } else {
			maxDps = same.data.dps;
			maxDuration = same.data.maxBuildUpDuration;
            data = same.data;
        }

        float totalDamageToBeDone = timeLeft * currentDps + same.data.dps * same.data.duration;

		currentDps = maxDps;
		timeLeft = Mathf.Min(totalDamageToBeDone / maxDps, maxDuration);
    }

	[System.Serializable]
	public class Data{
		public float duration = 1;
		public float dps = 3;
		public float maxBuildUpDuration = 5;
        public ParticleSystemsData effect;
	}
}

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
						effect.overrideDelay = UnityEngine.Random.Range (0f, 1f);
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
