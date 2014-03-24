﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : PolygonGameObject, IGotVelocity, IGotRotation
{
	[SerializeField] public Vector3 velocity;
	[SerializeField] public float rotation;


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
		cacheTransform.position += velocity * delta;
		cacheTransform.Rotate(Vector3.back, rotation*delta);
	}

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
	}

}