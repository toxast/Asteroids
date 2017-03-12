using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballGun : RocketLauncher {
	new MFireballGunData data;

	public FireballGun(Place place, MFireballGunData data, PolygonGameObject parent)
		:base(place, data, parent)	{ 
		this.data = data;
	}

	protected override Vector2[] GetVerts ()
	{
		return PolygonCreator.CreatePerfectPolygonVertices(data.ballRadius, 6);
	}

	protected override void InitPolygonGameObject (SpaceShip bullet, PhysicalData ph)
	{
		base.InitPolygonGameObject (bullet, ph);
		bullet.burnDotData = data.dot;
	}

	protected override bool CreateExplosion ()	{
		return false;
	}
}
