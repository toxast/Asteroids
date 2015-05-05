using UnityEngine;
using System.Collections;

public class PowerUp : PolygonGameObject 
{
	public EffectType effect;
	public float lived = 0;

	public void InitAsteroid(EffectType effect)
	{
		this.effect = effect;
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		lived += delta;
	}
}

public enum EffectType
{
	Min,
	SlowAsteroids,
	IncreasedShootingSpeed,
	PenetrationBullet,
	Max,
}
