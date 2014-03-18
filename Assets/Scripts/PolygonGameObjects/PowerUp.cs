using UnityEngine;
using System.Collections;

public class PowerUp : PolygonGameObject 
{
	public EffectType effect;

	public void Init(EffectType effect)
	{
		this.effect = effect;
	}
}

public enum EffectType
{
	Min,
	SlowAsteroids,
	IncreasedShootingSpeed,
	Max,
}
