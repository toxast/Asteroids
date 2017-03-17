using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public virtual void HandleHolderDestroying(){ }

	protected enum eType {
		None = 0,
		GravityShield,
		Burning,
		Ice,
		IceWave,
        GunsShow,
		PhysicalChanges,
		SpawnBackup,
		HeavyBullet,
		ExtraGuns,
		RotatingObjectsShield,
		KeepRotation,
		HealOT,
		HealOnce,
	}
}

public abstract class DurationEffect : TickableEffect {

    protected float duration;
    protected float timeLeft;

    public DurationEffect(IHasDuration durable) : this(durable.iduration) { }

    public DurationEffect(float duration) {
        if (duration <= 0) {
            Debug.LogError(this.GetType().ToString() + " wrong effect duration: " + duration);
        }
        this.duration = duration;
        timeLeft = duration;
    }

    public float Progress() {
        return timeLeft / duration;
    }

    public override bool IsFinished() {
        return timeLeft <= 0;
    }

    public override void Tick(float delta) {
        base.Tick(delta);
        if (!IsFinished()) {
            timeLeft -= delta;
            if (IsFinished()) {
                OnExpired();
            }
        }
    }
    public abstract void OnExpired();
}

