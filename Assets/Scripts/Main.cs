﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	SpaceShip spaceship;
	List <Asteroid> asteroids = new List<Asteroid>();
	List <Bullet> bullets = new List<Bullet>();


	private float DestroyTreshold = 2.5f;

	public Bullet bulletPrefab;

	Rect bounds;

	void Start()
	{
		CalculateBounds();

		CreateSpaceShip();
		 
		int asteroidsNum = 10;
		for (int i = 0; i < asteroidsNum; i++) 
		{
			Asteroid asteroid = CreateAsteroid();
			float angle = (Random.Range(0f,359f) * Mathf.PI) / 180f;
			float len = UnityEngine.Random.Range(spaceship.polygon.R + 2 * asteroid.polygon.R, bounds.yMax);
			asteroid.cacheTransform.position =  new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, 0);
			asteroids.Add(asteroid);
		}
	}

	private void CreateSpaceShip()
	{
		GameObject spaceShipGo;
		Polygon spaceshipPoly;
		CreatePolygonByMassCenter(SpaceshipsData.fastSpaceshipVertices, Color.blue, out spaceshipPoly, out spaceShipGo);
		spaceship = spaceShipGo.AddComponent<SpaceShip>();
		spaceship.Init(spaceshipPoly);
		spaceship.FireEvent += OnFire;
		spaceShipGo.name = "Spaceship";
	}
	
	private void CalculateBounds()
	{
		float height = 2*Camera.main.camera.orthographicSize;  
		float width = height * Screen.width / Screen.height;
		bounds = new Rect(-width/2f, -height/2f, width, height);
	}

	void OnFire ()
	{
		Bullet bullet = CreateBullet();

		for (int i = 0; i < bullets.Count; i++) 
		{
			if(bullets[i] == null)
			{
				bullets[i] = bullet;
				return;
			}
		}

		bullets.Add(bullet);
	}

	private Bullet CreateBullet()
	{
		float size = 0.3f;
		Vector2[] bulletVertices = new Vector2[]
		{
			new Vector2(size,size),
			new Vector2(size,-size),
			new Vector2(-size,-size),
			new Vector2(-size,size),
		};
		Polygon bulletPoly;
		GameObject bulletObj;
		CreatePolygonByMassCenter(bulletVertices, Color.red, out bulletPoly, out bulletObj);
		Bullet bullet = bulletObj.AddComponent<Bullet>();
		bullet.Init(bulletPoly, spaceship.cacheTransform.right);
		bullet.cacheTransform.position = spaceship.cacheTransform.position + spaceship.cacheTransform.right;
		bulletObj.name = "bullet";

		return bullet;
	}

	void Update()
	{
		spaceship.Tick(Time.deltaTime);
		CheckBounds(spaceship.cacheTransform, spaceship.polygon.R);

		for (int i = 0; i < asteroids.Count ; i++)
		{
			asteroids[i].Tick(Time.deltaTime);
			CheckBounds(asteroids[i].cacheTransform, asteroids[i].polygon.R);
		}

		for (int i = 0; i < bullets.Count; i++)
		{
			if(bullets[i] == null)
				continue;

			bullets[i].Tick(Time.deltaTime);
			CheckBounds(bullets[i].cacheTransform, bullets[i].polygon.R);
		}

		//TODO: refactor
		Asteroid asteroid;
		Bullet bullet;
		for (int i = 0; i < asteroids.Count ; i++)
		{
			asteroid = asteroids[i];
			for (int k = 0; k < bullets.Count; k++)
			{
				bullet = bullets[k];
				if(bullet == null)
					continue;

				if((asteroid.cacheTransform.position - bullet.cacheTransform.position).magnitude < asteroid.polygon.R + bullet.polygon.R && 
				   Math2d.IsCollides(asteroid.cacheTransform, asteroid.polygon, bullet.cacheTransform, bullet.polygon))
				{
					Destroy(bullet.gameObject);
					bullets[k] = null; 

					asteroid.Hit(bullet.damage);
					if(asteroid.IsKilled())
					{
						if(asteroid.polygon.R > DestroyTreshold)
						{
							List<Asteroid> parts = SplitAsteroid(asteroid);
							asteroids.AddRange(parts);
						}
						 
						Destroy(asteroid.gameObject);
						asteroids.RemoveAt(i);
					}

					if(asteroid.IsKilled())
					{
						break;
					}
				}
			}
		}
	}

	private void CheckBounds(Transform pTransform, float R)
	{
		Vector3 pos = pTransform.position;
		if(pos.x - R > bounds.xMax)
		{
			pTransform.position = pTransform.position.SetX(bounds.xMin - R);
		}
		else if(pos.x + R < bounds.xMin)
		{
			pTransform.position = pTransform.position.SetX(bounds.xMax + R);
		}

		if(pos.y + R < bounds.yMin)
		{
			pTransform.position = pTransform.position.SetY(bounds.yMax + R);
		}
		else if(pos.y - R > bounds.yMax)
		{
			pTransform.position = pTransform.position.SetY(bounds.yMin - R);
		}
	}

	private List<Asteroid> SplitAsteroid(Asteroid asteriod)
	{
		List<Vector2[]> polys = asteriod.Split();
		List<Asteroid> parts = new List<Asteroid>();

		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
			parts.Add(asteriod);
			return parts;
		}

		foreach(var vertices in polys)
		{
			Polygon polygonPart;
			GameObject polygonGo;
			CreatePolygonByMassCenter(vertices, Color.black, out polygonPart, out polygonGo);
			polygonGo.transform.Translate(asteriod.cacheTransform.position);//TODO: optimise
			polygonGo.transform.RotateAround(asteriod.cacheTransform.position, -Vector3.back, asteriod.cacheTransform.rotation.eulerAngles.z);
			polygonGo.name = "asteroid part";

			Asteroid asteroidPart = polygonGo.AddComponent<Asteroid>();
			asteroidPart.Init(polygonPart);

			Vector3 direction = asteroidPart.cacheTransform.position - asteriod.cacheTransform.position;
			asteroidPart.velocity = direction*2 + asteriod.velocity; 
			asteroidPart.velocity.z = 0;

			parts.Add(asteroidPart);
		}
		return parts;
	}

	private void CreatePolygonByMassCenter(Vector2[] vertices, Color color, out Polygon polygon, out GameObject polygonGo)
	{
		Vector2 pivot = Math2d.GetMassCenter(vertices);
		Math2d.ShiftVertices(vertices, -pivot);
		polygon = new Polygon(vertices);
		polygonGo = new GameObject();
		polygonGo.transform.Translate(new Vector3(pivot.x, pivot.y, 0)); //TODO: optimise
		PolygonCreator.AddRenderComponents (polygon, polygonGo, color);
	}

	private Asteroid CreateAsteroid()
	{
		float size = Random.Range(2f, 8f);
		int vcount = Random.Range(3, 3 + (int)size*2);
		Vector2[] vertices = PolygonCreator.CreatePolygonVertices(size, size/2f, vcount);
		
		Polygon polygon;
		GameObject polygonGo;
		CreatePolygonByMassCenter(vertices, Color.black, out polygon, out polygonGo);
		polygonGo.name = "asteroid";

		Asteroid asteroid = polygonGo.AddComponent<Asteroid>();
		asteroid.Init(polygon);

		return asteroid;
	}

	
}
