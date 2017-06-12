using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEarthBossData : MPolygonData {

	[SerializeField] public float rotationSpeed;

	[SerializeField] public MPolygonData shoulder;
	[SerializeField] public KeepOrientationEffect.Data shoulderOrientationData;
	[SerializeField] public KeepPositionEffect.Data shoulderPositionData;

	protected override PolygonGameObject CreateInternal (int layerNum)	{
		var spawn = PolygonCreator.CreatePolygonGOByMassCenter<EarthBoss> (verts, color);
		spawn.InitPolygonGameObject (physical);
		spawn.SetLayerNum (layerNum);
		spawn.targetSystem = new TargetSystem (spawn);
		spawn.Init (this);
		return spawn;
	}
}


