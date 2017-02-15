using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropCollector
{
	public float force;
	public float range;
	private float rangeSqr;

	public DropCollector(float force, float range)
	{
		this.force = force;
		this.range = range;
		rangeSqr = range * range;
	}

	public void Pull<T>(Vector2 pos, List<T> drops, float delta)
		where T: PolygonGameObject
	{
		foreach(var d in drops)
		{
			var dist = pos - d.position;
			if(dist.sqrMagnitude < rangeSqr)
			{
				float magnitude = dist.magnitude;
				d.position += force * (1 - magnitude/range) * (dist/magnitude);
			}
		}
	}

}
