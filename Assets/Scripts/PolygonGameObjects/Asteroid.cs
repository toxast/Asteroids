using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : PolygonGameObject
{
	public virtual void Init()
	{
		float speed = Random.Range(2f, 10f);
		float a = Random.Range(0f, 359f) * Mathf.Deg2Rad;
		velocity = new Vector2(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed);
		rotation = -Random.Range(30f, 90f);

		position = new Vector3(position.x, position.y, Random.Range(-1f, -0.1f));
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
	}
}
