using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEarthSpaceshipData : MSpaceshipData
{
	public int maxShields = 8;
	public MAsteroidData asteroidData;

	public override SpaceShip Create(int layer)
	{
		return ObjectsCreator.CreateEarthSpaceship<SpaceShip>(this, layer);
	}

	[Header("editor field")]
	[SerializeField] MSpaceshipData fillFrom;
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
}
