using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSuicideBombSpaceshipData : MSpaceshipData {
	public float delayBeforeExplode = 1f;
	public ParticleSystemsData explodeTimerEffect;

    [Header("editor field")]
    [SerializeField]
    MSpaceshipData fillFrom;

    private void OnValidate() {
        if (fillFrom != null) {
            System.Type type = this.GetType();
            Component copy = this;
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(fillFrom));
            }

            fillFrom = null;
        }
    }

	protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateSuisideBombSpaceship<SpaceShip>(this, layer);
    }

}

