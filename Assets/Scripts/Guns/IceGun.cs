using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceGun : RocketLauncher {
	new MIceGunData data;

	public IceGun(Place place, MIceGunData data, PolygonGameObject parent)
		:base(place, data, parent)	{ 
		this.data = data;
	}

	protected override void InitPolygonGameObject (SpaceShip bullet, PhysicalData ph)
	{
		base.InitPolygonGameObject (bullet, ph);
		bullet.iceEffectData = data.iceData;
	}

	protected override PolygonGameObject.DestructionType SetDestructionType () {
		return PolygonGameObject.DestructionType.eSptilOnlyOnHit;
	}

	protected override bool CreateExplosion ()	{
		return false;
	}
}
