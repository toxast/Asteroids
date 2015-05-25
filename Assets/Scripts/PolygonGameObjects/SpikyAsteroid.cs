using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikyAsteroid : Asteroid
{
	public event System.Action<Asteroid> SpikeAttack;

	private class Spike
	{
		// index
		//a-/\-b  a points to the tip, b points away from it

		public int index;
		public Edge a;
		public Edge b;

		public Spike(Edge e1, Edge e2, int index)
		{
			a = e1;
			b = e2;
			this.index = index;
		}
	}

//	private float detectionDistance = 70f;
//	private float detectionDistanceSqr;
	private float regrowPause = 3f;
	private float spikeSpeed = 45f;
	private float growSpeed = 0.1f;

	private List<Spike> spikesLeft = new List<Spike>();

	public void InitSpikyAsteroid (int[] spikes, SpikeShooterInitData data)
	{
		InitAsteroid (data.physical, data.speed, data.rotation);
		this.spikeSpeed = data.spikeVelocity;

		foreach(int spikeVertex in spikes)
		{
			int previous = polygon.Previous(spikeVertex);
			Spike spike = new Spike(polygon.edges[previous], polygon.edges[spikeVertex], spikeVertex);
			spikesLeft.Add(spike);
		}
	}

	void Start()
	{
		StartCoroutine (CheckForTarget ());
	}

	IEnumerator CheckForTarget()
	{
		float checkInterval = 0.5f;

		while(true)
		{
			if(!Main.IsNull(target))
			{
				float angle = cacheTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
				float cosA = Mathf.Cos(angle);
				float sinA = Mathf.Sin(angle);

				var randomSpeed = spikeSpeed + UnityEngine.Random.Range(-spikeSpeed/5f, 0);
				AimSystem aim = new AimSystem(target.position, target.velocity, position, randomSpeed);
				if(aim.canShoot)
				{
					for (int i = spikesLeft.Count - 1; i >= 0; i--) 
					{
						Spike spike = spikesLeft[i];
						
						Edge e1  = Math2d.RotateEdge(spike.a, cosA, sinA); 
						Vector2 spikeDirection = e1.p2;
					
						bool inFrontOfSpike = Math2d.Cos(spikeDirection, aim.direction) > 0.98f;
						
						if(inFrontOfSpike)
						{
							//split spike off
							List<Vector2[]> parts = polygon.SplitBy2Vertices(polygon.Previous(spike.index), polygon.Next(spike.index));
							Vector2[] spikePart = Math2d.RotateVerticesRad(parts[1], angle);
							
							spikesLeft.RemoveAt(i);
							StartCoroutine(GrowSpike(spike.index, spike.a.p2));
							
							Asteroid spikeAsteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(spikePart, ObjectsCreator.defaultEnemyColor);
							spikeAsteroid.InitPolygonGameObject(new PhysicalData(this.density, this.healthModifier, this.collisionDefence, this.collisionAttackModifier));
							spikeAsteroid.position += position;
							spikeAsteroid.rotation = 0f;
							spikeAsteroid.velocity = spikeSpeed * spikeDirection.normalized;
							
							//change mesh and polygon
							ChangeVertex(spike.index, (spike.a.p1 + spike.b.p2) / 2f);
							
							SpikeAttack(spikeAsteroid);
						}
					}
				}
			}
			yield return new WaitForSeconds(checkInterval);
		}
	}

	private IEnumerator GrowSpike(int indx, Vector2 tip)
	{
		yield return new WaitForSeconds(regrowPause);

		float length = tip.magnitude;

		bool growFinished = false;
		while(!growFinished)
		{
			Vector3 v = mesh.vertices[indx];
			v =  v.normalized * (v.magnitude + growSpeed);
			ChangeVertex(indx, new Vector2(v.x, v.y));

			if(v.magnitude > length)
			{
				int previous = polygon.Previous(indx);
				Spike spike = new Spike(polygon.edges[previous], polygon.edges[indx], indx);
				spikesLeft.Add(spike);

				growFinished = true;
			}

			yield return new WaitForSeconds(0.3f);
		}
	}
}
