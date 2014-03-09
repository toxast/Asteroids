using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour 
{
	public Transform cacheTransform;
	public Polygon polygon;

	public EffectType effect;

	public void Init(Polygon polygon, EffectType effect)
	{
		this.polygon = polygon;
		cacheTransform = transform;
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
