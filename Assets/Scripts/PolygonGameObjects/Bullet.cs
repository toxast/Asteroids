using UnityEngine;
using System.Collections;

public class Bullet : BulletBase
{
	private float startingSpeed;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(Vector3 direction, ShootPlace place)
	{
		base.Init (place);
		velocity = direction.normalized * place.speed;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
	}
}
 