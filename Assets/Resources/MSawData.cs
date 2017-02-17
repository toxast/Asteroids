using UnityEngine;
using System.Collections;

[System.Serializable]
public class MSawData : MSpawnDataBase, IGotShape
{
	public float prepareTime = 2f;

	public float chargeSpeed = 50f;
	public float chargeRotation = 300f;
	public float chargeDuration = 3f;

	public float slowingDuration = 2f;
	public AccuracyData accuracy;
	public Vector2[] vertices;
	public Vector2[] iverts {get {return vertices;} set{vertices = value;}}

	public Color color = new Color (0.5f, 0.5f, 0.5f);
	public int reward = 0;
	public PhysicalData physical;
	public RandomFloat speed;
	public RandomFloat rotation;
	public RandomFloat size;
	public RandomFloat spikeSize;
	public RandomInt spikesCount;

	public override PolygonGameObject Create(int layer)
	{
		return ObjectsCreator.CreateSawEnemy(this, layer);
	}
}
