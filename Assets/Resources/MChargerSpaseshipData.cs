using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MChargerSpaseshipData : MSpaceshipData {
    public ChargerData chargerData;

    [Header("editor field")]
    [SerializeField]
    MSpaceshipData fillFrom;

    private void OnValidate() {
        if (fillFrom != null) {
            System.Type type = fillFrom.GetType();
            Component copy = this;
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(fillFrom));
            }

            fillFrom = null;
        }
    }

    public override SpaceShip Create(int layer) {
        return ObjectsCreator.CreateChargerSpaceship<SpaceShip>(this, layer);
    }

}

[System.Serializable]
public class ChargerData {
    public float speedMultiplier = 2f;
    public float thrustMultiplier = 2f;
}
