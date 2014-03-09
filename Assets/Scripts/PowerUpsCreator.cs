using UnityEngine;
using System.Collections;

public class PowerUpsCreator 
{
	public event System.Action<PowerUp> PowerUpCreated;

	private float time2CreateLeft = float.MaxValue;
	private float minTimeGeneration;
	private float maxTimeGeneration;

	public PowerUpsCreator(float minTimeGeneration, float maxTimeGeneration)
	{
		this.minTimeGeneration = minTimeGeneration;
		this.maxTimeGeneration = maxTimeGeneration;
		time2CreateLeft = UnityEngine.Random.Range(minTimeGeneration, maxTimeGeneration);
	}

	public void Tick(float delta)
	{
		time2CreateLeft -= delta;
		if (time2CreateLeft <= 0) 
		{
			time2CreateLeft = UnityEngine.Random.Range(minTimeGeneration, maxTimeGeneration);

			if(PowerUpCreated != null)
			{
				PowerUp powerup = CreatePowerUp();
				PowerUpCreated(powerup);
			}
		}
	}

	private PowerUp CreatePowerUp()
	{
		EffectType type = (EffectType)UnityEngine.Random.Range ((int)EffectType.Min + 1, (int)EffectType.Max);

		Color color = Color.yellow;
		if (type == EffectType.SlowAsteroids) 
		{
			color = Color.cyan;
		}
		else if(type == EffectType.IncreasedShootingSpeed)
		{
			color = Color.magenta;
		}

		Vector2[] halfvertices = new Vector2[]
		{
			new Vector2 (1.2f, 0f),
			new Vector2 (0.5f, -1f),
			new Vector2 (-0.5f, -1f),
			new Vector2 (-1.2f, 0f),
		};
		
		//TODO: optimize
		Vector2[] vertices = PolygonCreator.GetCompleteVertexes(halfvertices, 1.2f).ToArray();
		
		Polygon polygon;
		GameObject polygonGo;
		PolygonCreator.CreatePolygonGOByMassCenter(vertices, color, out polygon, out polygonGo);
		polygonGo.name = "powerup";
		
		PowerUp powerup = polygonGo.AddComponent<PowerUp>();
		powerup.Init(polygon, type);
		
		return powerup;
	}
}
