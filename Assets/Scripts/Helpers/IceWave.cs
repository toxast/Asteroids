using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceWave
{
	public IceWave(Vector2 pos, float radius, IceEffect.Data iceEffect, float effectiveDuration, List<PolygonGameObject> objs, int collision = -1)
	{
		var objectsAroundData = ExplosionData.CollectData (pos, radius, objs, collision);
		foreach(var data in objectsAroundData) {
			var effect = iceEffect.Clone (Mathf.Sqrt(data.distance01) * effectiveDuration);
			data.obj.AddEffect (new IceEffect (effect));
		}
	}
}
