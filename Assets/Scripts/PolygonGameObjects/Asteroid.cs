using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : PolygonGameObject
{
	public virtual void InitAsteroid(float density, float healthModifier, RandomFloat pSpeed, RandomFloat pRotation)
	{
		InitPolygonGameObject (density, healthModifier);
		float speed = Random.Range(pSpeed.min, pSpeed.max);
		float a = Random.Range(0f, 359f) * Mathf.Deg2Rad;
		velocity = new Vector2(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed);
		rotation = Mathf.Sign(Random.Range(-1f, 1f)) * Random.Range(pRotation.min, pRotation.max);
		position = new Vector3(position.x, position.y, Random.Range(-1f, -0.1f));
	}
}
