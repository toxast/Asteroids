using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : PolygonGameObject
{
	public virtual void InitAsteroid(PhysicalData physical, RandomFloat pSpeed, RandomFloat pRotation)
	{
		InitPolygonGameObject (physical);
		InitRandomMovement (this, pSpeed, pRotation);
	}

	public static void InitRandomMovement(PolygonGameObject go, RandomFloat pSpeed, RandomFloat pRotation) {
		float speed = Random.Range(pSpeed.min, pSpeed.max);
		float a = Random.Range(0f, 359f) * Mathf.Deg2Rad;
		go.velocity = new Vector2(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed);
		go.rotation = Math2d.RandomSign() * Random.Range(pRotation.min, pRotation.max);
		go.position = new Vector3(go.position.x, go.position.y, Random.Range(-1f, -0.1f)); 
	}
}
