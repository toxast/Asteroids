using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPolygonData : MSpawnDataBase, IGotShape
{
	public PhysicalData physical;
	public Vector2[] verts;
	public Color color;

	public Vector2[] iverts {get {return verts;} set{verts = value;}}

	protected override PolygonGameObject CreateInternal(int layerNum)
	{
		var spawn = PolygonCreator.CreatePolygonGOByMassCenter<PolygonGameObject> (verts, color);
		spawn.InitPolygonGameObject (physical);
		spawn.SetLayerNum (layerNum);
		return spawn;
	}
}

