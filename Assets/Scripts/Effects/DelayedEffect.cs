using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEffect : TickableEffect {
    protected override eType etype { get { return eType.Delayed; } }
    public override bool CanBeUpdatedWithSameEffect { get { return false; } }
    DelayedEffect.Data data;
    bool applied = false;
    float timeLeft;

    public DelayedEffect(DelayedEffect.Data data) {
        this.data = data;
        timeLeft = data.delay;
    }

    public override void Tick(float delta) {
        base.Tick(delta);
        timeLeft -= delta;
        if (!applied && timeLeft <= 0) {
            applied = true;
            data.effect.Apply(holder);
        }
    }

    public override bool IsFinished() { return applied; }

    [System.Serializable]
    public class Data : IApplyable {
        public MEffectData effect;
        public float delay = 3f;
        public IHasProgress Apply(PolygonGameObject picker) {
            picker.AddEffect(new DelayedEffect(this));
            return null;
        }
    }
}
