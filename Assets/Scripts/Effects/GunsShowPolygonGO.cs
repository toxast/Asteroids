using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsShowPolygonGO : PolygonGameObject
{
	List<PolygonGameObject> gunsObjects;
	public void InitGunsShowPolygonGO(List<PolygonGameObject> gunsObjects){
		this.gunsObjects = new List<PolygonGameObject> (gunsObjects);
	}

	public override void Tick (float delta)
	{
		base.Tick (delta);
		for (int i = 0; i < gunsObjects.Count; i++) {
			var obj = gunsObjects [i];
			obj.velocity = Vector2.zero;
			obj.Tick (delta);
			obj.velocity = this.velocity;
			obj.TickGuns(delta);
			obj.Shoot();
		}
	}
}
