using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikyAsteroid : Asteroid
{
//	public event System.Action<Asteroid> SpikeAttack;

	private class Spike
	{
		// index
		//a-/\-b  a points to the tip, b points away from it

		public int index;
		public Edge a;
		public Edge b;
		public float lastCos = 0;

		public Spike(Edge e1, Edge e2, int index)
		{
			a = e1;
			b = e2;
			this.index = index;
		}
	}

//	private float detectionDistance = 70f;
//	private float detectionDistanceSqr;
	private float regrowPause = 2f;
	private float spikeSpeed;
	private float growSpeed;
	MSpikyData  data;

	private List<Spike> spikesLeft = new List<Spike>();

	public void InitSpikyAsteroid (int[] spikes, MSpikyData data)	{
		this.data = data;
		reward = data.reward;
		InitAsteroid (data.physical, data.speed, data.rotation);
		this.spikeSpeed = data.spikeVelocity;
		this.growSpeed = data.growSpeed;
		foreach(int spikeVertex in spikes) {
			int previous = polygon.Previous(spikeVertex);
			Spike spike = new Spike(polygon.edges[previous], polygon.edges[spikeVertex], spikeVertex);
			spikesLeft.Add(spike);
		}

		var accData = data.accuracy;
		accuracy = accData.startingAccuracy;
		if (accData.isDynamic) {
			StartCoroutine (AccuracyChanger (accData));
		}
		
		StartCoroutine (CheckForTarget ());
	}

	IEnumerator CheckForTarget()
	{
		float longRefreshInterval = 0.2f;
		float currentRefreshInterval = longRefreshInterval;

		while(true)
		{
			bool anySpikeNearShootingPlace = false;
			if (!Main.IsNull (target)) {
				float angle = cacheTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
				float cosA = Mathf.Cos (angle);
				float sinA = Mathf.Sin (angle);

				AimSystem aim = new AimSystem (target.position, accuracy * target.velocity, position, spikeSpeed * 1.05f);
				if (aim.canShoot && aim.time < 3f) {
					for (int i = spikesLeft.Count - 1; i >= 0; i--) {
						Spike spike = spikesLeft [i];

						Edge e1 = Math2d.RotateEdge (spike.a, cosA, sinA); 
						Vector2 spikeDirection = e1.p2;
					
						var oldCos = spike.lastCos;
						var newCos = Math2d.Cos (spikeDirection, aim.directionDist);
						spike.lastCos = newCos;

						anySpikeNearShootingPlace |= newCos > 0.98;

						bool inFrontOfSpike = oldCos > 0.98f && newCos > 0.98f && newCos < oldCos;
						
						if (inFrontOfSpike) {
							ShootSpike (i);
						}
					}
				}
			}

			currentRefreshInterval = anySpikeNearShootingPlace ? 0 : longRefreshInterval;

			yield return new WaitForSeconds(currentRefreshInterval);
		}
	}

	public override void HandleDestroying ()
	{
		base.HandleDestroying ();
		for (int i = spikesLeft.Count - 1; i >= 0; i--) {
			if (Math2d.Chance (data.chanceShootSpikeAtDeath)) {
				ShootSpike (i);
			}
		}
	}

	private void ShootSpike(int i){
		var spike = spikesLeft [i];
		float angle = cacheTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		//split spike off
		List<Vector2[]> parts = polygon.SplitBy2Vertices(polygon.Previous(spike.index), polygon.Next(spike.index));
		Vector2[] spikePart = Math2d.RotateVerticesRad(parts[1], angle);
		Vector2 spikeDirection = Math2d.RotateVertex (spike.a.p2, angle);
		spikesLeft.RemoveAt(i);
		StartCoroutine(GrowSpike(spike.index, spike.a.p2));

		Asteroid spikeGO = PolygonCreator.CreatePolygonGOByMassCenter<Asteroid>(spikePart, this.GetColor());
		spikeGO.InitPolygonGameObject(new PhysicalData(this.density, this.healthModifier, this.collisionDefence, this.collisionAttackModifier));
		spikeGO.SetCollisionLayerNum (this.layerNum);
		spikeGO.position += position;
		spikeGO.rotation = 0f;
		spikeGO.velocity = spikeSpeed * spikeDirection.normalized;
		spikeGO.priority = PolygonGameObject.ePriorityLevel.LOW;
		//change mesh and polygon
		ChangeVertex(spike.index, (spike.a.p1 + spike.b.p2) / 2f);

		Singleton<Main>.inst.HandleSpikeAttack (spikeGO);
	}

	private IEnumerator GrowSpike(int indx, Vector2 tip)
	{
		yield return new WaitForSeconds(regrowPause);

		float length = tip.magnitude;

		bool growFinished = false;
		while(!growFinished)
		{
			Vector3 v = mesh.vertices[indx];
			var magnitude = v.magnitude;

			if (magnitude + growSpeed < length) 
			{
				v = v.normalized * (magnitude + growSpeed);
				ChangeVertex (indx, new Vector2 (v.x, v.y));
			} 
			else 
			{
				v = v.normalized * length;
				ChangeVertex (indx, new Vector2 (v.x, v.y));
				int previous = polygon.Previous (indx);
				Spike spike = new Spike (polygon.edges [previous], polygon.edges [indx], indx);
				spikesLeft.Add (spike);

				growFinished = true;
			}

			yield return new WaitForSeconds(0.3f);
		}
	}

	float accuracy = 0;
	private IEnumerator AccuracyChanger(AccuracyData data) {
		Vector2 lastDir = Vector2.one; //just not zero
		float dtime = data.checkDtime;
		while (true) {
			if (!Main.IsNull (target)) {
				AIHelper.ChangeAccuracy (ref accuracy, ref lastDir, target, data);
			}
			yield return new WaitForSeconds (dtime);
		}
	}
}
