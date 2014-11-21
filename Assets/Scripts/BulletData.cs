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
	public float lifeTime;
	public float damage;
	public Vector2 position;
	public Vector2 direction;
	public float fireInterval;
	public Color color;
	public float timeToNextShot = 0f;

	public static ShootPlace GetSpaceshipShootPlace()
	{
		ShootPlace shooter = new ShootPlace();

		shooter.vertices = PolygonCreator.GetRectShape(0.4f, 0.2f);
		shooter.speed = 30f;
		shooter.lifeTime = 2f;
		shooter.damage = 3f;
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

	public bool ShootIfReady()
	{
		if(ReadyToShoot())
		{
			ResetTime();
			return true;
		}
		else
		{
			return false;
		}
	}
}


public class BulletCreator
{

	public static Vector2[] missileVertices = PolygonCreator.GetCompleteVertexes(
		new Vector2[]
		{
		new Vector2(2f, 0f),
		new Vector2(1.5f, -0.2f),
		new Vector2(0.3f, -0.15f),
		new Vector2(0f, -0.35f),
	}
	, 1f).ToArray();

	public static Bullet CreateBullet(Transform shooterTransform, ShootPlace shootPlace)
	{
		Bullet bullet = PolygonCreator.CreatePolygonGOByMassCenter<Bullet>(shootPlace.vertices, shootPlace.color);

		float angle = PositionOnShooterPlace (bullet, shooterTransform, shootPlace);

		bullet.gameObject.name = "bullet";

		Vector2 rotatedDirection = Math2d.RotateVertex(shootPlace.direction, angle);
		bullet.Init(rotatedDirection, shootPlace); //TODO: data

		return bullet;
	}


	public static Missile CreateMissile(GameObject target, Transform shooterTransform, ShootPlace shootPlace)
	{
		Missile missile = PolygonCreator.CreatePolygonGOByMassCenter<Missile>(missileVertices, shootPlace.color);
		
		PositionOnShooterPlace (missile, shooterTransform, shootPlace);
		
		missile.gameObject.name = "missile";
		
		missile.Init(target, shootPlace); 
		
		return missile;
	}

	private static float PositionOnShooterPlace(PolygonGameObject go, Transform shooterTransform, ShootPlace shootPlace)
	{
		float angle = Math2d.GetRotation(shooterTransform.right);
		go.cacheTransform.position = shootPlace.position;
		go.cacheTransform.RotateAround(Vector3.zero, Vector3.back, -angle/Math2d.PIdiv180);
		go.cacheTransform.position += shooterTransform.position;
		return angle;
	}
}
