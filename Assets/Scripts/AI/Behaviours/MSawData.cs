using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MSawData : MSpawnDataBase, IGotShape
{
	public float prepareTime = 2f;
	public float chargeRotation = 300f;
	public RandomFloat chargeSpeed = new RandomFloat(30, 40);
	public RandomFloat chargeDuration = new RandomFloat(2f, 3.5f);
	public float slowingDuration = 2f;

	public float chanceForceStopWhenMissed = 0.5f;
	public float chanceContinueChargeUntilMiss = 0.5f;

	public AccuracyData accuracy;
	public Vector2[] vertices;
	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	public Color color = new Color (0.5f, 0.5f, 0.5f);
	public PhysicalData physical;
	public RandomFloat speed;
	public RandomFloat rotation;

	public MGunsShow gunsShowChargeEffect;

	public bool useInvisibilityBeh = false;
	public InvisibilityComponent.Data invisData;
	public float thrust = 0;
	public float stability = 0;

    private void OnValidate() {
        if (vertices.Length < 3) {
            Debug.LogError("no verts on saw: " + name);
        }
    }

    protected override PolygonGameObject CreateInternal(int layer)
	{
		return ObjectsCreator.CreateSawEnemy(this, layer);
	}
}
