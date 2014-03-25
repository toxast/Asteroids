using UnityEngine;
using System.Collections;

public class Bullet : PolygonGameObject
{
	public Vector3 speed; 
	private float startingSpeed;

	private float lifeTime;

	public float damage;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(Vector3 direction, ShootPlace place)
	{
		damage = place.damage;
		lifeTime = place.lifeTime;
		speed = direction.normalized * place.speed;
	}

	public override void Tick(float delta)
	{
		Vector3 deltaDistance = speed*delta;
		cacheTransform.position += deltaDistance;

		lifeTime -= delta; 
		if(lifeTime < 0)
		{
			Destroy(gameObject);
		}
	}

	public Vector3 GetSpeed()
	{
		return speed;
	}
}
 