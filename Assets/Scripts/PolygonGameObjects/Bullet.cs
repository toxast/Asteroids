using UnityEngine;
using System.Collections;

public class Bullet : BulletBase
{
	private float startingSpeed;

	public void Init(float speed, float dmg, float lifetime)
	{
		base.Init (dmg, lifetime);
		velocity = cacheTransform.right * speed;
	}


}
 