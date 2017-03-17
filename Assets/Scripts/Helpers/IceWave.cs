using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceWave
{
	public IceWave(Vector2 pos, float radius, IceEffect.Data iceEffect, float effectiveDuration, List<PolygonGameObject> objs, int collision = -1, bool distanceMatters = true)
	{
		var objectsAroundData = ExplosionData.CollectData (pos, radius, objs, collision);
		foreach(var data in objectsAroundData) {
			IceEffect.Data cloneData;
			if (distanceMatters) {
				cloneData = iceEffect.Clone (Mathf.Sqrt (data.distance01) * effectiveDuration);
			} else {
				cloneData = iceEffect.Clone ();
			}
			data.obj.AddEffect (new IceEffect (cloneData));
		}
	}
}
