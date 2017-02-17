using UnityEngine;
using System.Collections;

public class MSpikyData : MSpawnDataBase
{
	public AccuracyData accuracy;
	public float spikeVelocity = 45f;
	public float growSpeed = 0.1f;
	public float chanceShootSpikeAtDeath = 0.5f;
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
		return ObjectsCreator.CreateSpikyAsteroid(this, layer);
	}
}
