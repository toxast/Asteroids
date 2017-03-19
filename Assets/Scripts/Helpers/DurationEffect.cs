using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DurationEffect : TickableEffect, IHasProgress {

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

	public float iprogress{ get { return Mathf.Clamp01(1f - timeLeft / duration);}}
 
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
