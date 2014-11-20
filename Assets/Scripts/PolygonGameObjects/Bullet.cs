using UnityEngine;
using System.Collections;

public class Bullet : PolygonGameObject, IGotVelocity
{
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

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
	}
}
 