using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MChargerSpaseshipData : MSpaceshipData {

    [Header ("charger")]
    public PhysicalChangesEffect.Data chargeEffect;
    public bool shootWhenCharge = false;
    public float aimOffset = 0; //aim at position = traget.pos + traget.velocity.norm * aimOffset


    [Header("editor field")]
    [SerializeField]
    MSpaceshipData fillFrom;

	protected override void OnValidate() {
		base.OnValidate ();
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

	protected override PolygonGameObject CreateInternal(int layer) {
        return ObjectsCreator.CreateChargerSpaceship<SpaceShip>(this, layer);
    }

}

