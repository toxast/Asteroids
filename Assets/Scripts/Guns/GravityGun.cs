using UnityEngine;
using System;
using System.Collections;

public class GravityGun : BulletGun<GravityBullet>
{
    new MGravityGunData data;

    public GravityGun(Place place, MGravityGunData data, PolygonGameObject parent)
        :base(place, data, parent)
    {
        this.data = data;
    }

	protected override void InitPolygonGameObject (GravityBullet bullet, PhysicalData ph)
	{
		base.InitPolygonGameObject (bullet, ph);
		float rangeMultiplier = parent.heavyBulletData != null ? Mathf.Sqrt(parent.heavyBulletData.multiplier) : 1f;
		int affectLayer = CollisionLayers.GetGravityBulletCollisions (CollisionLayers.GetBulletLayerNum(parent.layerLogic));
		bullet.InitGravityBullet(affectLayer, data, rangeMultiplier);
	}

//	protected override PolygonGameObject.DestructionType SetDestructionType ()
//	{
//		return  PolygonGameObject.DestructionType.eDisappear;
//	}

	protected override void SetCollisionLayer (GravityBullet bullet) {
		bullet.SetLayerNum(CollisionLayers.GetBulletLayerNum(parent.layerLogic), CollisionLayers.ilayerNoCollision);
	}

  
}
