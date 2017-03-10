using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDumbHitterData : MSpaceshipData {

    [Header("editor field")]
    [SerializeField]
    MSpaceshipData fillFrom;

    private void OnValidate() {
        if (fillFrom != null) {
            System.Type type = fillFrom.GetType();
            Component copy = this;
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(fillFrom));
            }
            fillFrom = null;
        }
    }

    protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateDumbHitterSpaceship<SpaceShip>(this, layer);
    }
}

