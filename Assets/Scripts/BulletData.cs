using UnityEngine;
using System.Collections;

public class BulletData
{
	public Vector2 direction;
	public float damage;
}

public class ShootPlace
{
	public Vector2[] vertices; //TODO: share mesh
	public float speed;
	public float travelDistance;
	public float damage;
	public Vector2 position;
	public Vector2 direction;
	public float fireInterval;
	public Color color;
	public float timeToNextShot = 0f;

	public static ShootPlace GetSpaceshipShootPlace()
	{
		ShootPlace shooter = new ShootPlace();

		float size = 0.3f;
		shooter.vertices = new Vector2[]
		{
			new Vector2(size,size),
			new Vector2(size,-size),
			new Vector2(-size,-size),
			new Vector2(-size,size),
		};
		shooter.speed = 30f;
		shooter.travelDistance = 40f;
		shooter.damage = 1f;
		shooter.position = new Vector2(2, 0); //TODO ship vertex
		shooter.direction =  new Vector2(1, 0);
		shooter.fireInterval = 0.3f;
		shooter.color = Color.red;

		return shooter;
	}

	public void Tick(float delta)
	{
		if(timeToNextShot > 0)
		{
			timeToNextShot -= delta;
		}
	}

	public bool ReadyToShoot()
	{
		return timeToNextShot <= 0;
	}

	public void ResetTime()
	{
		timeToNextShot = fireInterval;
	}
}


public class BulletCreator
{
	public static Bullet CreateBullet(Transform shooterTransform, ShootPlace shootPlace)
	{
		float angle = Math2d.GetRotation(shooterTransform.right);
		float cosA = Mathf.Cos(angle);
		float sinA = Mathf.Sin(angle);

		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(shootPlace.vertices, shootPlace.color);

		bullet.cacheTransform.position = shootPlace.position;
		bullet.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180); //TODO: my rotation?

		Vector2 rotatedDirection = Math2d.RotateVertex(shootPlace.direction, cosA, sinA);
		bullet.Init(rotatedDirection); //TODO: data

		bullet.cacheTransform.position += shooterTransform.position;
		bullet.gameObject.name = "bullet";
		
		return bullet;
	}
}
