using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MTowerRotatingData : MTowerData
{
	public float rotationSpeedWhileShooting = 10f;

	protected override PolygonGameObject CreateInternal(int layer)
	{
		return ObjectsCreator.CreateTowerRotating(this, layer);
	}
}
