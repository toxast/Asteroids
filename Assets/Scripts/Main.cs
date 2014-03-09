using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{


	Vector2[] spaceshipVertices = new Vector2[]
	{
		new Vector2(2f, 0f),
		new Vector2(-1f, 1f),
		new Vector2(-1f, -1f),
	};

	Vector2[] fastSpaceshipVertices = new Vector2[]
	{
		new Vector2(2f, 0f),
		new Vector2(-0.5f, -1f),
		new Vector2(-2f, -1f),
		new Vector2(-1f, -0.5f),
		new Vector2(-0.5f, 0f),
		new Vector2(-1f, 0.5f),
		new Vector2(-2f, 1f),
		new Vector2(-0.5f, 1f),
	};

	Vector2[] tankSpaceshipVertices = new Vector2[]
	{
		new Vector2(1f, 0f),
		new Vector2(1f, -0.5f),
		new Vector2(0.5f, -0.5f),
		new Vector2(1f, -1f),
		new Vector2(2f, -1f),
		new Vector2(2f, -1.5f),
		new Vector2(0f, -1.5f),
		new Vector2(-0.5f, -1f),
		new Vector2(-1f, -1f),
		new Vector2(-0.5f, -0.5f),
		new Vector2(-1f, 0f),
		new Vector2(-0.5f, 0.5f),
		new Vector2(-1f, 1f),
		new Vector2(-0.5f, 1f),
		new Vector2(0f, 1.5f),
		new Vector2(2f, 1.5f),
		new Vector2(2f, 1f),
		new Vector2(1f, 1f),
		new Vector2(0.5f, 0.5f),
		new Vector2(1f, 0.5f),
	};

	Vector2[] CornSpaceshipVertices = new Vector2[]
	{
		new Vector2(4f, 0f),
		new Vector2(3f, -0.5f),
		new Vector2(3f, -1.5f),
		new Vector2(2.5f, -1.5f),
		new Vector2(2.5f, -0.5f),
		new Vector2(2f, -0.5f),
		new Vector2(1f, -1f),
		new Vector2(1f, -3.5f),
		new Vector2(0f, -4f),
		new Vector2(-1f, -3.5f),
		new Vector2(-1f, -1f),
		new Vector2(-3f, -0.5f),
		new Vector2(-3f, -1f),
		new Vector2(-3.5f, -1.5f),
		new Vector2(-4f,  -1f),
		new Vector2(-4, -0.5f),
		new Vector2(-4.5f, -0.5f),
		new Vector2(-5f, 0f),
		new Vector2(-4.5f, 0.5f),
		new Vector2(-4f, 0.5f),
		new Vector2(-4f,  1f),
		new Vector2(-3.5f, 1.5f),
		new Vector2(-3f, 1f),
		new Vector2(-3f, 0.5f),
		new Vector2(-1f, 1f),
		new Vector2(-1f, 3.5f),
		new Vector2(0f, 4f),
		new Vector2(1f, 3.5f),
		new Vector2(1f, 1f),
		new Vector2(2f, 0.5f),
		new Vector2(2.5f, 0.5f),
		new Vector2(2.5f, 1.5f),
		new Vector2(3f, 1.5f),
		new Vector2(3f, 0.5f), 
	};

	SpaceShip spaceship;
	List <Asteroid> asteroids = new List<Asteroid>();
	List <Bullet> bullets = new List<Bullet>();


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
			asteroid.cacheTransform.position +=  new Vector3(Mathf.Cos(angle)*len, Mathf.Sin(angle)*len, 0);
			asteroids.Add(asteroid);
		}
	}

	private void CreateSpaceShip()
	{
		GameObject spaceShipGo;
		Polygon spaceshipPoly;
		CreatePolygonByMassCenter(tankSpaceshipVertices, out spaceshipPoly, out spaceShipGo);
		spaceship = spaceShipGo.AddComponent<SpaceShip>();
		spaceship.Init(spaceshipPoly);
		spaceship.FireEvent += OnFire;
		spaceShipGo.name = "Spaceship";
		//TODO optimize
		MeshRenderer renderer = spaceship.GetComponent<MeshRenderer>();
		renderer.material.SetColor("_Color", Color.blue);
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
		CreatePolygonByMassCenter(bulletVertices, out bulletPoly, out bulletObj);
		Bullet bullet = bulletObj.AddComponent<Bullet>();
		bullet.Init(bulletPoly, spaceship.cacheTransform.right);
		bullet.cacheTransform.position = spaceship.cacheTransform.position + spaceship.cacheTransform.right;
		bulletObj.name = "bullet";
		//TODO optimize
		MeshRenderer renderer = bullet.GetComponent<MeshRenderer>();
		renderer.material.SetColor("_Color", Color.red);


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

		//TODO: polygon collisions, refactor
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


					if(asteroid.polygon.R*2f/3f > spaceship.polygon.R)
					{
						//split asteroid 
						List<Asteroid> parts = SplitAsteroid(asteroid);
						if(parts.Count < 2)
						{
							Debug.LogWarning("couldnt split asteroid");
						}
						asteroids.AddRange(parts);
					}
					 
					Destroy(bullet.gameObject);
					bullets[k] = null; 

					Destroy(asteroid.gameObject);
					asteroids.RemoveAt(i);

					break;
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

	/*private void CreateRandomPolygonGameObject(out Polygon polygon, out GameObject polygonGo)
	{
		float size = Random.Range(3f, 6f);
		int vcount = Random.Range(12, 18);
		Vector2[] vertices = PolygonCreator.CreatePolygonVertices(size, size/2f, vcount);

		CreatePolygonByMassCenter(vertices, out polygon, out polygonGo);
		polygonGo.name = "main polygon";
	}*/

	private List<Asteroid> SplitAsteroid(Asteroid asteriod)
	{
		List<Vector2[]> polys = asteriod.Split();
		List<Asteroid> asteroids = new List<Asteroid>();

		if(polys.Count < 2)
		{
			Debug.LogError("couldnt split asteroid");
			asteroids.Add(asteriod);
			return asteroids;
		}

		foreach(var vertices in polys)
		{
			Polygon polygonPart;
			GameObject polygonGo;
			CreatePolygonByMassCenter(vertices, out polygonPart, out polygonGo);
			polygonGo.transform.Translate(asteriod.cacheTransform.position);//TODO: optimise
			polygonGo.transform.RotateAround(asteriod.cacheTransform.position, -Vector3.back, asteriod.cacheTransform.rotation.eulerAngles.z);
			polygonGo.name = "asteroid part";

			Asteroid asteroidPart = polygonGo.AddComponent<Asteroid>();
			asteroidPart.Init(polygonPart);

			Vector2 diraction = asteroidPart.cacheTransform.position - asteriod.cacheTransform.position;
			asteroidPart.velocity = diraction*2 + asteriod.velocity; 

			asteroids.Add(asteroidPart);
		}
		return asteroids;
	}

	private void CreatePolygonByMassCenter(Vector2[] vertices, out Polygon polygon, out GameObject polygonGo)
	{
		Vector2 pivot = Math2d.GetMassCenter(vertices);
		Math2d.ShiftVertices(vertices, -pivot);
		polygon = new Polygon(vertices);
		polygonGo = new GameObject();
		polygonGo.transform.Translate(new Vector3(pivot.x, pivot.y, 0)); //TODO: optimise
		PolygonCreator.AddRenderComponents (polygon, polygonGo);
	}

	private Asteroid CreateAsteroid()
	{
		float size = Random.Range(2f, 8f);
		int vcount = Random.Range(3, (int)size*4);
		Vector2[] vertices = PolygonCreator.CreatePolygonVertices(size, size/2f, vcount);
		
		Polygon polygon;
		GameObject polygonGo;
		CreatePolygonByMassCenter(vertices, out polygon, out polygonGo);
		polygonGo.name = "asteroid";

		Asteroid asteroid = polygonGo.AddComponent<Asteroid>();
		asteroid.Init(polygon);

		return asteroid;
	}

	
}
