using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : PolygonGameObject
{
	public void Init()
	{
		float speed = Random.Range(2f, 10f);
		float a = Random.Range(0f, 359f) * Math2d.PIdiv180;
		velocity = new Vector3(Mathf.Cos(a)*speed, Mathf.Sin(a)*speed, 0f);
		rotation = -Random.Range(30f, 90f);
		
		cacheTransform.position = new Vector3(cacheTransform.position.x, cacheTransform.position.y, Random.Range(-1f, -0.1f));
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
	}
}
