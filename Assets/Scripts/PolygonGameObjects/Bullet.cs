﻿using UnityEngine;
using System.Collections;

public class Bullet : BulletBase
{
	private float startingSpeed;

	private float lifeTime;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(Vector3 direction, ShootPlace place)
	{
		damage = place.damage;
		lifeTime = place.lifeTime;
		velocity = direction.normalized * place.speed;
	}

	public override void Tick(float delta)
	{
		Vector3 deltaDistance = velocity*delta;
		cacheTransform.position += deltaDistance;

		lifeTime -= delta; 
		if(lifeTime < 0)
		{
			Destroy(gameObject);
		}
	}
}
 