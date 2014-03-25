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
		float thesholdDistance = 32f;
		float checkInterval = 0.5f;
		float thesholdDistanceSqr = thesholdDistance * thesholdDistance;
		
		while(true)
		{
			Vector2 tragetPos = new Vector2(target.position.x - cacheTransform.position.x, target.position.y - cacheTransform.position.y);
			if(tragetPos.sqrMagnitude < thesholdDistanceSqr)
			{
				float angle = cacheTransform.rotation.eulerAngles.z * Math2d.PIdiv180;
				float cosA = Mathf.Cos(angle);
				float sinA = Mathf.Sin(angle);

				for (int i = spikesLeft.Count - 1; i >= 0; i--) 
				{
					Spike spike = spikesLeft[i];

					Edge e1  = Math2d.RotateEdge(spike.a, cosA, sinA); 
					Edge e2  = Math2d.RotateEdge(spike.b, cosA, sinA);
					Vector2 spikeBase = e2.p2 - e1.p1;

					float sign =  Mathf.Sign(Math2d.Cross(ref spikeBase, ref tragetPos));
					if(sign > 0)
					{
						Vector2 toTheTip1 = e1.p2 - e1.p1;
						Vector2 toTheTip2 = e2.p1 - e2.p2;

						float sign1 = Mathf.Sign(Math2d.Cross(ref toTheTip1, ref tragetPos));
						float sign2 = Mathf.Sign(Math2d.Cross(ref toTheTip2, ref tragetPos));

						bool inRange = (sign1 != sign2);

						if(inRange)
						{
							//split spike off
							List<Vector2[]> parts = polygon.SplitBy2Vertices(polygon.Previous(spike.index), polygon.Next(spike.index));
							Vector2[] spikePart = Math2d.RotateVertices(parts[1], angle);

							spikesLeft.RemoveAt(i);
 							StartCoroutine(GrowSpike(spike.index, spike.a.p2));

							Asteroid spikeAsteroid = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(spikePart, Color.black);
							spikeAsteroid.Init();
							spikeAsteroid.cacheTransform.position += cacheTransform.position;
							spikeAsteroid.rotation = 0f;
							spikeAsteroid.velocity = (e1.p2 - (e1.p1 + e2.p2)/2f).normalized *15f;

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

	//TODO: game obj
	private void ChangeVertex(int indx, Vector2 v)
	{
		Vector3[] vertx3d = mesh.vertices;
		vertx3d[indx] =  new Vector3(v.x, v.y, 0f);
		mesh.vertices = vertx3d;

		polygon.ChangeVertex(indx, v);
	}

	private IEnumerator GrowSpike(int indx, Vector2 tip)
	{
		float reqlen = tip.magnitude;
		float deltaGrow = 0.04f;
		bool growFinished = false;

		yield return new WaitForSeconds(3f);

		while(!growFinished)
		{
			Vector3 v = mesh.vertices[indx];
			v =  v.normalized * (v.magnitude + deltaGrow);
			ChangeVertex(indx, new Vector2(v.x, v.y));

			if(v.magnitude > reqlen)
			{
				int previous = polygon.Previous(indx);
				Spike spike = new Spike(polygon.edges[previous], polygon.edges[indx], indx);
				spikesLeft.Add(spike);

				growFinished = true;
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}
