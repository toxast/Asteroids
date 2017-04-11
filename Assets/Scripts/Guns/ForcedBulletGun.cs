using UnityEngine;
using System;
using System.Collections;

public class ForcedBulletGun : BulletGun<ForcedBullet>
{
    MForcedBulletGun fdata;
    
    public ForcedBulletGun(Place place, MForcedBulletGun fdata, PolygonGameObject parent)
        : base(place, fdata, parent) {
        this.fdata = fdata;
    }

	protected override void SetCollisionLayer (ForcedBullet bullet)
	{
		bullet.SetLayerNum(CollisionLayers.GetBulletLayerNum(parent.layerLogic), CollisionLayers.ilayerNoCollision);
		var affectedLayer = CollisionLayers.GetLayerCollisions(CollisionLayers.GetBulletLayerNum(parent.layerLogic));
		bullet.InitForcedBullet(fdata, affectedLayer);
	}
}