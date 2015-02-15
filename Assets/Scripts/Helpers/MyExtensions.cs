using UnityEngine;
using System.Collections;

public static class MyExtensions
{
	public static void SetCollisionLayer(this PolygonGameObject g, int layerNum)
	{
		g.layer = 1 << layerNum;
		g.collision = GlobalConfig.GetLayerCollisions (layerNum);
	}
}
