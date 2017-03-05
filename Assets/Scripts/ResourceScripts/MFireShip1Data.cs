using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MFireShip1Data : MSpaceshipData
{
	[Header ("specific")]
	public int startFireballs = 2;
	public int fireballCount = 3;
	public float respawnFireballDuration = 2;
	public float radius = 15f;
    public float shootInterval = 0.5f;
    public float randomizeAimAngle = 15f;
	public float overrideMaxComfortDist = -1;
	public MFireballGunData fireballData;

	protected override PolygonGameObject CreateInternal(int layer) {
		return ObjectsCreator.CreateFireSpaceship1<SpaceShip>(this, layer);
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
