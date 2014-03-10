using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikyAsteroid : Asteroid
{
	public event System.Action<SpikyAsteroid, SpikyAsteroid, Asteroid> SpikeAttack;

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

	private Transform target;
	private List<Spike> spikesLeft = new List<Spike>();

	public void Init (Transform ptraget, int[] spikes)
	{
		base.Init ();

		foreach(int spikeVertex in spikes)
		{
			int previous = polygon.Previous(spikeVertex);
			Spike spike = new Spike(polygon.edges[previous], polygon.edges[spikeVertex], spikeVertex);
			spikesLeft.Add(spike);
		}

		this.target = ptraget;
	}

	void Start()
	{
		StartCoroutine (CheckForTarget ());
	}

	IEnumerator CheckForTarget()
	{
		float thesholdDistance = 24f;
		float checkInterval = 0.5f;
		float thesholdDistanceSqr = thesholdDistance * thesholdDistance;
		
		while(true)
		{
			Vector2 tragetPos = new Vector2(target.position.x - cacheTransform.position.x, target.position.y - cacheTransform.position.y);
			if(tragetPos.sqrMagnitude < thesholdDistanceSqr)
			{
				float angle = cacheTransform.rotation.eulerAngles.z * Mathf.PI / 180f;
				float cosA = Mathf.Cos(angle);
				float sinA = Mathf.Sin(angle);

				for (int i = spikesLeft.Count - 1; i >= 0; i--) 
				{
					Spike spike = spikesLeft[i];

					Edge e1  = Math2d.RotateEdge(spike.a, cosA, sinA); 
					Edge e2  = Math2d.RotateEdge(spike.b, cosA, sinA);
					Vector2 spikeBase = e2.p2 - e1.p1;

					float sign =  Mathf.Sign(Math2d.Rotate(ref spikeBase, ref tragetPos));
					if(sign > 0)
					{
						Vector2 toTheTip1 = e1.p2 - e1.p1;
						Vector2 toTheTip2 = e2.p1 - e2.p2;

						float sign1 = Mathf.Sign(Math2d.Rotate(ref toTheTip1, ref tragetPos));
						float sign2 = Mathf.Sign(Math2d.Rotate(ref toTheTip2, ref tragetPos));

						bool inRange = (sign1 != sign2);

						if(inRange)
						{
							//split spike off
							List<Vector2[]> parts = polygon.SplitBy2Vertices(polygon.Previous(spike.index), polygon.Next(spike.index));
							Vector2[] mainPart = Math2d.RotateVertices(parts[0], angle);
							Vector2[] spikePart = Math2d.RotateVertices(parts[1], angle);

							SpikyAsteroid mainAsteroid = PolygonCreator.CreatePolygonGOByMassCenter<SpikyAsteroid>(mainPart, Color.black);
							mainAsteroid.cacheTransform.position += cacheTransform.position;

							int k = 0;
							int[] spikes = new int[spikesLeft.Count - 1];
							for (int n = 0; n < spikesLeft.Count; n++) 
							{
								if(n != i)
								{
									spikes[k] = (spike.index < spikesLeft[n].index) ? spikesLeft[n].index - 1 : spikesLeft[n].index;
									k++;
								}
							}

							mainAsteroid.Init(target, spikes);
							mainAsteroid.velocity = this.velocity;
							mainAsteroid.rotation = this.rotation;

							Asteroid spikeAsteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(spikePart, Color.black);
							spikeAsteroid.Init();
							spikeAsteroid.cacheTransform.position += cacheTransform.position;
							spikeAsteroid.rotation = 0f;
							spikeAsteroid.velocity = (e1.p2 - (e1.p1 + e2.p2)/2f).normalized *15f;

							SpikeAttack(this, mainAsteroid, spikeAsteroid);

							spikesLeft.RemoveAt(i);
						}
					}
					
				}
			}
			yield return new WaitForSeconds(checkInterval);
		}
	}

}
