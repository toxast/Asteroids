using UnityEngine;
using System.Collections;

public class Bullet : PolygonGameObject
{
	private Vector3 speed; 
	private float startingSpeed;

	private float distanceTraveled;
	private float maxDistance;

	public float damage;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(Vector3 direction, ShootPlace place)
	{
		damage = place.damage;
		maxDistance = place.travelDistance;
		speed = direction.normalized * place.speed;
		distanceTraveled = 0;
	}

	public override void Tick(float delta)
	{
		Vector3 deltaDistance = speed*delta;
		cacheTransform.position += deltaDistance;

		distanceTraveled += deltaDistance.magnitude; //TODO: lifetime?
		if(distanceTraveled > maxDistance)
		{
			Destroy(gameObject);
		}
	}

	public Vector3 GetSpeed()
	{
		return speed;
	}
}
 