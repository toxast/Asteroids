using UnityEngine;
using System;
using System.Collections;

internal class ObjectGun<T> : BulletGun<T> where T : PolygonGameObject
{
	public ObjectGun(Place place, MGunData data, PolygonGameObject parent)
		:base(place, data, parent)
	{
		
	}

	protected override void InitPolygonGameObject (T bullet, PhysicalData ph) {
		base.InitPolygonGameObject (bullet, ph);
		bullet.targetSystem = new TargetSystem (bullet);
	}

	protected override void SetCollisionLayer (T bullet)
	{
		bullet.SetLayerNum(CollisionLayers.GetSpawnedLayer (parent.layerLogic));
		bullet.showOffScreen = false;
	}

	protected override void AddToMainLoop (T b)
	{
		Singleton<Main>.inst.Add2Objects (b);
	}
}
