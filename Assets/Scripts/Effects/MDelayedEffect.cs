using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDelayedEffect : MEffectData {
    DelayedEffect.Data data;
    public override IHasProgress Apply(PolygonGameObject picker) {
        return data.Apply(picker);
    }
}