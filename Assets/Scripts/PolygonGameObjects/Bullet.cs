using UnityEngine;
using System.Collections;

public class Bullet : BulletBase
{
	private float startingSpeed;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(float speed, float dmg, float lifetime)
	{
		base.Init (dmg, lifetime);
		velocity = cacheTransform.right * speed;
	}

	public override void Tick(float delta)
	{
		base.Tick (delta);
	}
}
 