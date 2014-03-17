using UnityEngine;
using System.Collections;

public class Bullet : PolygonGameObject
{
	private Vector3 speed; 
	private float startingSpeed = 30f;

	private float distanceTraveledSqr;
	private float maxDistance = 6f;
	private float maxDistanceSqr;

	public float damage;

	void Awake () 
	{
		cacheTransform = transform;
	}

	public void Init(Vector3 direction)
	{
		this.speed = direction.normalized * startingSpeed;
		distanceTraveledSqr = 0;
		maxDistanceSqr = maxDistance*maxDistance;

		damage = 1f;
	}

	public override void Tick(float delta)
	{
		Vector3 deltaDistance = speed*delta;
		cacheTransform.position += deltaDistance;

		distanceTraveledSqr += deltaDistance.sqrMagnitude;
		if(distanceTraveledSqr > maxDistanceSqr)
		{
			Destroy(gameObject);
		}
	}

	public Vector3 GetSpeed()
	{
		return speed;
	}
}
 